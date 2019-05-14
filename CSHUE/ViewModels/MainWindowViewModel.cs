using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSGSI;
using CSGSI.Nodes;
using CSHUE.Helpers;
using CSHUE.Views;
using Microsoft.Win32;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
// ReSharper disable FunctionNeverReturns
// ReSharper disable SwitchStatementMissingSomeCases

namespace CSHUE.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        public bool WindowMinimized;
        public bool AllowStartLightsChecking;

        public Config ConfigPage;
        public Donate DonatePage;
        public About AboutPage;
        public Home HomePage;
        public Settings SettingsPage;

        private List<Light> _globalLightsBackup;
        private bool _alreadySetLights;
        private bool _alreadyMinimized;
        private bool? _lastSetState;
        private bool _previousState;
        private bool _firstCsgoIteration = true;
        private bool _firstTimeIteration = true;

        private GameState _lastgs;
        private bool _blockLightChange;
        private bool _isPlanted;
        private bool _flashedZeroed = true;
        private int _killAmount = 60000;
        private int _deathAmount = 60000;
        private bool _mainMenuState = true;

        #endregion

        #region Properties

        private WindowState _windowState = WindowState.Normal;
        public WindowState WindowState
        {
            get =>
                _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged();
            }
        }

        private string _bridgeIp = "";
        public static ILocalHueClient Client { get; set; }

        private Visibility _notifyIconVisibility = Visibility.Collapsed;
        public Visibility NotifyIconVisibility
        {
            get =>
                _notifyIconVisibility;
            set
            {
                _notifyIconVisibility = value;
                OnPropertyChanged();
            }
        }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get =>
                _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void Navigate(Frame page, object sender)
        {
            switch (((Grid)sender).Name)
            {
                case "Config":
                    page.Navigate(ConfigPage);
                    break;
                case "Donate":
                    page.Navigate(DonatePage);
                    break;
                case "Settings":
                    page.Navigate(SettingsPage);
                    break;
                case "About":
                    page.Navigate(AboutPage);
                    break;
                default:
                    page.Navigate(HomePage);
                    break;
            }
        }

        public void Navigate(Frame page, string pageName)
        {
            switch (pageName)
            {
                case "Config":
                    page.Navigate(ConfigPage);
                    break;
                case "Donate":
                    page.Navigate(DonatePage);
                    break;
                case "Settings":
                    page.Navigate(SettingsPage);
                    break;
                case "About":
                    page.Navigate(AboutPage);
                    break;
                default:
                    page.Navigate(HomePage);
                    break;
            }
        }

        #endregion

        #region Initializers

        public void CreateInstances()
        {
            ConfigPage = new Config();
            DonatePage = new Donate();
            HomePage = new Home();
            SettingsPage = new Settings();
            AboutPage = new About();
        }

        public static async Task SetDefaultLightsSettings()
        {
            if (Client == null) return;

            var allLights = (await Client.GetLightsAsync()).ToList();

            if (Properties.Settings.Default.MainMenu == null)
                Properties.Settings.Default.MainMenu = new EventProperty();
            if (Properties.Settings.Default.PlayerGetsKill == null)
                Properties.Settings.Default.PlayerGetsKill = new EventBrightnessProperty();
            if (Properties.Settings.Default.PlayerGetsKilled == null)
                Properties.Settings.Default.PlayerGetsKilled = new EventBrightnessProperty();
            if (Properties.Settings.Default.PlayerGetsFlashed == null)
                Properties.Settings.Default.PlayerGetsFlashed = new EventProperty();
            if (Properties.Settings.Default.TerroristsWin == null)
                Properties.Settings.Default.TerroristsWin = new EventProperty();
            if (Properties.Settings.Default.CounterTerroristsWin == null)
                Properties.Settings.Default.CounterTerroristsWin = new EventProperty();
            if (Properties.Settings.Default.RoundStarts == null)
                Properties.Settings.Default.RoundStarts = new EventProperty();
            if (Properties.Settings.Default.FreezeTime == null)
                Properties.Settings.Default.FreezeTime = new EventBrightnessProperty();
            if (Properties.Settings.Default.Warmup == null)
                Properties.Settings.Default.Warmup = new EventBrightnessProperty();
            if (Properties.Settings.Default.BombExplodes == null)
                Properties.Settings.Default.BombExplodes = new EventBrightnessProperty();
            if (Properties.Settings.Default.BombPlanted == null)
                Properties.Settings.Default.BombPlanted = new EventProperty();
            if (Properties.Settings.Default.BombBlink == null)
                Properties.Settings.Default.BombBlink = new EventBrightnessProperty();

            var rnd = new Random();

            if (Properties.Settings.Default.MainMenu.Lights == null)
            {
                Properties.Settings.Default.MainMenu.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.MainMenu.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17), (byte)rnd.Next(240, 256)),
                        Brightness = (byte)rnd.Next(184, 201)
                    });
            }
            if (Properties.Settings.Default.MainMenu.SelectedLights == null)
            {
                Properties.Settings.Default.MainMenu.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.MainMenu.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.PlayerGetsKill.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsKill.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKill.Lights.Add(new UniqueBrightnessLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(0, 17), (byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(240, 256),
                        OnlyBrightness = true
                    });
            }
            if (Properties.Settings.Default.PlayerGetsKill.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsKill.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKill.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.PlayerGetsKilled.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsKilled.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKilled.Lights.Add(new UniqueBrightnessLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(120, 137),
                        OnlyBrightness = true
                    });
            }
            if (Properties.Settings.Default.PlayerGetsKilled.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsKilled.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKilled.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.PlayerGetsFlashed.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsFlashed.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsFlashed.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256)),
                        Brightness = (byte)rnd.Next(240, 256)
                    });
            }
            if (Properties.Settings.Default.PlayerGetsFlashed.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsFlashed.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsFlashed.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.TerroristsWin.Lights == null)
            {
                Properties.Settings.Default.TerroristsWin.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.TerroristsWin.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(162, 179), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(184, 201)
                    });
            }
            if (Properties.Settings.Default.TerroristsWin.SelectedLights == null)
            {
                Properties.Settings.Default.TerroristsWin.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.TerroristsWin.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.CounterTerroristsWin.Lights == null)
            {
                Properties.Settings.Default.CounterTerroristsWin.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.CounterTerroristsWin.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17), (byte)rnd.Next(240, 256)),
                        Brightness = (byte)rnd.Next(184, 201)
                    });
            }
            if (Properties.Settings.Default.CounterTerroristsWin.SelectedLights == null)
            {
                Properties.Settings.Default.CounterTerroristsWin.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.CounterTerroristsWin.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.RoundStarts.Lights == null)
            {
                Properties.Settings.Default.RoundStarts.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.RoundStarts.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(0, 17), (byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(184, 201)
                    });
            }
            if (Properties.Settings.Default.RoundStarts.SelectedLights == null)
            {
                Properties.Settings.Default.RoundStarts.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.RoundStarts.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.FreezeTime.Lights == null)
            {
                Properties.Settings.Default.FreezeTime.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.FreezeTime.Lights.Add(new UniqueBrightnessLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256)),
                        Brightness = (byte)rnd.Next(120, 137),
                        OnlyBrightness = true
                    });
            }
            if (Properties.Settings.Default.FreezeTime.SelectedLights == null)
            {
                Properties.Settings.Default.FreezeTime.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.FreezeTime.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.Warmup.Lights == null)
            {
                Properties.Settings.Default.Warmup.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.Warmup.Lights.Add(new UniqueBrightnessLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256), (byte)rnd.Next(240, 256)),
                        Brightness = (byte)rnd.Next(120, 137),
                        OnlyBrightness = true
                    });
            }
            if (Properties.Settings.Default.Warmup.SelectedLights == null)
            {
                Properties.Settings.Default.Warmup.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.Warmup.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.BombExplodes.Lights == null)
            {
                Properties.Settings.Default.BombExplodes.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombExplodes.Lights.Add(new UniqueBrightnessLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(240, 256),
                        OnlyBrightness = true
                    });
            }
            if (Properties.Settings.Default.BombExplodes.SelectedLights == null)
            {
                Properties.Settings.Default.BombExplodes.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombExplodes.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.BombPlanted.Lights == null)
            {
                Properties.Settings.Default.BombPlanted.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombPlanted.Lights.Add(new UniqueLight
                    {
                        Id = i.UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(120, 137)
                    });
            }
            if (Properties.Settings.Default.BombPlanted.SelectedLights == null)
            {
                Properties.Settings.Default.BombPlanted.SelectedLights = new List<string>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombPlanted.SelectedLights.Add(i.UniqueId);
            }

            if (Properties.Settings.Default.BombBlink.Lights == null)
            {
                Properties.Settings.Default.BombBlink.Lights = new List<UniqueBrightnessLight>
                {
                    new UniqueBrightnessLight
                    {
                        Id = allLights.First().UniqueId,
                        Color = Color.FromRgb((byte)rnd.Next(240, 256), (byte)rnd.Next(0, 17), (byte)rnd.Next(0, 17)),
                        Brightness = (byte)rnd.Next(240, 256),
                        OnlyBrightness = true
                    }
                };
            }
            if (Properties.Settings.Default.BombBlink.SelectedLights == null)
            {
                Properties.Settings.Default.BombBlink.SelectedLights = new List<string>
                {
                    allLights.FirstOrDefault().UniqueId
                };
            }
        }

        #endregion

        #region Methods

        #region Connection

        public async void HueAsync()
        {
            _bridgeIp = await GetBridgeIpAsync().ConfigureAwait(false);

            if (_bridgeIp == "") return;

            Client = new LocalHueClient(_bridgeIp);

            await GetAppKeyAsync();
            while (Properties.Settings.Default.AppKey == "")
            {
                await GetAppKeyAsync();
            }

            SettingsPage.ViewModel.UpdateGradients();

            HomePage.ViewModel.SetDone();

            HomePage.StartLightsChecking();

            AllowStartLightsChecking = true;
        }

        public async Task<string> GetBridgeIpAsync()
        {
            var locator = new HttpBridgeLocator();

            List<Q42.HueApi.Models.Bridge.LocatedBridge> bridgeIPs;

            try
            {
                HomePage.ViewModel.SetWarningSearching();

                bridgeIPs = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5.5)).ConfigureAwait(false)).ToList();
            }
            catch
            {
                bridgeIPs = null;
            }

            if (bridgeIPs == null || bridgeIPs.Count < 1)
            {
                HomePage.ViewModel.SetWarningNoHub();

                return "";
            }

            if (bridgeIPs.Count > 1)
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    var list = new List<HubInfoCellViewModel>();

                    WebRequest request;

                    foreach (var b in bridgeIPs)
                    {
                        HomePage.ViewModel.SetWarningValidating();

                        try
                        {
                            request = WebRequest.Create($"http://{b.IpAddress}/debug/clip.html");
                            request.Timeout = 7000;
                            if (((HttpWebResponse)request.GetResponse()).StatusCode == HttpStatusCode.OK)
                                list.Add(new HubInfoCellViewModel
                                {
                                    Text = $"ip: {b.IpAddress}, id: {b.BridgeId}",
                                    Ip = b.IpAddress
                                });
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    if (list.Count < 1)
                    {
                        HomePage.ViewModel.SetWarningNoReachableHubs();

                        return "";
                    }

                    if (list.Count == 1)
                    {
                        return list.ElementAt(0).Ip;
                    }

                    var hubSelector = new HubSelector(list)
                    {
                        Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
                    };
                    hubSelector.ShowDialog();

                    HomePage.ViewModel.SetWarningValidating();

                    try
                    {
                        request = WebRequest.Create($"http://{hubSelector.SelectedBridge}/debug/clip.html");
                        request.Timeout = 7000;
                        if (((HttpWebResponse)request.GetResponse()).StatusCode == HttpStatusCode.OK)
                            return hubSelector.SelectedBridge;
                    }
                    catch
                    {
                        HomePage.ViewModel.SetWarningHubNotAvailable();

                        return "";
                    }

                    return "";
                });
            }

            HomePage.ViewModel.SetWarningValidating();

            try
            {
                var request = WebRequest.Create($"http://{bridgeIPs.First().IpAddress}/debug/clip.html");
                request.Timeout = 7000;
                if (((HttpWebResponse)request.GetResponse()).StatusCode == HttpStatusCode.OK)
                    return bridgeIPs.First().IpAddress;
            }
            catch
            {
                HomePage.ViewModel.SetWarningNoReachableHubs();
            }

            return "";
        }

        public async Task GetAppKeyAsync()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.AppKey))
            {
                HomePage.ViewModel.SetWarningLink();

                while (Properties.Settings.Default.AppKey == "")
                {
                    try
                    {
                        Properties.Settings.Default.AppKey = await Client.RegisterAsync("CSHUE", "Desktop").ConfigureAwait(false);
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }

                Properties.Settings.Default.Save();
            }

            Client.Initialize(Properties.Settings.Default.AppKey);

            try
            {
                await SetDefaultLightsSettings();
            }
            catch
            {
                Properties.Settings.Default.AppKey = "";
                Properties.Settings.Default.Save();
            }
        }

        public void Csgo()
        {
            var gsl = new GameStateListener(3000);
            gsl.NewGameState += OnNewGameState;
            gsl.Start();

            if (Properties.Settings.Default.RunCsgo)
                RunCsgo();

            CheckCsgoProcessLoop();
            CheckTimeLoop();
        }

        #endregion

        #region Loops

        public void CheckTimeLoop()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(60000 - CheckTime());

                    _firstTimeIteration = false;
                }
            })
            { IsBackground = true }.Start();
        }

        public int CheckTime()
        {
            if (!Properties.Settings.Default.AutoActivate)
            {
                _lastSetState = !Properties.Settings.Default.Activated;
                return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
            }

            // For some reason the NumericUpDown was setting sometimes the variables as HH:mm:ss
            // This was crashing the app, so this is a workarround.
            if (Properties.Settings.Default.AutoActivateStart.Length > 5)
            {
                Properties.Settings.Default.AutoActivateStart =
                    Properties.Settings.Default.AutoActivateStart.Substring(0, 5);
            }

            if (Properties.Settings.Default.AutoActivateEnd.Length > 5)
            {
                Properties.Settings.Default.AutoActivateEnd =
                    Properties.Settings.Default.AutoActivateEnd.Substring(0, 5);
            }

            var start = DateTime.ParseExact(Properties.Settings.Default.AutoActivateStart, "HH:mm",
                CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(Properties.Settings.Default.AutoActivateEnd, "HH:mm",
                CultureInfo.InvariantCulture);

            if (end.Hour > start.Hour || end.Hour == start.Hour && end.Minute > start.Minute)
            {
                if (DateTime.Now.Hour > start.Hour ||
                    DateTime.Now.Hour == start.Hour &&
                    DateTime.Now.Minute >= start.Minute)
                {
                    if (DateTime.Now.Hour < end.Hour ||
                        DateTime.Now.Hour == end.Hour &&
                        DateTime.Now.Minute < end.Minute)
                    {
                        if (_lastSetState == true) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                        Properties.Settings.Default.Activated = true;
                        _lastSetState = true;
                    }
                    else if (Properties.Settings.Default.AutoDeactivate)
                    {
                        if (_lastSetState == false) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                        Properties.Settings.Default.Activated = false;
                        _lastSetState = false;
                    }
                }
                else if (Properties.Settings.Default.AutoDeactivate)
                {
                    if (_lastSetState == false) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                    Properties.Settings.Default.Activated = false;
                    _lastSetState = false;
                }
            }
            else
            {
                if (DateTime.Now.Hour > start.Hour ||
                    DateTime.Now.Hour == start.Hour &&
                    DateTime.Now.Minute >= start.Minute)
                {
                    if (_lastSetState == true) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                    Properties.Settings.Default.Activated = true;
                    _lastSetState = true;
                }
                else if (DateTime.Now.Hour < end.Hour ||
                          DateTime.Now.Hour == end.Hour &&
                          DateTime.Now.Minute < end.Minute)
                {
                    if (_lastSetState == true) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                    Properties.Settings.Default.Activated = true;
                    _lastSetState = true;
                }
                else if (Properties.Settings.Default.AutoDeactivate)
                {
                    if (_lastSetState == false) return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
                    Properties.Settings.Default.Activated = false;
                    _lastSetState = false;
                }
            }

            return _firstTimeIteration ? DateTime.Now.Second * 1000 - 1000 : 0;
        }

        public void CheckCsgoProcessLoop()
        {
            new Thread(async () =>
            {
                while (true)
                {
                    ConfigPage.ViewModel.CheckConfigFile();

                    var pname = Process.GetProcessesByName("csgo");

                    if (pname.Length > 0)
                    {
                        if (_globalLightsBackup == null &&
                            !string.IsNullOrEmpty(Properties.Settings.Default.AppKey) &&
                            Client != null)
                            _globalLightsBackup = (await Client.GetLightsAsync()).ToList();

                        if (WindowState != WindowState.Minimized
                            && Properties.Settings.Default.AutoMinimize
                            && !_alreadyMinimized
                            && !_previousState
                            && !Resetting)
                        {
                            _alreadyMinimized = true;

                            if (!_firstCsgoIteration)
                                WindowState = WindowState.Minimized;
                        }

                        _previousState = true;
                    }
                    else
                    {
                        _alreadyMinimized = false;

                        if (_previousState)
                            RestoreLights();

                        _previousState = false;
                    }

                    Resetting = false;
                    _firstCsgoIteration = false;

                    Thread.Sleep(Properties.Settings.Default.CsgoCheckingPeriod * 1000);
                }
            })
            { IsBackground = true }.Start();
        }

        #endregion

        #region LightsSetting

        public async Task SetLightsAsync(EventProperty config)
        {
            var i = 1;
            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                var color = l.Color;

                var command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                    Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                _alreadySetLights = false;
            }
        }

        public async Task SetLightsAsync(EventBrightnessProperty config, EventProperty main)
        {
            var bombblinklightsids = new List<string>();
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
                bombblinklightsids.Add(l.Id);

            var i = 1;
            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                if (_isPlanted && bombblinklightsids.Contains(l.Id)) continue;

                LightCommand command;
                Color color;
                if (!l.OnlyBrightness)
                {
                    color = l.Color;

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                color = main.Lights.Find(x => x.Id == l.Id).Color;

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                    Saturation = (byte)Math.Round(ColorConverters.GetSaturation(main.Lights.Find(x => x.Id == l.Id).Color) * 255),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                _alreadySetLights = false;
            }
        }

        public async Task SetLightsAsync(EventBrightnessProperty config, EventBrightnessProperty config2, EventProperty main)
        {
            var bombblinklightsids = new List<string>();
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
                bombblinklightsids.Add(l.Id);

            var i = 1;
            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                if (_isPlanted && bombblinklightsids.Contains(l.Id)) continue;

                LightCommand command;
                Color color;
                if (!l.OnlyBrightness)
                {
                    color = l.Color;

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                if (!config2.Lights.Find(x => x.Id == l.Id).OnlyBrightness)
                {
                    color = config2.Lights.Find(x => x.Id == l.Id).Color;

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(config2.Lights.Find(x => x.Id == l.Id).Color) * 255),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                color = main.Lights.Find(x => x.Id == l.Id).Color;

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                    Saturation = (byte)Math.Round(ColorConverters.GetSaturation(main.Lights.Find(x => x.Id == l.Id).Color) * 255),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> { $"{i++}" }).ConfigureAwait(false);

                _alreadySetLights = false;
            }
        }

        public async void RestoreLights()
        {
            if (_globalLightsBackup == null || !Properties.Settings.Default.RememberLightsStates ||
                _alreadySetLights) return;

            _alreadySetLights = true;
            for (var i = 0; i < _globalLightsBackup.Count; i++)
            {
                if (_globalLightsBackup.ElementAt(i).State.IsReachable != true) continue;

                var command = new LightCommand
                {
                    On = _globalLightsBackup.ElementAt(i).State.On,
                    Hue = _globalLightsBackup.ElementAt(i).State.Hue,
                    Saturation = _globalLightsBackup.ElementAt(i).State.Saturation,
                    Brightness = _globalLightsBackup.ElementAt(i).State.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> { $"{i + 1}" })
                    .ConfigureAwait(false);
            }

            _globalLightsBackup = null;
        }

        public async void BlinkingBombAsync()
        {
            if (!_isPlanted) return;

            var i = 1;
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
            {
                if (!Properties.Settings.Default.BombBlink.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                var color = l.Color;

                var back = Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color;

                LightCommand command;
                if (!Properties.Settings.Default.BombBlink.Lights.Find(x => x.Id == l.Id).OnlyBrightness)
                {
                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                        Brightness = l.Brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(100)
                    };
                }
                else
                {
                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(back) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color) * 255),
                        Brightness = l.Brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(100)
                    };
                }

                await Client.SendCommandAsync(command, new List<string> { $"{i}" }).ConfigureAwait(false);

                _alreadySetLights = false;

                Thread.Sleep(100);

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(ColorConverters.GetHue(back) / 360 * 65535),
                    Saturation = (byte)Math.Round(ColorConverters.GetSaturation(Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color) * 255),
                    Brightness = Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Brightness,
                    TransitionTime = TimeSpan.FromMilliseconds(100)
                };

                await Client.SendCommandAsync(command, new List<string> { $"{i}" }).ConfigureAwait(false);

                _alreadySetLights = false;

                i++;
            }
        }

        public bool BackLights(GameState gs)
        {
            switch (gs.Round.Bomb)
            {
                case BombState.Planted:
                    _isPlanted = true;
                    SetLightsAsync(Properties.Settings.Default.BombPlanted).Wait();
                    return true;
                case BombState.Exploded:
                    _isPlanted = false;
                    SetLightsAsync(Properties.Settings.Default.BombExplodes, Properties.Settings.Default.BombPlanted).Wait();
                    return true;
                default:
                    {
                        switch (gs.Round.WinTeam)
                        {
                            case RoundWinTeam.T:
                                _isPlanted = false;
                                SetLightsAsync(Properties.Settings.Default.TerroristsWin).Wait();
                                return true;
                            case RoundWinTeam.CT:
                                _isPlanted = false;
                                SetLightsAsync(Properties.Settings.Default.CounterTerroristsWin).Wait();
                                return true;
                            default:
                                {
                                    switch (gs.Round.Phase)
                                    {
                                        case RoundPhase.Live:
                                            _isPlanted = false;
                                            SetLightsAsync(Properties.Settings.Default.RoundStarts).Wait();
                                            return true;
                                        case RoundPhase.FreezeTime:
                                            _isPlanted = false;
                                            SetLightsAsync(Properties.Settings.Default.FreezeTime, Properties.Settings.Default.RoundStarts).Wait();
                                            return true;
                                    }

                                    return false;
                                }
                        }
                    }
            }
        }

        #endregion

        #region Events

        public void MainMenu()
        {
            SetLightsAsync(Properties.Settings.Default.MainMenu).Wait();
        }

        public void BombPlanted()
        {
            _isPlanted = true;
            SetLightsAsync(Properties.Settings.Default.BombPlanted).Wait();

            new Thread(() =>
            {
                for (var i = 0; i < 80 && _isPlanted; i++)
                {
                    if (_flashedZeroed)
                        BlinkingBombAsync();

                    Thread.Sleep(Convert.ToInt32(1000.0 - i * 21 + i * i * 0.14));
                }
            })
            { IsBackground = true }.Start();
        }

        public void BombExplodes()
        {
            if (!_flashedZeroed) return;

            SetLightsAsync(Properties.Settings.Default.BombExplodes, Properties.Settings.Default.BombPlanted).Wait();
        }

        public void RoundEnd(RoundWinTeam winner)
        {
            if (!_flashedZeroed) return;

            switch (winner)
            {
                case RoundWinTeam.T:
                    _isPlanted = false;
                    SetLightsAsync(Properties.Settings.Default.TerroristsWin).Wait();
                    break;
                case RoundWinTeam.CT:
                    _isPlanted = false;
                    SetLightsAsync(Properties.Settings.Default.CounterTerroristsWin).Wait();
                    break;
            }
        }

        public void RoundStarts()
        {
            SetLightsAsync(Properties.Settings.Default.RoundStarts).Wait();
        }

        public void RoundPhaseChanged(GameState current)
        {
            if (current.Round.Phase == RoundPhase.FreezeTime)
            {
                SetLightsAsync(Properties.Settings.Default.FreezeTime, Properties.Settings.Default.RoundStarts).Wait();
            }

            if (current.Map.Phase == MapPhase.Warmup)
            {
                SetLightsAsync(Properties.Settings.Default.Warmup, Properties.Settings.Default.RoundStarts).Wait();
            }
        }

        public void Kill(GameState gs)
        {
            _blockLightChange = true;

            switch (gs.Round.Bomb)
            {
                case BombState.Planted:
                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.BombPlanted).Wait();
                    break;
                case BombState.Exploded:
                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.BombExplodes, Properties.Settings.Default.BombPlanted).Wait();
                    break;
                default:
                    {
                        switch (gs.Round.WinTeam)
                        {
                            case RoundWinTeam.T:
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.TerroristsWin).Wait();
                                break;
                            case RoundWinTeam.CT:
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.CounterTerroristsWin).Wait();
                                break;
                            default:
                                if (gs.Round.Phase == RoundPhase.Live)
                                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.RoundStarts).Wait();
                                break;
                        }

                        break;
                    }
            }

            new Thread(() =>
            {
                Thread.Sleep((int)(Properties.Settings.Default.PlayerGetsKillDuration * 1000));

                BackLights(_lastgs);

                _blockLightChange = false;
            })
            { IsBackground = true }.Start();
        }

        public void Killed(GameState gs)
        {
            switch (gs.Round.Bomb)
            {
                case BombState.Planted:
                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.BombPlanted).Wait();
                    break;
                case BombState.Exploded:
                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.BombExplodes, Properties.Settings.Default.BombPlanted).Wait();
                    break;
                default:
                    {
                        switch (gs.Round.WinTeam)
                        {
                            case RoundWinTeam.T:
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.TerroristsWin).Wait();
                                break;
                            case RoundWinTeam.CT:
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.CounterTerroristsWin).Wait();
                                break;
                            default:
                                if (gs.Round.Phase == RoundPhase.Live)
                                    SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.RoundStarts).Wait();
                                break;
                        }

                        break;
                    }
            }

            new Thread(() =>
            {
                Thread.Sleep((int)(Properties.Settings.Default.PlayerGetsKilledDuration * 1000));

                BackLights(_lastgs);
            })
            { IsBackground = true }.Start();
        }

        #endregion

        #region Generic

        public void RunCsgo()
        {
            if (Process.GetProcessesByName("csgo").Length > 0) return;

            if (string.IsNullOrEmpty(Properties.Settings.Default.SteamFolder))
            {
                using (var key32 = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam"))
                using (var key64 = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
                {
                    object o = null;
                    if (key64 != null)
                        o = key64.GetValue("InstallPath");
                    else if (key32 != null)
                        o = key32.GetValue("InstallPath");

                    if (o != null)
                        Properties.Settings.Default.SteamFolder = o as string;
                }
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.SteamFolder)
                || !File.Exists(Properties.Settings.Default.SteamFolder + "\\Steam.exe")) return;

            Process.Start("steam://run/730//" + Properties.Settings.Default.LaunchOptions + "/");
        }

        #endregion

        #endregion

        #region Events Handlers

        public void OnNewGameState(GameState gs)
        {
            if (!Properties.Settings.Default.Activated
                || Client == null
                || _globalLightsBackup == null) return;

            _lastgs = gs;

            if (gs.Map.Name == "" && _mainMenuState)
            {
                _mainMenuState = false;
                _isPlanted = false;
                _flashedZeroed = true;
                _killAmount = 60000;
                _deathAmount = 60000;
                MainMenu();
                return;
            }

            if (gs.Map.Name != "" && _mainMenuState == false)
            {
                _mainMenuState = true;

                switch (gs.Round.Bomb)
                {
                    case BombState.Planted:
                        BombPlanted();
                        break;
                    case BombState.Exploded:
                        BombExplodes();
                        break;
                    default:
                        {
                            switch (gs.Round.Phase)
                            {
                                case RoundPhase.Over:
                                    RoundEnd(gs.Round.WinTeam);
                                    break;
                                case RoundPhase.Live:
                                    RoundStarts();
                                    break;
                                default:
                                    RoundPhaseChanged(gs);
                                    break;
                            }

                            break;
                        }
                }
            }

            if ((gs.Provider.SteamID == gs.Player.SteamID ||
                Properties.Settings.Default.TriggerSpecEvents) &&
                gs.Map.Phase != MapPhase.Warmup)
            {
                if (gs.Previously.Player.SteamID != "")
                {
                    _killAmount = gs.Player.MatchStats.Kills;
                    _deathAmount = gs.Player.MatchStats.Deaths;
                    _flashedZeroed = true;
                }

                if (gs.Player.MatchStats.Kills > _killAmount &&
                !gs.Added.JSON.Contains("match_stats"))
                {
                    _killAmount = gs.Player.MatchStats.Kills;
                    if (!_blockLightChange)
                        Kill(gs);
                }
                else if ((gs.Player.MatchStats.Kills < _killAmount ||
                    gs.Player.MatchStats.Kills == 60000) &&
                    !gs.Added.JSON.Contains("match_stats") &&
                    gs.Player.JSON.Contains("match_stats"))
                {
                    _killAmount = gs.Player.MatchStats.Kills;
                }

                if (gs.Player.MatchStats.Deaths > _deathAmount)
                {
                    _deathAmount = gs.Player.MatchStats.Deaths;
                    Killed(gs);
                }
                else if ((gs.Player.MatchStats.Deaths < _deathAmount ||
                    gs.Player.MatchStats.Deaths == 60000) &&
                    !gs.Added.JSON.Contains("match_stats") &&
                    gs.Player.JSON.Contains("match_stats"))
                {
                    _deathAmount = gs.Player.MatchStats.Deaths;
                }

                if (gs.Player.State.Flashed > gs.Previously.Player.State.Flashed &&
                    gs.Player.State.Flashed - gs.Previously.Player.State.Flashed > 32 &&
                    gs.Previously.Player.State.Flashed >= 0)
                {
                    _flashedZeroed = false;
                    SetLightsAsync(Properties.Settings.Default.PlayerGetsFlashed).Wait();
                }
                else if (gs.Player.State.Flashed < gs.Previously.Player.State.Flashed && !_flashedZeroed)
                {
                    _flashedZeroed = true;
                    BackLights(gs);
                }
            }

            if (_blockLightChange) return;

            if (gs.Previously.Round.Phase != RoundPhase.Undefined &&
                gs.Round.Phase != RoundPhase.Undefined &&
                gs.Previously.Round.Phase != gs.Round.Phase)
            {
                RoundPhaseChanged(gs);
            }

            switch (gs.Previously.Round.Phase)
            {
                case RoundPhase.Live when gs.Round.Phase == RoundPhase.Over:
                    _isPlanted = false;
                    RoundEnd(gs.Round.WinTeam);
                    break;
                case RoundPhase.FreezeTime when gs.Round.Phase == RoundPhase.Live:
                    _isPlanted = false;
                    RoundStarts();
                    break;
            }

            if (gs.Previously.Round.Bomb == BombState.Planted &&
                gs.Round.Bomb == BombState.Exploded)
            {
                _isPlanted = false;
                BombExplodes();
            }

            if (!_isPlanted &&
                gs.Round.Phase == RoundPhase.Live &&
                gs.Round.Bomb == BombState.Planted &&
                gs.Previously.Round.Bomb == BombState.Undefined)
            {
                BombPlanted();
            }
        }

        #endregion
    }
}

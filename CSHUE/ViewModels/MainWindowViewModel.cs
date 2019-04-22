using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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

namespace CSHUE.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public Config ConfigPage;
        public Donate DonatePage;
        public About AboutPage;
        public Home HomePage;
        public Settings SettingsPage;

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

        public void CreateInstances()
        {
            ConfigPage = new Config();
            DonatePage = new Donate();
            HomePage = new Home();
            SettingsPage = new Settings();
            AboutPage = new About();

            var list = new List<BridgeSelector>()
            {
                new BridgeSelector(),
                new BridgeSelector(),
                new BridgeSelector()
            };

            new Selector()
            {
                Ok = "aaa",
                List = list
            }.ShowDialog();
        }

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
        private string _appKey;
        private static ILocalHueClient Client { get; set; }
        private List<Light> _globalLightsBackup;
        private bool _alreadySetLights;
        private bool _alreadyMinimized;

        public async void HueAsync()
        {
            HomePage.ViewModel.LoadingVisibility = Visibility.Visible;

            if (_bridgeIp == "")
                _bridgeIp = await GetBridgeIpAsync().ConfigureAwait(false);

            if (_bridgeIp == "-1") return;

            Client = new LocalHueClient(_bridgeIp);
            
            await GetAppKeyAsync();

            HomePage.ViewModel.LoadingVisibility = Visibility.Hidden;
        }

        public async Task<string> GetBridgeIpAsync()
        {
            var locator = new HttpBridgeLocator();

            List<Q42.HueApi.Models.Bridge.LocatedBridge> bridgeIPs;

            try
            {
                bridgeIPs = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false)).ToList();
            }
            catch
            {
                bridgeIPs = null;
            }


            if (bridgeIPs == null || bridgeIPs.Count < 1)
            {
                HomePage.ViewModel.WarningNoHub = Visibility.Visible;
                HomePage.ViewModel.WarningLink = Visibility.Collapsed;

                while (bridgeIPs == null || bridgeIPs.Count < 1)
                {
                    try
                    {
                        bridgeIPs = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false)).ToList();
                    }
                    catch
                    {
                        bridgeIPs = null;
                    }

                    Thread.Sleep(1000);
                }

                return bridgeIPs.First().IpAddress;
            }

            return bridgeIPs.Count > 1 ? "-1" : bridgeIPs.First().IpAddress;
        }

        public async Task GetAppKeyAsync()
        {
            if (!File.Exists("appKey"))
            {
                var key = "";

                HomePage.ViewModel.WarningNoHub = Visibility.Collapsed;
                HomePage.ViewModel.WarningLink = Visibility.Visible;

                while (key == "")
                {
                    try
                    {
                        key = await Client.RegisterAsync("CSHUE", "Desktop").ConfigureAwait(false);
                    }
                    catch
                    {
                        key = "";
                    }

                    Thread.Sleep(1000);
                }

                _appKey = key;

                File.WriteAllText("appKey", _appKey);
            }
            else
                _appKey = File.ReadAllText("appKey");

            Client.Initialize(_appKey);

            await SetDefaultLightsSettings();

            Csgo();
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

        public void CheckTimeLoop()
        {
            new Thread(() =>
            {
                while (true)
                {
                    CheckTime();

                    Thread.Sleep(60000);
                }
            }) {IsBackground = true}.Start();
        }

        public void CheckTime()
        {
            if (Properties.Settings.Default.AutoActivate)
            {
                var start = DateTime.ParseExact(Properties.Settings.Default.AutoActivateStart, "HH:mm", CultureInfo.InvariantCulture);
                var end = DateTime.ParseExact(Properties.Settings.Default.AutoActivateEnd, "HH:mm", CultureInfo.InvariantCulture);

                if (end.Hour > start.Hour || (end.Hour == start.Hour && end.Minute > start.Minute))
                {
                    if (DateTime.Now.Hour > start.Hour || (DateTime.Now.Hour == start.Hour && DateTime.Now.Minute >= start.Minute))
                    {
                        if (DateTime.Now.Hour < end.Hour || (DateTime.Now.Hour == end.Hour && DateTime.Now.Minute < end.Minute))
                        {
                            if (Properties.Settings.Default.Activated == false)
                                Properties.Settings.Default.Activated = true;
                        }
                        else
                        {
                            if (Properties.Settings.Default.Activated)
                                Properties.Settings.Default.Activated = false;
                        }
                    }
                    else
                    {
                        if (Properties.Settings.Default.Activated)
                            Properties.Settings.Default.Activated = false;
                    }
                }
                else
                {
                    if (DateTime.Now.Hour > start.Hour || (DateTime.Now.Hour == start.Hour && DateTime.Now.Minute >= start.Minute))
                    {
                        if (Properties.Settings.Default.Activated == false)
                            Properties.Settings.Default.Activated = true;
                    }
                    else if (DateTime.Now.Hour < end.Hour || (DateTime.Now.Hour == end.Hour && DateTime.Now.Minute < end.Minute))
                    {
                        if (Properties.Settings.Default.Activated == false)
                            Properties.Settings.Default.Activated = true;
                    }
                    else
                    {
                        if (Properties.Settings.Default.Activated)
                            Properties.Settings.Default.Activated = false;
                    }
                }
            }
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
                        if (_globalLightsBackup == null)
                            _globalLightsBackup = (await Client.GetLightsAsync()).ToList();

                        if (WindowState != WindowState.Minimized
                            && Properties.Settings.Default.AutoMinimize
                            && !_alreadyMinimized)
                        {
                            _alreadyMinimized = true;
                            WindowState = WindowState.Minimized;
                        }
                    }
                    else
                    {
                        if (!_alreadyMinimized)
                            _alreadyMinimized = false;

                        if (_globalLightsBackup != null
                            && Properties.Settings.Default.RememberLightsStates
                            && !_alreadySetLights)
                        {
                            _alreadySetLights = true;

                            for (var i = 0; i < _globalLightsBackup.Count; i++)
                            {
                                if (_globalLightsBackup.ElementAt(i).State.IsReachable == true)
                                {
                                    var command = new LightCommand
                                    {
                                        On = _globalLightsBackup.ElementAt(i).State.On,
                                        Hue = _globalLightsBackup.ElementAt(i).State.Hue,
                                        Saturation = _globalLightsBackup.ElementAt(i).State.Saturation,
                                        Brightness = _globalLightsBackup.ElementAt(i).State.Brightness
                                    };

                                    await Client.SendCommandAsync(command, new List<string> {$"{i + 1}"})
                                        .ConfigureAwait(false);
                                }
                            }
                        }

                        _globalLightsBackup = null;
                    }

                    Thread.Sleep(Properties.Settings.Default.CsgoCheckingPeriod * 1000);
                }
            }) { IsBackground = true }.Start();
        }

        public void RunCsgo()
        {
            if (Process.GetProcessesByName("csgo").Length > 0) return;

            var steampath = "";

            using (var key32 = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam"))
            using (var key64 = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
            {
                object o = null;
                if (key64 != null)
                    o = key64.GetValue("InstallPath");
                else if (key32 != null)
                    o = key32.GetValue("InstallPath");

                if (o != null)
                    steampath = o as string;
            }

            steampath += "\\Steam.exe";

            var p = new Process
            {
                StartInfo =
                {
                    FileName = steampath,
                    Arguments = "steam://run/730/" + "/" + Properties.Settings.Default.LaunchOptions + "/",
                    UseShellExecute = false
                }
            };
            p.Start();
        }

        private GameState _lastgs;

        private bool _blockLightChange;
        private bool _isPlanted;
        private bool _flashedZeroed = true;
        private int _killAmount = 60000;
        private int _deathAmount = 60000;
        private bool _mainMenuState = true;

        public void OnNewGameState(GameState gs)
        {
            if (!Properties.Settings.Default.Activated) return;

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
            else if (gs.Map.Name != "" && _mainMenuState == false)
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
                    case BombState.Undefined:
                        break;
                    case BombState.Dropped:
                        break;
                    case BombState.Carried:
                        break;
                    case BombState.Planting:
                        break;
                    case BombState.Defusing:
                        break;
                    case BombState.Defused:
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
                            case RoundPhase.Undefined:
                                break;
                            case RoundPhase.FreezeTime:
                                break;
                            default:
                                RoundPhaseChanged(gs);
                                break;
                        }

                        break;
                    }
                }
            }

            if (gs.Provider.SteamID == gs.Player.SteamID ||
                Properties.Settings.Default.TriggerSpecEvents)
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

        public async Task SetLightsAsync(EventProperty config)
        {
            var i = 1;
            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                var color = Color.FromArgb(l.Color.Red, l.Color.Green, l.Color.Blue);

                var command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                    Saturation = GetSaturation(l.Color.Red,
                                                l.Color.Green,
                                                l.Color.Blue),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

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
                    color = Color.FromArgb(l.Color.Red, l.Color.Green, l.Color.Blue);

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                        Saturation = GetSaturation(l.Color.Red,
                                                l.Color.Green,
                                                l.Color.Blue),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                color = Color.FromArgb(main.Lights.Find(x => x.Id == l.Id).Color.Red,
                    main.Lights.Find(x => x.Id == l.Id).Color.Green,
                    main.Lights.Find(x => x.Id == l.Id).Color.Blue);

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                    Saturation = GetSaturation(main.Lights.Find(x => x.Id == l.Id).Color.Red,
                                                main.Lights.Find(x => x.Id == l.Id).Color.Green,
                                                main.Lights.Find(x => x.Id == l.Id).Color.Blue),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

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
                    color = Color.FromArgb(l.Color.Red, l.Color.Green, l.Color.Blue);

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                        Saturation = GetSaturation(l.Color.Red,
                                                l.Color.Green,
                                                l.Color.Blue),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                if (!config2.Lights.Find(x => x.Id == l.Id).OnlyBrightness)
                {
                    color = Color.FromArgb(config2.Lights.Find(x => x.Id == l.Id).Color.Red,
                        config2.Lights.Find(x => x.Id == l.Id).Color.Green,
                        config2.Lights.Find(x => x.Id == l.Id).Color.Blue);

                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                        Saturation = GetSaturation(config2.Lights.Find(x => x.Id == l.Id).Color.Red,
                                                config2.Lights.Find(x => x.Id == l.Id).Color.Green,
                                                config2.Lights.Find(x => x.Id == l.Id).Color.Blue),
                        Brightness = l.Brightness
                    };

                    await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

                    _alreadySetLights = false;
                    continue;
                }

                color = Color.FromArgb(main.Lights.Find(x => x.Id == l.Id).Color.Red,
                    main.Lights.Find(x => x.Id == l.Id).Color.Green,
                    main.Lights.Find(x => x.Id == l.Id).Color.Blue);

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                    Saturation = GetSaturation(main.Lights.Find(x => x.Id == l.Id).Color.Red,
                                                main.Lights.Find(x => x.Id == l.Id).Color.Green,
                                                main.Lights.Find(x => x.Id == l.Id).Color.Blue),
                    Brightness = l.Brightness
                };

                await Client.SendCommandAsync(command, new List<string> {$"{i++}"}).ConfigureAwait(false);

                _alreadySetLights = false;
            }
        }

        public void MainMenu()
        {
            SetLightsAsync(Properties.Settings.Default.MainMenu).Wait();
        }

        public void BombPlanted()
        {
            _isPlanted = true;
            SetLightsAsync(Properties.Settings.Default.BombPlanted).Wait();

            Task.Run(async () =>
            {
                for (var i = 0; i < 80 && _isPlanted; i++)
                {
                    if (_flashedZeroed)
                        BlinkingBombAsync();

                    await Task.Delay(Convert.ToInt32(1000.0 - (i * 21) + (i * i * 0.14)));
                }
            });
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
                case RoundWinTeam.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(winner), winner, null);
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
                case BombState.Undefined:
                    break;
                case BombState.Dropped:
                    break;
                case BombState.Carried:
                    break;
                case BombState.Planting:
                    break;
                case BombState.Defusing:
                    break;
                case BombState.Defused:
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
                        case RoundWinTeam.Undefined:
                            break;
                        default:
                            if (gs.Round.Phase == RoundPhase.Live)
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKill, Properties.Settings.Default.RoundStarts).Wait();
                            break;
                    }

                    break;
                }
            }

            Task.Run(async () =>
            {
                await Task.Delay((int)(Properties.Settings.Default.PlayerGetsKillDuration * 1000));

                BackLights(_lastgs);

                _blockLightChange = false;
            });
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
                case BombState.Undefined:
                    break;
                case BombState.Dropped:
                    break;
                case BombState.Carried:
                    break;
                case BombState.Planting:
                    break;
                case BombState.Defusing:
                    break;
                case BombState.Defused:
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
                        case RoundWinTeam.Undefined:
                            break;
                        default:
                            if (gs.Round.Phase == RoundPhase.Live)
                                SetLightsAsync(Properties.Settings.Default.PlayerGetsKilled, Properties.Settings.Default.RoundStarts).Wait();
                            break;
                    }

                    break;
                }
            }

            Task.Run(async () =>
            {
                await Task.Delay((int)(Properties.Settings.Default.PlayerGetsKilledDuration * 1000));

                BackLights(_lastgs);
            });
        }

        public async void BlinkingBombAsync()
        {
            if (!_isPlanted) return;

            var i = 1;
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
            {
                if (!Properties.Settings.Default.BombBlink.SelectedLights.FindAll(x => x == l.Id).Any()) continue;

                var color = Color.FromArgb(l.Color.Red,
                                                l.Color.Green,
                                                l.Color.Blue);

                var back = Color.FromArgb(Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Red,
                    Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Green,
                    Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Blue);

                LightCommand command;
                if (!Properties.Settings.Default.BombBlink.Lights.Find(x => x.Id == l.Id).OnlyBrightness)
                {
                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(color.GetHue() / 360 * 65535),
                        Saturation = GetSaturation(l.Color.Red,
                                                l.Color.Green,
                                                l.Color.Blue),
                        Brightness = l.Brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(100)
                    };
                }
                else
                {
                    command = new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(back.GetHue() / 360 * 65535),
                        Saturation = GetSaturation(Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Red,
                            Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Green,
                            Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Blue),
                        Brightness = l.Brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(100)
                    };
                }

                await Client.SendCommandAsync(command, new List<string> {$"{i}"}).ConfigureAwait(false);

                _alreadySetLights = false;

                Thread.Sleep(100);

                command = new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(back.GetHue() / 360 * 65535),
                    Saturation = GetSaturation(Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Red,
                        Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Green,
                        Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Color.Blue),
                    Brightness = Properties.Settings.Default.BombPlanted.Lights.Find(x => x.Id == l.Id).Brightness,
                    TransitionTime = TimeSpan.FromMilliseconds(100)
                };

                await Client.SendCommandAsync(command, new List<string> {$"{i}"}).ConfigureAwait(false);

                _alreadySetLights = false;

                i++;
            }
        }

        private static int GetSaturation(int red, int green, int blue)
        {
            var r = (double)red / 255;
            var g = (double)green / 255;
            var b = (double)blue / 255;

            var max = r;

            if (g > max) max = g;
            if (b > max) max = b;

            var min = r;

            if (g < min) min = g;
            if (b < min) min = b;

            var delta = max - min;

            return (int)Math.Round(delta / max * 255);
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
                case BombState.Undefined:
                    break;
                case BombState.Dropped:
                    break;
                case BombState.Carried:
                    break;
                case BombState.Planting:
                    break;
                case BombState.Defusing:
                    break;
                case BombState.Defused:
                    break;
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
                        case RoundWinTeam.Undefined:
                            break;
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
                                case RoundPhase.Undefined:
                                    break;
                                case RoundPhase.Over:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            if (gs.Map.Phase != MapPhase.Warmup) return false;

            _isPlanted = false;
            SetLightsAsync(Properties.Settings.Default.Warmup, Properties.Settings.Default.RoundStarts).Wait();

            return true;
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

            if (Properties.Settings.Default.MainMenu.Lights == null)
            {
                Properties.Settings.Default.MainMenu.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.MainMenu.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(0, 0, 255),
                        Brightness = 192
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
                    Properties.Settings.Default.PlayerGetsKill.Lights.Add(new UniqueBrightnessLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(0, 255, 0),
                        Brightness = 255,
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
                    Properties.Settings.Default.PlayerGetsKilled.Lights.Add(new UniqueBrightnessLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 0, 0),
                        Brightness = 128,
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
                    Properties.Settings.Default.PlayerGetsFlashed.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 255, 255),
                        Brightness = 255
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
                    Properties.Settings.Default.TerroristsWin.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 200, 0),
                        Brightness = 192
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
                    Properties.Settings.Default.CounterTerroristsWin.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(0, 0, 255),
                        Brightness = 192
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
                    Properties.Settings.Default.RoundStarts.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(0, 255, 0),
                        Brightness = 192
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
                    Properties.Settings.Default.FreezeTime.Lights.Add(new UniqueBrightnessLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 255, 255),
                        Brightness = 128,
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
                    Properties.Settings.Default.Warmup.Lights.Add(new UniqueBrightnessLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 255, 255),
                        Brightness = 128,
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
                    Properties.Settings.Default.BombExplodes.Lights.Add(new UniqueBrightnessLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 0, 0),
                        Brightness = 255,
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
                    Properties.Settings.Default.BombPlanted.Lights.Add(new UniqueLight()
                    {
                        Id = i.UniqueId,
                        Color = Color.FromArgb(255, 0, 0),
                        Brightness = 128
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
                    new UniqueBrightnessLight()
                    {
                        Id = allLights.First().UniqueId,
                        Color = Color.FromArgb(255, 0, 0),
                        Brightness = 255,
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
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Media;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CSGSI;
using CSGSI.Nodes;
using CSHUE.Core;
using CSHUE.Core.Utilities;
using CSHUE.Views;
using Microsoft.Win32;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace CSHUE.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        public bool WindowMinimized;
        public bool AllowStartLightsChecking;
        public bool? IsModeDark;
        public bool? IsTransparencyTrue;

        public Config Config;
        public Donate Donate;
        public About About;
        public Help Help;
        public Update Update;
        public Home Home;
        public Settings Settings;

        private GameStateListener _gameStateListener;
        public List<Light> GlobalLightsBackup;
        private bool _alreadySetLights;
        private bool _alreadyMinimized;
        private bool _previousState;
        private bool _firstCsgoIteration = true;
        private bool _firstCheckIteration = true;
        private string _bridgeIp = "";

        private GameState _lastgs;
        private bool _blockLightChange;
        private bool _isPlanted;
        private bool _flashedZeroed = true;
        private int _killAmount = 60000;
        private int _deathAmount = 60000;
        private bool _mainMenuState = true;
        private bool _lastActivatedState;

        private System.Timers.Timer _csgoTimer;
        private System.Timers.Timer _initTimer;
        private System.Timers.Timer _checkTimer;

        #endregion

        #region Properties

        private Geometry _maximizeRestore = Geometry.Parse((string)Application.Current.Resources["Maximize"]);
        public Geometry MaximizeRestore
        {
            get => _maximizeRestore;
            set
            {
                _maximizeRestore = value;
                OnPropertyChanged();
            }
        }

        private WindowState _windowState = WindowState.Normal;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged();
            }
        }

        public static ILocalHueClient Client { get; set; }

        private Visibility _notifyIconVisibility = Visibility.Collapsed;
        public Visibility NotifyIconVisibility
        {
            get => _notifyIconVisibility;
            set
            {
                _notifyIconVisibility = value;
                OnPropertyChanged();
            }
        }

        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        private object _content;
        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Initializers

        public void CreateInstances()
        {
            Config = new Config();
            Donate = new Donate();
            Home = new Home();
            Settings = new Settings();
            About = new About();
            Help = new Help();
            Update = new Update();
        }

        public static async Task SetDefaultLightsSettings()
        {
            if (Client == null)
                return;

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

            if (Properties.Settings.Default.UniqueIds != null)
            {
                var uniqueIds = new StringCollection();
                foreach (var l in allLights)
                    uniqueIds.Add(l.UniqueId);

                foreach (var u in Properties.Settings.Default.UniqueIds)
                    if (!uniqueIds.Contains(u))
                    {
                        Properties.Settings.Default.MainMenu.Lights.Remove(
                            Properties.Settings.Default.MainMenu.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.PlayerGetsKill.Lights.Remove(
                            Properties.Settings.Default.PlayerGetsKill.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.PlayerGetsKilled.Lights.Remove(
                            Properties.Settings.Default.PlayerGetsKilled.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.PlayerGetsFlashed.Lights.Remove(
                            Properties.Settings.Default.PlayerGetsFlashed.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.TerroristsWin.Lights.Remove(
                            Properties.Settings.Default.TerroristsWin.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.CounterTerroristsWin.Lights.Remove(
                            Properties.Settings.Default.CounterTerroristsWin.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.RoundStarts.Lights.Remove(
                            Properties.Settings.Default.RoundStarts.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.FreezeTime.Lights.Remove(
                            Properties.Settings.Default.FreezeTime.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.Warmup.Lights.Remove(
                            Properties.Settings.Default.Warmup.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.BombExplodes.Lights.Remove(
                            Properties.Settings.Default.BombExplodes.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.BombPlanted.Lights.Remove(
                            Properties.Settings.Default.BombPlanted.Lights.Find(x => x.UniqueId == u));
                        Properties.Settings.Default.BombBlink.Lights.Remove(
                            Properties.Settings.Default.BombBlink.Lights.Find(x => x.UniqueId == u));

                        Properties.Settings.Default.MainMenu.SelectedLights.Remove(u);
                        Properties.Settings.Default.PlayerGetsKill.SelectedLights.Remove(u);
                        Properties.Settings.Default.PlayerGetsKilled.SelectedLights.Remove(u);
                        Properties.Settings.Default.PlayerGetsFlashed.SelectedLights.Remove(u);
                        Properties.Settings.Default.TerroristsWin.SelectedLights.Remove(u);
                        Properties.Settings.Default.CounterTerroristsWin.SelectedLights.Remove(u);
                        Properties.Settings.Default.RoundStarts.SelectedLights.Remove(u);
                        Properties.Settings.Default.FreezeTime.SelectedLights.Remove(u);
                        Properties.Settings.Default.Warmup.SelectedLights.Remove(u);
                        Properties.Settings.Default.BombExplodes.SelectedLights.Remove(u);
                        Properties.Settings.Default.BombPlanted.SelectedLights.Remove(u);
                        Properties.Settings.Default.BombBlink.SelectedLights.Remove(u);

                        Properties.Settings.Default.UniqueIds.Remove(u);
                    }
            }

            if (Properties.Settings.Default.MainMenu.Lights == null)
            {
                Properties.Settings.Default.MainMenu.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.MainMenu.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(0, 0, 255),
                        Brightness = 192
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.MainMenu.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(0, 0, 255),
                            Brightness = 192
                        });
            }
            if (Properties.Settings.Default.MainMenu.SelectedLights == null)
            {
                Properties.Settings.Default.MainMenu.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.PlayerGetsKill.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsKill.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKill.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(0, 255, 0),
                        Brightness = 255,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.PlayerGetsKill.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(0, 255, 0),
                            Brightness = 255,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.PlayerGetsKill.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsKill.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.PlayerGetsKilled.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsKilled.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsKilled.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 2000,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 137, 14)
                            : Color.FromRgb(255, 0, 0),
                        Brightness = 128,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.PlayerGetsKilled.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 2000,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 137, 14)
                                : Color.FromRgb(255, 0, 0),
                            Brightness = 128,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.PlayerGetsKilled.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsKilled.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.PlayerGetsFlashed.Lights == null)
            {
                Properties.Settings.Default.PlayerGetsFlashed.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.PlayerGetsFlashed.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(255, 255, 255),
                        Brightness = 255
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.PlayerGetsFlashed.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(255, 255, 255),
                            Brightness = 255
                        });
            }
            if (Properties.Settings.Default.PlayerGetsFlashed.SelectedLights == null)
            {
                Properties.Settings.Default.PlayerGetsFlashed.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.TerroristsWin.Lights == null)
            {
                Properties.Settings.Default.TerroristsWin.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.TerroristsWin.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 4250,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 137, 14)
                            : Color.FromRgb(255, 170, 0),
                        Brightness = 192
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.TerroristsWin.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 4250,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 137, 14)
                                : Color.FromRgb(255, 170, 0),
                            Brightness = 192
                        });
            }
            if (Properties.Settings.Default.TerroristsWin.SelectedLights == null)
            {
                Properties.Settings.Default.TerroristsWin.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.CounterTerroristsWin.Lights == null)
            {
                Properties.Settings.Default.CounterTerroristsWin.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.CounterTerroristsWin.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(0, 0, 255),
                        Brightness = 192
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.CounterTerroristsWin.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(0, 0, 255),
                            Brightness = 192
                        });
            }
            if (Properties.Settings.Default.CounterTerroristsWin.SelectedLights == null)
            {
                Properties.Settings.Default.CounterTerroristsWin.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.RoundStarts.Lights == null)
            {
                Properties.Settings.Default.RoundStarts.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.RoundStarts.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(0, 255, 0),
                        Brightness = 192
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.RoundStarts.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(0, 255, 0),
                            Brightness = 192
                        });
            }
            if (Properties.Settings.Default.RoundStarts.SelectedLights == null)
            {
                Properties.Settings.Default.RoundStarts.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.FreezeTime.Lights == null)
            {
                Properties.Settings.Default.FreezeTime.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.FreezeTime.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(255, 255, 255),
                        Brightness = 128,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.FreezeTime.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(255, 255, 255),
                            Brightness = 128,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.FreezeTime.SelectedLights == null)
            {
                Properties.Settings.Default.FreezeTime.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.Warmup.Lights == null)
            {
                Properties.Settings.Default.Warmup.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.Warmup.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 6500,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 254, 250)
                            : Color.FromRgb(255, 255, 255),
                        Brightness = 128,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.Warmup.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 6500,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 254, 250)
                                : Color.FromRgb(255, 255, 255),
                            Brightness = 128,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.Warmup.SelectedLights == null)
            {
                Properties.Settings.Default.Warmup.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.BombExplodes.Lights == null)
            {
                Properties.Settings.Default.BombExplodes.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombExplodes.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 2000,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 137, 14)
                            : Color.FromRgb(255, 0, 0),
                        Brightness = 255,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.BombExplodes.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 2000,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 137, 14)
                                : Color.FromRgb(255, 0, 0),
                            Brightness = 255,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.BombExplodes.SelectedLights == null)
            {
                Properties.Settings.Default.BombExplodes.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.BombPlanted.Lights == null)
            {
                Properties.Settings.Default.BombPlanted.Lights = new List<UniqueLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombPlanted.Lights.Add(new UniqueLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 2000,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 137, 14)
                            : Color.FromRgb(255, 0, 0),
                        Brightness = 128
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.BombPlanted.Lights.Add(new UniqueLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 2000,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 137, 14)
                                : Color.FromRgb(255, 0, 0),
                            Brightness = 128
                        });
            }
            if (Properties.Settings.Default.BombPlanted.SelectedLights == null)
            {
                Properties.Settings.Default.BombPlanted.SelectedLights = new List<string>();
            }

            if (Properties.Settings.Default.BombBlink.Lights == null)
            {
                Properties.Settings.Default.BombBlink.Lights = new List<UniqueBrightnessLight>();
                foreach (var i in allLights)
                    Properties.Settings.Default.BombBlink.Lights.Add(new UniqueBrightnessLight
                    {
                        UniqueId = i.UniqueId,
                        Id = i.Id,
                        IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                        ColorTemperature = 2000,
                        Color = i.Capabilities.Control.ColorGamut == null
                            ? Color.FromRgb(255, 137, 14)
                            : Color.FromRgb(255, 0, 0),
                        Brightness = 255,
                        OnlyBrightness = true
                    });
            }
            else if (Properties.Settings.Default.UniqueIds != null)
            {
                foreach (var i in allLights)
                    if (!Properties.Settings.Default.UniqueIds.Contains(i.UniqueId))
                        Properties.Settings.Default.BombBlink.Lights.Add(new UniqueBrightnessLight
                        {
                            UniqueId = i.UniqueId,
                            Id = i.Id,
                            IsColorTemperature = i.Capabilities.Control.ColorGamut == null,
                            ColorTemperature = 2000,
                            Color = i.Capabilities.Control.ColorGamut == null
                                ? Color.FromRgb(255, 137, 14)
                                : Color.FromRgb(255, 0, 0),
                            Brightness = 255,
                            OnlyBrightness = true
                        });
            }
            if (Properties.Settings.Default.BombBlink.SelectedLights == null)
            {
                Properties.Settings.Default.BombBlink.SelectedLights = new List<string>();
            }

            Properties.Settings.Default.UniqueIds = new StringCollection();
            foreach (var l in allLights)
            {
                Properties.Settings.Default.UniqueIds.Add(l.UniqueId);
            }
        }

        #endregion

        #region Methods

        #region Connection

        public async void HueAsync(bool cleanSearching)
        {
            AllowStartLightsChecking = false;

            _bridgeIp = await GetBridgeIpAsync(cleanSearching).ConfigureAwait(false);

            if (_bridgeIp == "")
                return;

            Client = new LocalHueClient(_bridgeIp);

            await GetAppKeyAsync();
            while (Properties.Settings.Default.AppKey == "")
            {
                await GetAppKeyAsync();
            }

            Settings.ViewModel.UpdateGradients();

            Home.ViewModel.SetDone();

            Home.StartLightsChecking();

            AllowStartLightsChecking = true;
        }

        public async Task<string> GetBridgeIpAsync(bool cleanSearching)
        {
            var locator = new HttpBridgeLocator();

            List<LocatedBridge> bridges;

            try
            {
                Home.ViewModel.SetWarningSearching();

                bridges = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5.5)).ConfigureAwait(false)).ToList();
            }
            catch
            {
                bridges = null;
            }

            if (bridges == null || bridges.Count < 1)
            {
                Home.ViewModel.SetWarningNoBridge();

                return "";
            }

            string selectedBridgeIp;
            string selectedBridgeId;

            if (!cleanSearching &&
                Properties.Settings.Default.BridgeId != null &&
                bridges.Any(x => x.BridgeId == Properties.Settings.Default.BridgeId))
            {
                selectedBridgeIp = bridges.Find(x => x.BridgeId == Properties.Settings.Default.BridgeId).IpAddress;
                selectedBridgeId = bridges.Find(x => x.BridgeId == Properties.Settings.Default.BridgeId).BridgeId;
            }
            else if (bridges.Count == 1)
            {
                selectedBridgeIp = bridges.First().IpAddress;
                selectedBridgeId = bridges.First().BridgeId;
            }
            else
            {
                var list = new List<BridgeInfoCellViewModel>();

                foreach (var b in bridges)
                {
                    try
                    {
                        list.Add(new BridgeInfoCellViewModel
                        {
                            Text = $"ip: {b.IpAddress}, id: {b.BridgeId}",
                            Ip = b.IpAddress,
                            Id = b.BridgeId
                        });
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (list.Count < 1)
                {
                    Home.ViewModel.SetWarningNoReachableBridges();

                    return "";
                }

                if (list.Count == 1)
                {
                    return list.ElementAt(0).Ip;
                }

                var bridge = Application.Current.Dispatcher?.Invoke(() =>
                {
                    var bridgeSelector = new BridgeSelector(list)
                    {
                        Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
                    };
                    bridgeSelector.ShowDialog();

                    return bridgeSelector;
                });

                selectedBridgeIp = bridge?.SelectedBridge.IpAddress;
                selectedBridgeId = bridge?.SelectedBridge.BridgeId;
            }

            Home.ViewModel.SetWarningValidating();

            try
            {
                var request = WebRequest.Create($"http://{selectedBridgeIp}/debug/clip.html");
                request.Timeout = 1000;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (selectedBridgeId != Properties.Settings.Default.BridgeId)
                            Properties.Settings.Default.AppKey = "";

                        Properties.Settings.Default.BridgeId = selectedBridgeId;
                        return selectedBridgeIp;
                    }
                }
            }
            catch
            {
                Home.ViewModel.SetWarningBridgeNotAvailable();
            }

            return "";
        }

        public async Task GetAppKeyAsync()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.AppKey))
            {
                Home.ViewModel.SetWarningLink();

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
                // ignored
            }

            Properties.Settings.Default.Save();
        }

        public void Csgo()
        {
            _gameStateListener = new GameStateListener(int.Parse(Properties.Settings.Default.Port));
            _gameStateListener.NewGameState += OnNewGameState;
            _gameStateListener.Start();

            if (Properties.Settings.Default.RunCsgo)
                RunCsgo();

            Config.ViewModel.CheckConfigFile();

            _initTimer = new System.Timers.Timer(7000);
            _initTimer.Elapsed += OnInitTimerElapsed;
            _initTimer.AutoReset = false;
            _initTimer.Start();

            OnCheckTimerElapsed(null, null);
            _checkTimer = new System.Timers.Timer((61 - DateTime.Now.Second) * 1000);
            _checkTimer.Elapsed += OnCheckTimerElapsed;
            _checkTimer.AutoReset = false;
            _checkTimer.Start();
        }

        #endregion

        #region Timers

        private void OnCsgoTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _alreadyMinimized = false;

            if (_previousState)
                RestoreLights();

            _previousState = false;
        }

        private void OnInitTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _firstCsgoIteration = false;
        }

        public void OnCheckTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // For some reason the NumericUpDown was setting sometimes the variables as HH:mm:ss
            // This was crashing the app, so this is a workarround.
            if (Properties.Settings.Default.AutoActivateStart.Length > 5)
                Properties.Settings.Default.AutoActivateStart = Properties.Settings.Default.AutoActivateStart.Substring(0, 5);
            if (Properties.Settings.Default.AutoActivateEnd.Length > 5)
                Properties.Settings.Default.AutoActivateEnd = Properties.Settings.Default.AutoActivateEnd.Substring(0, 5);

            var start = DateTime.ParseExact(Properties.Settings.Default.AutoActivateStart, "HH:mm", CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(Properties.Settings.Default.AutoActivateEnd, "HH:mm", CultureInfo.InvariantCulture);
            var now = DateTime.Now;

            if (end.Hour > start.Hour || end.Hour == start.Hour && end.Minute > start.Minute)
            {
                if (now.Hour > start.Hour || now.Hour == start.Hour && now.Minute >= start.Minute)
                {
                    if (now.Hour < end.Hour || now.Hour == end.Hour && now.Minute < end.Minute)
                    {
                        if (Properties.Settings.Default.AutoActivate && !_lastActivatedState)
                            Properties.Settings.Default.Activated = true;

                        _lastActivatedState = true;
                    }
                    else if (Properties.Settings.Default.AutoDeactivate)
                    {
                        if (Properties.Settings.Default.AutoActivate && _lastActivatedState)
                            Properties.Settings.Default.Activated = false;

                        _lastActivatedState = false;
                    }
                }
                else if (Properties.Settings.Default.AutoDeactivate)
                {
                    if (Properties.Settings.Default.AutoActivate && _lastActivatedState)
                        Properties.Settings.Default.Activated = false;

                    _lastActivatedState = false;
                }
            }
            else
            {
                if (now.Hour > start.Hour || now.Hour == start.Hour && now.Minute >= start.Minute)
                {
                    if (Properties.Settings.Default.AutoActivate && !_lastActivatedState)
                        Properties.Settings.Default.Activated = true;

                    _lastActivatedState = true;
                }
                else if (now.Hour < end.Hour || now.Hour == end.Hour && now.Minute < end.Minute)
                {
                    if (Properties.Settings.Default.AutoActivate && !_lastActivatedState)
                        Properties.Settings.Default.Activated = true;

                    _lastActivatedState = true;
                }
                else if (Properties.Settings.Default.AutoDeactivate)
                {
                    if (Properties.Settings.Default.AutoActivate && _lastActivatedState)
                        Properties.Settings.Default.Activated = false;

                    _lastActivatedState = false;
                }
            }

            if (!_firstCheckIteration)
            {
                _checkTimer = new System.Timers.Timer(60000);
                _checkTimer.Elapsed += OnCheckTimerElapsed;
                _checkTimer.AutoReset = false;
                _checkTimer.Start();
            }
            else
                _firstCheckIteration = false;
        }

        #endregion

        #region LightsSetting

        public async Task SetLightsAsync(EventProperty config)
        {
            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.UniqueId).Any()) continue;

                var color = l.Color;
                var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
                var brightnessModified = brightnessModifier <= 1
                    ? (byte)Math.Round(l.Brightness * brightnessModifier)
                    : (byte)(l.Brightness + Math.Round((255 - l.Brightness) * (brightnessModifier - 1)));

                try
                {
                    if (l.IsColorTemperature)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                            Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }

                _alreadySetLights = false;
            }
        }

        public async Task SetLightsAsync(EventBrightnessProperty config, EventProperty main)
        {
            var bombblinklightsids = new List<string>();
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
                bombblinklightsids.Add(l.UniqueId);

            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.UniqueId).Any()) continue;

                if (_isPlanted && bombblinklightsids.Contains(l.UniqueId)) continue;

                var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
                var brightnessModified = brightnessModifier <= 1
                    ? (byte)Math.Round(l.Brightness * brightnessModifier)
                    : (byte)(l.Brightness + Math.Round((255 - l.Brightness) * (brightnessModifier - 1)));

                if (!l.OnlyBrightness)
                {

                    try
                    {
                        if (l.IsColorTemperature)
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                        else
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                Hue = (int)Math.Round(ColorConverters.GetHue(l.Color) / 360 * 65535),
                                Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }

                    _alreadySetLights = false;
                    continue;
                }

                try
                {
                    if (l.IsColorTemperature)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(main.Lights.Find(x => x.UniqueId == l.UniqueId).Color) /
                                                  360 *
                                                  65535),
                            Saturation =
                                (byte)Math.Round(
                                    ColorConverters.GetSaturation(main.Lights.Find(x => x.UniqueId == l.UniqueId).Color) *
                                    255),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }

                _alreadySetLights = false;
            }
        }

        public async Task SetLightsAsync(EventBrightnessProperty config, EventBrightnessProperty config2, EventProperty main)
        {
            var bombblinklightsids = new List<string>();
            foreach (var l in Properties.Settings.Default.BombBlink.Lights)
                bombblinklightsids.Add(l.UniqueId);

            foreach (var l in config.Lights)
            {
                if (!config.SelectedLights.FindAll(x => x == l.UniqueId).Any()) continue;

                if (_isPlanted && bombblinklightsids.Contains(l.UniqueId)) continue;

                var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
                var brightnessModified = brightnessModifier <= 1
                    ? (byte)Math.Round(l.Brightness * brightnessModifier)
                    : (byte)(l.Brightness + Math.Round((255 - l.Brightness) * (brightnessModifier - 1)));

                if (!l.OnlyBrightness)
                {
                    try
                    {
                        if (l.IsColorTemperature)
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                        else
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                Hue = (int)Math.Round(ColorConverters.GetHue(l.Color) / 360 * 65535),
                                Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }

                    _alreadySetLights = false;
                    continue;
                }

                if (!config2.Lights.Find(x => x.UniqueId == l.UniqueId).OnlyBrightness)
                {
                    try
                    {
                        if (l.IsColorTemperature)
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                        else
                            await Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                Hue = (int)Math.Round(
                                    ColorConverters.GetHue(config2.Lights.Find(x => x.UniqueId == l.UniqueId).Color) / 360 * 65535),
                                Saturation =
                                    (byte)Math.Round(
                                        ColorConverters.GetSaturation(config2.Lights.Find(x => x.UniqueId == l.UniqueId).Color) *
                                        255),
                                Brightness = brightnessModified
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }

                    _alreadySetLights = false;
                    continue;
                }

                try
                {
                    if (l.IsColorTemperature)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            ColorTemperature = (int)Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(main.Lights.Find(x => x.UniqueId == l.UniqueId).Color) /
                                                  360 *
                                                  65535),
                            Saturation =
                                (byte)Math.Round(
                                    ColorConverters.GetSaturation(main.Lights.Find(x => x.UniqueId == l.UniqueId).Color) *
                                    255),
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }

                _alreadySetLights = false;
            }
        }

        public async void RestoreLights()
        {
            if (GlobalLightsBackup == null
                || !Properties.Settings.Default.RememberLightsStates
                || _alreadySetLights)
                return;

            _alreadySetLights = true;

            foreach (var l in GlobalLightsBackup)
            {
                if (l.State.IsReachable != true) continue;

                var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
                var brightnessModified = brightnessModifier <= 1
                    ? (byte)Math.Round(l.State.Brightness * brightnessModifier)
                    : (byte)(l.State.Brightness + Math.Round((255 - l.State.Brightness) * (brightnessModifier - 1)));

                try
                {
                    if (l.Capabilities.Control.ColorGamut == null)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = l.State.On,
                            ColorTemperature = l.State.ColorTemperature,
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = l.State.On,
                            Hue = l.State.Hue,
                            Saturation = l.State.Saturation,
                            Brightness = brightnessModified
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            }

            GlobalLightsBackup = null;
        }

        public async void BlinkingBombAsync()
        {
            if (!_isPlanted || Properties.Settings.Default.BombBlink.SelectedLights.Count == 0)
                return;

            var uniqueId = Properties.Settings.Default.BombBlink.SelectedLights.First();

            var light = Properties.Settings.Default.BombBlink.Lights.Find(x => x.UniqueId == uniqueId);
            var plantedLight = Properties.Settings.Default.BombPlanted.Lights.Find(x => x.UniqueId == uniqueId);

            var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
            var brightnessModifiedLight = brightnessModifier <= 1
                ? (byte)Math.Round(light.Brightness * brightnessModifier)
                : (byte)(light.Brightness + Math.Round((255 - light.Brightness) * (brightnessModifier - 1)));
            var brightnessModifiedPlantedLight = brightnessModifier <= 1
                ? (byte)Math.Round(plantedLight.Brightness * brightnessModifier)
                : (byte)(plantedLight.Brightness + Math.Round((255 - plantedLight.Brightness) * (brightnessModifier - 1)));

            if (!light.OnlyBrightness)
            {
                try
                {
                    if (light.IsColorTemperature)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            ColorTemperature = (int)Math.Round(light.ColorTemperature * -0.077111 + 654.222),
                            Brightness = brightnessModifiedLight
                        }, new List<string> { $"{light.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(light.Color) / 360 * 65535),
                            Saturation = (byte)Math.Round(ColorConverters.GetSaturation(light.Color) * 255),
                            Brightness = brightnessModifiedLight,
                            TransitionTime = TimeSpan.FromMilliseconds(100)
                        }, new List<string> { $"{light.Id}" })
                            .ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    if (light.IsColorTemperature)
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            ColorTemperature = (int)Math.Round(plantedLight.ColorTemperature * -0.077111 + 654.222),
                            Brightness = brightnessModifiedLight
                        }, new List<string> { $"{light.Id}" }).ConfigureAwait(false);
                    else
                        await Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(plantedLight.Color) / 360 * 65535),
                            Saturation = (byte)Math.Round(ColorConverters.GetSaturation(plantedLight.Color) * 255),
                            Brightness = brightnessModifiedLight,
                            TransitionTime = TimeSpan.FromMilliseconds(100)
                        }, new List<string> { $"{light.Id}" })
                            .ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            }

            Thread.Sleep(100);

            try
            {
                if (light.IsColorTemperature)
                    await Client.SendCommandAsync(new LightCommand
                    {
                        On = true,
                        ColorTemperature = (int)Math.Round(plantedLight.ColorTemperature * -0.077111 + 654.222),
                        Brightness = brightnessModifiedPlantedLight
                    }, new List<string> { $"{light.Id}" }).ConfigureAwait(false);
                else
                    await Client.SendCommandAsync(new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(ColorConverters.GetHue(plantedLight.Color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(plantedLight.Color) * 255),
                        Brightness = brightnessModifiedPlantedLight,
                        TransitionTime = TimeSpan.FromMilliseconds(100)
                    }, new List<string> { $"{light.Id}" }).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }

            _alreadySetLights = false;
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
            if (!_flashedZeroed)
                return;

            SetLightsAsync(Properties.Settings.Default.BombExplodes, Properties.Settings.Default.BombPlanted).Wait();
        }

        public void RoundEnd(RoundWinTeam winner)
        {
            if (!_flashedZeroed)
                return;

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

        public void Navigate(string name)
        {
            switch (name)
            {
                case "Config":
                    Content = Config;
                    break;
                case "Donate":
                    Content = Donate;
                    break;
                case "Settings":
                    Content = Settings;
                    break;
                case "About":
                    Content = About;
                    break;
                case "Help":
                    Content = Help;
                    break;
                case "Update":
                    Update.ViewModel.WarningUpdateVisibility = WarningUpdateVisibility;
                    Content = Update;
                    break;
                default:
                    Content = Home;
                    break;
            }
        }

        public string GetLastVersion()
        {
            string data;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://github.com/joao7yt/CSHUE/tags");

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    return "";

                    using (var receiveStream = response.GetResponseStream())
                    {
                        if (receiveStream == null)
                            return "";

                        using (var readStream = response.CharacterSet == null
                            ? new StreamReader(receiveStream)
                            : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                        {
                            data = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return "";
            }

            var latestVersion = Regex.Match(data, "\"/joao7yt/CSHUE/releases/tag/(.*)\"").Groups[1].Value;

            WarningUpdateVisibility = Core.Utilities.Version.CurrentVersion != latestVersion ? Visibility.Visible : Visibility.Collapsed;

            return Core.Utilities.Version.CurrentVersion != latestVersion ? latestVersion : "";
        }

        public void RunCsgo()
        {
            if (Process.GetProcessesByName("csgo").Length > 0)
                return;

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

            if (string.IsNullOrEmpty(Properties.Settings.Default.SteamFolder) ||
                !File.Exists(Properties.Settings.Default.SteamFolder + "\\Steam.exe"))
                return;

            Process.Start("steam://run/730//" + Properties.Settings.Default.LaunchOptions + "/");
        }

        public void SearchAgain()
        {
            HueAsync(true);
        }

        #endregion

        #endregion

        #region Events Handlers

        public async void OnNewGameState(GameState gs)
        {
            if (GlobalLightsBackup == null &&
                !string.IsNullOrEmpty(Properties.Settings.Default.AppKey) &&
                Client != null)
            {
                try
                {
                    GlobalLightsBackup = (await Client.GetLightsAsync()).ToList();
                }
                catch
                {
                    //ignored
                }
            }

            if (WindowState != WindowState.Minimized &&
                Properties.Settings.Default.AutoMinimize &&
                !_alreadyMinimized &&
                !_previousState)
            {
                _alreadyMinimized = true;

                if (!_firstCsgoIteration)
                    WindowState = WindowState.Minimized;
            }

            _previousState = true;

            if (_csgoTimer == null)
            {
                _csgoTimer = new System.Timers.Timer(10000);
                _csgoTimer.Elapsed += OnCsgoTimerElapsed;
                _csgoTimer.AutoReset = false;
                _csgoTimer.Start();
            }
            else
            {
                _csgoTimer.Stop();
                _csgoTimer.Start();
            }

            if (!Properties.Settings.Default.Activated ||
                Client == null ||
                GlobalLightsBackup == null)
                return;

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

            if (_blockLightChange)
                return;

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

            switch (gs.Previously.Round.Bomb)
            {
                case BombState.Planted when gs.Round.Bomb == BombState.Exploded:
                    _isPlanted = false;
                    BombExplodes();
                    break;
                case BombState.Planted:
                {
                    if (gs.Round.Bomb != BombState.Planted)
                    {
                        switch (gs.Round.Phase)
                        {
                            case RoundPhase.FreezeTime:
                                _isPlanted = false;
                                SetLightsAsync(Properties.Settings.Default.FreezeTime, Properties.Settings.Default.RoundStarts).Wait();
                                break;
                            case RoundPhase.Live:
                                _isPlanted = false;
                                RoundStarts();
                                break;
                        }
                    }
                    break;
                }
                case BombState.Exploded:
                    switch (gs.Round.Phase)
                    {
                        case RoundPhase.Live:
                            RoundStarts();
                            break;
                    }
                    break;
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

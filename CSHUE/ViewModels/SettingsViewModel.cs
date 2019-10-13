using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CSHUE.Core;
using CSHUE.Cultures;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// Main windows viewmodel field.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel = null;

        /// <summary>
        /// Lights backup field.
        /// </summary>
        public List<Light> LightsBackup;

        #endregion

        #region Properties

        /// <summary>
        /// Themes list back field.
        /// </summary>
        private ObservableCollection<Theme> _themes = new ObservableCollection<Theme>
        {
            new Theme
            {
                Text = Resources.Default,
                Index = 0
            },
            new Theme
            {
                Text = Resources.ThemeDark,
                Index = 1
            },
            new Theme
            {
                Text = Resources.ThemeLight,
                Index = 2
            }
        };
        /// <summary>
        /// Themes list property.
        /// </summary>
        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            set
            {
                _themes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formula back field.
        /// </summary>
        private string _formula = $"{Resources.BrightnessPercentage} * 0.00";
        /// <summary>
        /// Formula property.
        /// </summary>
        public string Formula
        {
            get => _formula;
            set
            {
                _formula = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// SelectedTheme back field.
        /// </summary>
        private Theme _selectedTheme;
        /// <summary>
        /// SelectedTheme property.
        /// </summary>
        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Transparencies list back field.
        /// </summary>
        private ObservableCollection<Transparency> _transparencies = new ObservableCollection<Transparency>
        {
            new Transparency
            {
                Text = Resources.Default,
                Index = 0
            },
            new Transparency
            {
                Text = Resources.Yes,
                Index = 1
            },
            new Transparency
            {
                Text = Resources.No,
                Index = 2
            }
        };
        /// <summary>
        /// Transparencies list property.
        /// </summary>
        public ObservableCollection<Transparency> Transparencies
        {
            get => _transparencies;
            set
            {
                _transparencies = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// SelectedTransparency back field.
        /// </summary>
        private Transparency _selectedTransparency;
        /// <summary>
        /// SelectedTransparency property.
        /// </summary>
        public Transparency SelectedTransparency
        {
            get => _selectedTransparency;
            set
            {
                _selectedTransparency = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// MainMenu gradients stops back field.
        /// </summary>
        private GradientStopCollection _gradientStopsMainMenu;
        /// <summary>
        /// MainMenu gradients stops property.
        /// </summary>
        public GradientStopCollection GradientStopsMainMenu
        {
            get => _gradientStopsMainMenu;
            set
            {
                _gradientStopsMainMenu = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PlayerGetsKill gradients stops back field.
        /// </summary>
        private GradientStopCollection _gradientStopsPlayerGetsKill;
        /// <summary>
        /// PlayerGetsKill gradients stops property.
        /// </summary>
        public GradientStopCollection GradientStopsPlayerGetsKill
        {
            get => _gradientStopsPlayerGetsKill;
            set
            {
                _gradientStopsPlayerGetsKill = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PlayerGetsKilled gradients stops back field.
        /// </summary>
        private GradientStopCollection _gradientStopsPlayerGetsKilled;
        /// <summary>
        /// PlayerGetsKilled gradients stops property.
        /// </summary>
        public GradientStopCollection GradientStopsPlayerGetsKilled
        {
            get => _gradientStopsPlayerGetsKilled;
            set
            {
                _gradientStopsPlayerGetsKilled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PlayerGetsFlashed gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsPlayerGetsFlashed;
        /// <summary>
        /// PlayerGetsFlashed gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsPlayerGetsFlashed
        {
            get => _gradientStopsPlayerGetsFlashed;
            set
            {
                _gradientStopsPlayerGetsFlashed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// TerroristsWin gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsTerroristsWin;
        /// <summary>
        /// TerroristsWin gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsTerroristsWin
        {
            get => _gradientStopsTerroristsWin;
            set
            {
                _gradientStopsTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CounterTerroristsWin gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsCounterTerroristsWin;
        /// <summary>
        /// CounterTerroristsWin gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsCounterTerroristsWin
        {
            get => _gradientStopsCounterTerroristsWin;
            set
            {
                _gradientStopsCounterTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// RoundStarts gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsRoundStarts;
        /// <summary>
        /// RoundStarts gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsRoundStarts
        {
            get => _gradientStopsRoundStarts;
            set
            {
                _gradientStopsRoundStarts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// FreezeTime gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsFreezeTime;
        /// <summary>
        /// FreezeTime gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsFreezeTime
        {
            get => _gradientStopsFreezeTime;
            set
            {
                _gradientStopsFreezeTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Warmup gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsWarmup;
        /// <summary>
        /// Warmup gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsWarmup
        {
            get => _gradientStopsWarmup;
            set
            {
                _gradientStopsWarmup = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// BombExplodes gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsBombExplodes;
        /// <summary>
        /// BombExplodes gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsBombExplodes
        {
            get => _gradientStopsBombExplodes;
            set
            {
                _gradientStopsBombExplodes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// BombPlanted gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsBombPlanted;
        /// <summary>
        /// BombPlanted gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsBombPlanted
        {
            get => _gradientStopsBombPlanted;
            set
            {
                _gradientStopsBombPlanted = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// BombBlink gradients stops back field. 
        /// </summary>
        private GradientStopCollection _gradientStopsBombBlink;
        /// <summary>
        /// BombBlink gradients stops property. 
        /// </summary>
        public GradientStopCollection GradientStopsBombBlink
        {
            get => _gradientStopsBombBlink;
            set
            {
                _gradientStopsBombBlink = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method that adds the startup path in the Registry.
        /// </summary>
        /// <param name="silent">The silent parameter.</param>
        public void AddStartup(bool silent)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (silent)
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule?.FileName + "\" -silent");
            else
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule?.FileName + "\"");
        }

        /// <summary>
        /// Method that removes the startup path from the Registry.
        /// </summary>
        public void RemoveStartup()
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk?.DeleteValue("CSHUE", false);
        }

        /// <summary>
        /// Method used to call the true update gradient method.
        /// </summary>
        public void UpdateGradient(string propertyName)
        {
            switch (propertyName)
            {
                case "MainMenu":
                    UpdateGradient(propertyName, Properties.Settings.Default.MainMenu);
                    break;
                case "PlayerGetsKill":
                    UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsKill);
                    break;
                case "PlayerGetsKilled":
                    UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsKilled);
                    break;
                case "PlayerGetsFlashed":
                    UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsFlashed);
                    break;
                case "TerroristsWin":
                    UpdateGradient(propertyName, Properties.Settings.Default.TerroristsWin);
                    break;
                case "CounterTerroristsWin":
                    UpdateGradient(propertyName, Properties.Settings.Default.CounterTerroristsWin);
                    break;
                case "RoundStarts":
                    UpdateGradient(propertyName, Properties.Settings.Default.RoundStarts);
                    break;
                case "FreezeTime":
                    UpdateGradient(propertyName, Properties.Settings.Default.FreezeTime);
                    break;
                case "Warmup":
                    UpdateGradient(propertyName, Properties.Settings.Default.Warmup);
                    break;
                case "BombExplodes":
                    UpdateGradient(propertyName, Properties.Settings.Default.BombExplodes);
                    break;
                case "BombPlanted":
                    UpdateGradient(propertyName, Properties.Settings.Default.BombPlanted);
                    break;
                case "BombBlink":
                    UpdateGradient(propertyName, Properties.Settings.Default.BombBlink);
                    break;
            }
        }

        /// <summary>
        /// Method that updates all the events gradients.
        /// </summary>
        public void UpdateGradients()
        {
            UpdateGradient("MainMenu", Properties.Settings.Default.MainMenu);
            UpdateGradient("PlayerGetsKill", Properties.Settings.Default.PlayerGetsKill);
            UpdateGradient("PlayerGetsKilled", Properties.Settings.Default.PlayerGetsKilled);
            UpdateGradient("PlayerGetsFlashed", Properties.Settings.Default.PlayerGetsFlashed);
            UpdateGradient("TerroristsWin", Properties.Settings.Default.TerroristsWin);
            UpdateGradient("CounterTerroristsWin", Properties.Settings.Default.CounterTerroristsWin);
            UpdateGradient("RoundStarts", Properties.Settings.Default.RoundStarts);
            UpdateGradient("FreezeTime", Properties.Settings.Default.FreezeTime);
            UpdateGradient("Warmup", Properties.Settings.Default.Warmup);
            UpdateGradient("BombExplodes", Properties.Settings.Default.BombExplodes);
            UpdateGradient("BombPlanted", Properties.Settings.Default.BombPlanted);
            UpdateGradient("BombBlink", Properties.Settings.Default.BombBlink);
        }

        /// <summary>
        /// Method that updates a single event gradient.
        /// </summary>
        private void UpdateGradient(string propertyName, EventProperty Event)
        {
            switch (propertyName)
            {
                case "MainMenu" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsMainMenu = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsMainMenu.Add(new GradientStop
                                {
                                    Color = !selectedLights.Any()
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "MainMenu":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsMainMenu = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "PlayerGetsFlashed" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsPlayerGetsFlashed = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsPlayerGetsFlashed.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "PlayerGetsFlashed":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsPlayerGetsFlashed = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "TerroristsWin" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsTerroristsWin = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsTerroristsWin.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "TerroristsWin":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsTerroristsWin = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "CounterTerroristsWin" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsCounterTerroristsWin = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsCounterTerroristsWin.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "CounterTerroristsWin":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsCounterTerroristsWin = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "RoundStarts" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsRoundStarts = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsRoundStarts.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "RoundStarts":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsRoundStarts = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "BombPlanted" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsBombPlanted = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsBombPlanted.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].Color
                                            : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "BombPlanted":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsBombPlanted = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
            }
        }

        /// <summary>
        /// Method that updates a single brightness event gradient.
        /// </summary>
        private void UpdateGradient(string propertyName, EventBrightnessProperty Event)
        {
            switch (propertyName)
            {
                case "PlayerGetsKill" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsPlayerGetsKill = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsPlayerGetsKill.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? Colors.Gray
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? Colors.Gray
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "PlayerGetsKill":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsPlayerGetsKill = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "PlayerGetsKilled" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsPlayerGetsKilled = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsPlayerGetsKilled.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? Colors.Gray
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? Colors.Gray
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "PlayerGetsKilled":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsPlayerGetsKilled = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "FreezeTime" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));
                        var selectedMainLights =
                            Properties.Settings.Default.RoundStarts.Lights.FindAll(x =>
                                Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsFreezeTime = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsFreezeTime.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? selectedMainLights[0].Color
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? selectedMainLights[i].Color
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "FreezeTime":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsFreezeTime = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "Warmup" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));
                        var selectedMainLights =
                            Properties.Settings.Default.RoundStarts.Lights.FindAll(x =>
                                Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsWarmup = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsWarmup.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? selectedMainLights[0].Color
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? selectedMainLights[i].Color
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "Warmup":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsWarmup = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "BombExplodes" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));
                        var selectedMainLights =
                            Properties.Settings.Default.BombPlanted.Lights.FindAll(x =>
                                Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsBombExplodes = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsBombExplodes.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? selectedMainLights[0].Color
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? selectedMainLights[i].Color
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "BombExplodes":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsBombExplodes = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
                case "BombBlink" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.UniqueId));
                        var selectedMainLights =
                            Properties.Settings.Default.BombPlanted.Lights.FindAll(x =>
                                Event.SelectedLights.Contains(x.UniqueId));

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            GradientStopsBombBlink = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsBombBlink.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? selectedLights[0].OnlyBrightness
                                                ? selectedMainLights[0].Color
                                                : selectedLights[0].Color
                                            : selectedLights[i].OnlyBrightness
                                                ? selectedMainLights[i].Color
                                                : selectedLights[i].Color,
                                    Offset = i * 1 / (float) ((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "BombBlink":
                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        GradientStopsBombBlink = new GradientStopCollection
                        {
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                    break;
            }
        }
        
        /// <summary>
        /// Method that restore all lights.
        /// </summary>
        public async void RestoreLights()
        {
            if (LightsBackup != null)
                foreach (var l in LightsBackup)
                {
                    if (l.State.IsReachable == true)
                        try
                        {
                            var brightnessModifier = (double)Properties.Settings.Default.BrightnessModifier / 100;
                            var brightnessModified = brightnessModifier <= 1
                                ? (byte)Math.Round(l.State.Brightness * brightnessModifier)
                                : (byte)(l.State.Brightness + Math.Round((255 - l.State.Brightness) * (brightnessModifier - 1)));

                            if (l.Capabilities.Control.ColorGamut == null)
                                await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                                {
                                    On = l.State.On,
                                    ColorTemperature = l.State.ColorTemperature,
                                    Brightness = brightnessModified
                                }, new List<string> {$"{l.Id}"}).ConfigureAwait(false);
                            else
                                await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                                {
                                    On = l.State.On,
                                    Hue = l.State.Hue,
                                    Saturation = l.State.Saturation,
                                    Brightness = brightnessModified
                                }, new List<string> {$"{l.Id}"}).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignored
                        }
                }
        }

        #endregion
    }

    public class Theme
    {
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class Transparency
    {
        public string Text { get; set; }
        public int Index { get; set; }
    }
}

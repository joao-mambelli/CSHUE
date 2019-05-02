using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CSHUE.Helpers;
// ReSharper disable SwitchStatementMissingSomeCases

namespace CSHUE.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        public void AddStartup(bool silent)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (silent)
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule?.FileName + "\" -silent");
            else
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule?.FileName + "\"");
        }

        public void RemoveStartup()
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk?.DeleteValue("CSHUE", false);
        }

        private GradientStopCollection _gradientStopsMainMenu;
        public GradientStopCollection GradientStopsMainMenu
        {
            get =>
                _gradientStopsMainMenu;
            set
            {
                _gradientStopsMainMenu = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsKill;
        public GradientStopCollection GradientStopsPlayerGetsKill
        {
            get =>
                _gradientStopsPlayerGetsKill;
            set
            {
                _gradientStopsPlayerGetsKill = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsKilled;
        public GradientStopCollection GradientStopsPlayerGetsKilled
        {
            get =>
                _gradientStopsPlayerGetsKilled;
            set
            {
                _gradientStopsPlayerGetsKilled = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsFlashed;
        public GradientStopCollection GradientStopsPlayerGetsFlashed
        {
            get =>
                _gradientStopsPlayerGetsFlashed;
            set
            {
                _gradientStopsPlayerGetsFlashed = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsTerroristsWin;
        public GradientStopCollection GradientStopsTerroristsWin
        {
            get =>
                _gradientStopsTerroristsWin;
            set
            {
                _gradientStopsTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsCounterTerroristsWin;
        public GradientStopCollection GradientStopsCounterTerroristsWin
        {
            get =>
                _gradientStopsCounterTerroristsWin;
            set
            {
                _gradientStopsCounterTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsRoundStarts;
        public GradientStopCollection GradientStopsRoundStarts
        {
            get =>
                _gradientStopsRoundStarts;
            set
            {
                _gradientStopsRoundStarts = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsFreezeTime;
        public GradientStopCollection GradientStopsFreezeTime
        {
            get =>
                _gradientStopsFreezeTime;
            set
            {
                _gradientStopsFreezeTime = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsWarmup;
        public GradientStopCollection GradientStopsWarmup
        {
            get =>
                _gradientStopsWarmup;
            set
            {
                _gradientStopsWarmup = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombExplodes;
        public GradientStopCollection GradientStopsBombExplodes
        {
            get =>
                _gradientStopsBombExplodes;
            set
            {
                _gradientStopsBombExplodes = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombPlanted;
        public GradientStopCollection GradientStopsBombPlanted
        {
            get =>
                _gradientStopsBombPlanted;
            set
            {
                _gradientStopsBombPlanted = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombBlink;
        public GradientStopCollection GradientStopsBombBlink
        {
            get =>
                _gradientStopsBombBlink;
            set
            {
                _gradientStopsBombBlink = value;
                OnPropertyChanged();
            }
        }

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

        private void UpdateGradient(string propertyName, EventProperty Event)
        {
            switch (propertyName)
            {
                case "MainMenu" when Event?.SelectedLights != null:
                    {
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsMainMenu = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsMainMenu.Add(new GradientStop
                                {
                                    Color = !selectedLights.Any()
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "MainMenu":
                    Application.Current.Dispatcher.Invoke(() =>
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
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsPlayerGetsFlashed = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsPlayerGetsFlashed.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "PlayerGetsFlashed":
                    Application.Current.Dispatcher.Invoke(() =>
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
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsTerroristsWin = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsTerroristsWin.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "TerroristsWin":
                    Application.Current.Dispatcher.Invoke(() =>
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
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsCounterTerroristsWin = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsCounterTerroristsWin.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "CounterTerroristsWin":
                    Application.Current.Dispatcher.Invoke(() =>
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
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsRoundStarts = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsRoundStarts.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "RoundStarts":
                    Application.Current.Dispatcher.Invoke(() =>
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
                        var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GradientStopsBombPlanted = new GradientStopCollection();
                            for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                            {
                                GradientStopsBombPlanted.Add(new GradientStop
                                {
                                    Color = selectedLights.Count < 1
                                        ? Colors.Black
                                        : selectedLights.Count < 2
                                            ? Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                    Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                                });
                            }
                        });
                        break;
                    }
                case "BombPlanted":
                    Application.Current.Dispatcher.Invoke(() =>
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

        private void UpdateGradient(string propertyName, EventBrightnessProperty Event)
        {
            switch (propertyName)
            {
                case "PlayerGetsKill" when Event?.SelectedLights != null:
                {
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Colors.Gray
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "PlayerGetsKill":
                    Application.Current.Dispatcher.Invoke(() =>
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
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Colors.Gray
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "PlayerGetsKilled":
                    Application.Current.Dispatcher.Invoke(() =>
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
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));
                    var selectedMainLights =
                        Properties.Settings.Default.RoundStarts.Lights.FindAll(x =>
                            Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            ? Color.FromRgb(selectedMainLights[0].Color.Red, selectedMainLights[0].Color.Green, selectedMainLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Color.FromRgb(selectedMainLights[i].Color.Red, selectedMainLights[i].Color.Green, selectedMainLights[i].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "FreezeTime":
                    Application.Current.Dispatcher.Invoke(() =>
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
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));
                    var selectedMainLights =
                        Properties.Settings.Default.RoundStarts.Lights.FindAll(x =>
                            Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            ? Color.FromRgb(selectedMainLights[0].Color.Red, selectedMainLights[0].Color.Green, selectedMainLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Color.FromRgb(selectedMainLights[i].Color.Red, selectedMainLights[i].Color.Green, selectedMainLights[i].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "Warmup":
                    Application.Current.Dispatcher.Invoke(() =>
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
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));
                    var selectedMainLights =
                        Properties.Settings.Default.BombPlanted.Lights.FindAll(x =>
                            Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            ? Color.FromRgb(selectedMainLights[0].Color.Red, selectedMainLights[0].Color.Green, selectedMainLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Color.FromRgb(selectedMainLights[i].Color.Red, selectedMainLights[i].Color.Green, selectedMainLights[i].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "BombExplodes":
                    Application.Current.Dispatcher.Invoke(() =>
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
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));
                    var selectedMainLights =
                        Properties.Settings.Default.BombPlanted.Lights.FindAll(x =>
                            Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
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
                                            ? Color.FromRgb(selectedMainLights[0].Color.Red, selectedMainLights[0].Color.Green, selectedMainLights[0].Color.Blue)
                                            : Color.FromRgb(selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : selectedLights[i].OnlyBrightness
                                            ? Color.FromRgb(selectedMainLights[i].Color.Red, selectedMainLights[i].Color.Green, selectedMainLights[i].Color.Blue)
                                            : Color.FromRgb(selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                    break;
                }
                case "BombBlink":
                    Application.Current.Dispatcher.Invoke(() =>
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
    }
}

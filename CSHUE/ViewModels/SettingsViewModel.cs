using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CSHUE.Helpers;

namespace CSHUE.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        public void AddStartup(bool silent)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (silent)
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" -silent");
            else
                rk?.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\"");
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
            if (propertyName == "MainMenu")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.MainMenu);
            }
            else if (propertyName == "PlayerGetsKill")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsKill);
            }
            else if (propertyName == "PlayerGetsKilled")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsKilled);
            }
            else if (propertyName == "PlayerGetsFlashed")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.PlayerGetsFlashed);
            }
            else if (propertyName == "TerroristsWin")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.TerroristsWin);
            }
            else if (propertyName == "CounterTerroristsWin")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.CounterTerroristsWin);
            }
            else if (propertyName == "RoundStarts")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.RoundStarts);
            }
            else if (propertyName == "FreezeTime")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.FreezeTime);
            }
            else if (propertyName == "Warmup")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.Warmup);
            }
            else if (propertyName == "BombExplodes")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.BombExplodes);
            }
            else if (propertyName == "BombPlanted")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.BombPlanted);
            }
            else if (propertyName == "BombBlink")
            {
                UpdateGradient(propertyName, Properties.Settings.Default.BombBlink);
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
            if (propertyName == "MainMenu")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsMainMenu = new GradientStopCollection()
                        {
                            new GradientStop()
                            {
                                Color = Colors.Black,
                                Offset = 0
                            },
                            new GradientStop()
                            {
                                Color = Colors.Black,
                                Offset = 1
                            }
                        };
                    });
                }
            }
            else if (propertyName == "PlayerGetsFlashed")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsPlayerGetsFlashed = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "TerroristsWin")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsTerroristsWin = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "CounterTerroristsWin")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsCounterTerroristsWin = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "RoundStarts")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsRoundStarts = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "BombPlanted")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsBombPlanted = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
        }

        private void UpdateGradient(string propertyName, EventBrightnessProperty Event)
        {
            if (propertyName == "PlayerGetsKill")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsPlayerGetsKill = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "PlayerGetsKilled")
            {
                if (Event?.SelectedLights != null)
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
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsPlayerGetsKilled = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "FreezeTime")
            {
                if (Event?.SelectedLights != null)
                {
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsFreezeTime = new GradientStopCollection();
                        for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                        {
                            GradientStopsFreezeTime.Add(new GradientStop
                            {
                                Color = selectedLights.Count < 1
                                    ? Colors.Black
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsFreezeTime = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "Warmup")
            {
                if (Event?.SelectedLights != null)
                {
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsWarmup = new GradientStopCollection();
                        for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                        {
                            GradientStopsWarmup.Add(new GradientStop
                            {
                                Color = selectedLights.Count < 1
                                    ? Colors.Black
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsWarmup = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "BombExplodes")
            {
                if (Event?.SelectedLights != null)
                {
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsBombExplodes = new GradientStopCollection();
                        for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                        {
                            GradientStopsBombExplodes.Add(new GradientStop
                            {
                                Color = selectedLights.Count < 1
                                    ? Colors.Black
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsBombExplodes = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
            else if (propertyName == "BombBlink")
            {
                if (Event?.SelectedLights != null)
                {
                    var selectedLights = Event.Lights.FindAll(x => Event.SelectedLights.Contains(x.Id));

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsBombBlink = new GradientStopCollection();
                        for (var i = 0; i < (selectedLights.Count < 2 ? 2 : selectedLights.Count); i++)
                        {
                            GradientStopsBombBlink.Add(new GradientStop
                            {
                                Color = selectedLights.Count < 1
                                    ? Colors.Black
                                    : (selectedLights.Count < 2
                                        ? Color.FromArgb(255, selectedLights[0].Color.Red, selectedLights[0].Color.Green, selectedLights[0].Color.Blue)
                                        : Color.FromArgb(255, selectedLights[i].Color.Red, selectedLights[i].Color.Green, selectedLights[i].Color.Blue)),
                                Offset = i * 1 / (float)((selectedLights.Count < 2 ? 2 : selectedLights.Count) - 1)
                            });
                        }
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GradientStopsBombBlink = new GradientStopCollection()
                    {
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 0
                        },
                        new GradientStop()
                        {
                            Color = Colors.Black,
                            Offset = 1
                        }
                    };
                    });
                }
            }
        }
    }
}

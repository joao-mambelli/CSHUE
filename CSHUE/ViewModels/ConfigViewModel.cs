using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using CSHUE.Cultures;
using CSHUE.Views;

namespace CSHUE.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        private readonly string[] _lines =
        {
            "\"CSHUE\"",
            "{",
            "\t\"uri\" \"http://localhost:3000\"",
            "\t\"timeout\" \"5.0\"",
            "\t\"throttle\" \"0.1\"",
            "\t\"data\"",
            "\t{",
            "\t\t\"provider\"\t\t\t\t\t\"1\"",
            "\t\t\"map\"\t\t\t\t\t\t\"1\"",
            "\t\t\"round\"\t\t\t\t\t\t\"1\"",
            "\t\t\"player_id\"\t\t\t\t\t\"1\"",
            "\t\t\"player_state\"\t\t\t\t\"1\"",
            "\t\t\"player_weapons\"\t\t\t\"1\"",
            "\t\t\"player_match_stats\"\t\t\"1\"",
            "\t\t\"allplayers_match_stats\"\t\"1\"",
            "\t\t\"allplayers_id\"\t\t\t\t\"1\"",
            "\t\t\"allplayers_state\"\t\t\t\"1\"",
            "\t}",
            "}"
        };

        private string _failText;

        private string FailText
        {
            get =>
                _failText;
            set
            {
                _failText = value;
                OnPropertyChanged();
            }
        }

        public void CreateConfigFile()
        {
            var fail = false;

            var path = "";
            var cfgpath = Properties.Settings.Default.CsgoFolder;
            var file = "";

            if (string.IsNullOrEmpty(cfgpath))
            {
                using (var key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
                {
                    if (key != null)
                    {
                        var o = key.GetValue("InstallPath");
                        if (o != null)
                        {
                            path = o as string;
                        }
                        else
                        {
                            FailText = Resources.WarningSteam;
                            fail = true;
                        }
                    }
                    else
                    {
                        using (var key1 = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam"))
                        {
                            if (key1 != null)
                            {
                                var o = key1.GetValue("InstallPath");
                                if (o != null)
                                {
                                    path = o as string;
                                }
                                else
                                {
                                    FailText = Resources.WarningSteam;
                                    fail = true;
                                }
                            }
                            else
                            {
                                FailText = Resources.WarningSteam;
                                fail = true;
                            }
                        }
                    }
                }

                if (!fail)
                {
                    path += "\\steamapps\\";

                    try
                    {
                        file = File.ReadAllText(path + "appmanifest_730.acf");
                    }
                    catch
                    {
                        FailText = Resources.WarningCSGO;
                        fail = true;
                    }

                    if (!fail)
                    {
                        var m = Regex.Match(file,
                            "\"installdir\".*\"(.*)\"");

                        if (m.Groups[1].Value == "")
                        {
                            FailText = Resources.WarningCSGO;
                            fail = true;
                        }
                        else
                        {
                            cfgpath = path +
                                      "common\\" +
                                      m.Groups[1]
                                          .Value +
                                      "\\csgo\\cfg";
                        }
                    }
                }
            }

            if (fail || !Directory.Exists(cfgpath))
            {
                Window messageBox = new CustomMessageBox
                {
                    Yes = Resources.Ok,
                    Message = $"{FailText} {Resources.SelectFolder}"
                };
                messageBox.Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                messageBox.ShowDialog();

                using (var fbd = new CommonOpenFileDialog())
                {
                    fbd.Title = Resources.FolderSelection;
                    fbd.IsFolderPicker = true;
                    fbd.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";

                    fbd.AddToMostRecentlyUsedList = false;
                    fbd.AllowNonFileSystemItems = false;
                    fbd.DefaultDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                    fbd.EnsureFileExists = true;
                    fbd.EnsurePathExists = true;
                    fbd.EnsureReadOnly = false;
                    fbd.EnsureValidNames = true;
                    fbd.Multiselect = false;
                    fbd.ShowPlacesList = true;

                    var result = fbd.ShowDialog();

                    if (result != CommonFileDialogResult.Ok || string.IsNullOrWhiteSpace(fbd.FileName))
                        return;
                    Properties.Settings.Default.CsgoFolder = fbd.FileName;
                    cfgpath = fbd.FileName;

                    File.WriteAllLines(cfgpath + "\\gamestate_integration_cshue.cfg", _lines);

                    messageBox = new CustomMessageBox
                    {
                        Yes = Resources.Ok,
                        Message = $"{Resources.FileCreated}:\n" + cfgpath + "\\gamestate_integration_cshue.cfg"
                    };
                    messageBox.Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    messageBox.ShowDialog();

                    CheckConfigFile();
                }
            }
            else
            {
                File.WriteAllLines(cfgpath + "\\gamestate_integration_cshue.cfg", _lines);

                Window messageBox = new CustomMessageBox
                {
                    Yes = Resources.Ok,
                    Message = $"{Resources.FileCreated}:\n" + cfgpath + "\\gamestate_integration_cshue.cfg"
                };
                messageBox.Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                messageBox.ShowDialog();

                CheckConfigFile();
            }
        }

        public void CheckConfigFile()
        {
            MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningCsgoVisibility = Visibility.Collapsed;

            var fail = false;

            var path = "";
            var cfgpath = Properties.Settings.Default.CsgoFolder;
            var file = "";

            if (string.IsNullOrEmpty(cfgpath))
            {
                using (var key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
                {
                    if (key != null)
                    {
                        var o = key.GetValue("InstallPath");
                        if (o != null)
                        {
                            path = o as string;
                        }
                        else
                        {
                            MainWindowViewModel.WarningSteamVisibility = Visibility.Visible;
                            fail = true;
                        }
                    }
                    else
                    {
                        using (var key1 = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam"))
                        {
                            if (key1 != null)
                            {
                                var o = key1.GetValue("InstallPath");
                                if (o != null)
                                {
                                    path = o as string;
                                }
                                else
                                {
                                    MainWindowViewModel.WarningSteamVisibility = Visibility.Visible;
                                    fail = true;
                                }
                            }
                            else
                            {
                                MainWindowViewModel.WarningSteamVisibility = Visibility.Visible;
                                fail = true;
                            }
                        }
                    }
                }

                if (!fail)
                {
                    path += "\\steamapps\\";

                    try
                    {
                        file = File.ReadAllText(path + "appmanifest_730.acf");
                    }
                    catch
                    {
                        MainWindowViewModel.WarningCsgoVisibility = Visibility.Visible;
                        fail = true;
                    }

                    if (!fail)
                    {
                        var m = Regex.Match(file,
                            "\"installdir\".*\"(.*)\"");

                        if (m.Groups[1]
                                .Value ==
                            "")
                        {
                            MainWindowViewModel.WarningCsgoVisibility = Visibility.Visible;
                            fail = true;
                        }
                        else
                        {
                            cfgpath = path +
                                      "common\\" +
                                      m.Groups[1]
                                          .Value +
                                      "\\csgo\\cfg";
                        }
                    }
                }
            }

            if (!fail && !File.Exists(cfgpath + "\\gamestate_integration_cshue.cfg"))
            {
                MainWindowViewModel.WarningGsiVisibility = Visibility.Visible;
                fail = true;
            }

            if (!fail && !_lines.SequenceEqual(File.ReadAllLines(cfgpath + "\\gamestate_integration_cshue.cfg")))
            {
                MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Visible;
            }
        }
    }
}

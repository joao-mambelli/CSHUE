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

        private string FailText { get; set; }

        public void CreateConfigFile()
        {
            var fail = false;

            var path = "";
            var cfgpath = Properties.Settings.Default.CsgoFolder;
            var file = "";

            if (string.IsNullOrEmpty(cfgpath))
            {
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
                        else
                        {
                            FailText = Resources.WarningSteam;
                            fail = true;
                        }
                    }
                }

                if (!fail)
                {
                    path += Properties.Settings.Default.SteamFolder + "\\steamapps\\";

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
                        var m = Regex.Match(file, "\"installdir\".*\"(.*)\"");

                        if (m.Groups[1].Value == "")
                        {
                            FailText = Resources.WarningCSGO;
                            fail = true;
                        }
                        else
                        {
                            cfgpath = path + "common\\" + m.Groups[1].Value + "\\csgo\\cfg";
                        }
                    }
                }
            }

            if (fail)
            {
                new CustomMessageBox
                {
                    Text1 = Resources.Ok,
                    Text2 = null,
                    Message = $"{FailText} {(string.IsNullOrEmpty(Properties.Settings.Default.SteamFolder) ? Resources.SelectSteamFolder : Resources.SelectFolder)}",
                    Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
                }.ShowDialog();

                using (var fbd = new CommonOpenFileDialog
                {
                    Title = Resources.FolderSelection,
                    IsFolderPicker = true,
                    InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                    AddToMostRecentlyUsedList = false,
                    AllowNonFileSystemItems = false,
                    DefaultDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                    EnsureFileExists = true,
                    EnsurePathExists = true,
                    EnsureReadOnly = false,
                    EnsureValidNames = true,
                    Multiselect = false,
                    ShowPlacesList = true
                })
                {
                    var result = fbd.ShowDialog();

                    if (result != CommonFileDialogResult.Ok || string.IsNullOrWhiteSpace(fbd.FileName))
                        return;

                    if (string.IsNullOrEmpty(Properties.Settings.Default.SteamFolder))
                    {
                        Properties.Settings.Default.SteamFolder = fbd.FileName;
                        CreateConfigFile();
                        return;
                    }

                    Properties.Settings.Default.CsgoFolder = fbd.FileName;

                    cfgpath = fbd.FileName;

                    File.WriteAllLines(cfgpath + "\\gamestate_integration_cshue.cfg", _lines);

                    CheckConfigFile();

                    new CustomMessageBox
                    {
                        Text1 = Resources.Ok,
                        Text2 = Resources.OpenFolder,
                        Folder = cfgpath,
                        Message = $"{Resources.FileCreated}:\n" + cfgpath + "\\gamestate_integration_cshue.cfg",
                        Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
                    }.ShowDialog();
                }
            }
            else
            {
                Directory.CreateDirectory(cfgpath);

                File.WriteAllLines(cfgpath + "\\gamestate_integration_cshue.cfg", _lines);

                CheckConfigFile();

                new CustomMessageBox
                {
                    Text1 = Resources.Ok,
                    Text2 = Resources.OpenFolder,
                    Folder = cfgpath,
                    Message = $"{Resources.FileCreated}:\n" + cfgpath + "\\gamestate_integration_cshue.cfg",
                    Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
                }.ShowDialog();
            }
        }

        public void CheckConfigFile()
        {
            var fail = false;

            var path = "";
            var cfgpath = Properties.Settings.Default.CsgoFolder;
            var file = "";

            if (string.IsNullOrEmpty(cfgpath))
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
                        path = o as string;
                    else
                    {
                        MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
                        MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
                        MainWindowViewModel.WarningCsgoVisibility = Visibility.Collapsed;
                        MainWindowViewModel.WarningSteamVisibility = Visibility.Visible;
                        fail = true;
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
                        MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
                        MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
                        MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
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
                            MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
                            MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
                            MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
                            MainWindowViewModel.WarningCsgoVisibility = Visibility.Visible;
                            fail = true;
                        }
                        else
                        {
                            cfgpath = path + "common\\" + m.Groups[1].Value + "\\csgo\\cfg";
                        }
                    }
                }
            }

            if (!fail && !File.Exists(cfgpath + "\\gamestate_integration_cshue.cfg"))
            {
                MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningCsgoVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningGsiVisibility = Visibility.Visible;
                fail = true;
            }

            if (!fail && !_lines.SequenceEqual(File.ReadAllLines(cfgpath + "\\gamestate_integration_cshue.cfg")))
            {
                MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningCsgoVisibility = Visibility.Collapsed;
                MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Visible;
            }

            if (fail) return;
            MainWindowViewModel.WarningGsiVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningSteamVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningCsgoVisibility = Visibility.Collapsed;
            MainWindowViewModel.WarningGsiCorruptedVisibility = Visibility.Collapsed;
        }
    }
}

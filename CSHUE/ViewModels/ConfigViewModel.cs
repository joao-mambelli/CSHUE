using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            var cfgpath = "";
            var file = "";

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
                        FailText = "Fail to locate Steam path.";
                        fail = true;
                    }
                }
                else
                {
                    FailText = "Fail to locate Steam path.";
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
                    FailText = "Fail to locate CS:GO path.";
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
                        FailText = "Fail to locate CS:GO path.";
                        fail = true;
                    }
                    else
                    {
                        cfgpath = path +
                                  "common\\" +
                                  m.Groups[1]
                                      .Value +
                                  "\\csgo\\cfg\\gamestate_integration_cshue.cfg";
                    }
                }
            }

            if (fail)
            {
                MessageBox.Show($"{FailText} Please select the CS:GO cfg folder manually.");

                using (var fbd = new CommonOpenFileDialog())
                {
                    fbd.Title = "CS:GO Folder Selection";
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
                    cfgpath = fbd.FileName + "\\gamestate_integration_cshue.cfg";

                    File.WriteAllLines(cfgpath,
                        _lines);

                    MessageBox.Show("File created at:\n" + cfgpath);
                }
            }
            else
            {
                File.WriteAllLines(cfgpath,
                    _lines);

                MessageBox.Show("File created at:\n" + cfgpath);

                CheckConfigFile();
            }
        }

        public void CheckConfigFile()
        {
            var fail = false;

            var path = "";
            var cfgpath = "";
            var file = "";

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
                        MainWindowViewModel.WarningText = "Couldn't find Steam path.";
                        fail = true;
                    }
                }
                else
                {
                    MainWindowViewModel.WarningText = "Couldn't find Steam path.";
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
                    MainWindowViewModel.WarningText = "Couldn't find CS:GO path.";
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
                        MainWindowViewModel.WarningText = "Couldn't find CS:GO path.";
                        fail = true;
                    }
                    else
                    {
                        cfgpath = path +
                                  "common\\" +
                                  m.Groups[1]
                                      .Value +
                                  "\\csgo\\cfg\\gamestate_integration_cshue.cfg";
                    }
                }
            }

            if (!fail && !File.Exists(cfgpath))
            {
                MainWindowViewModel.WarningText = "Couldn't find CS:GO Game State Integration file.";
                fail = true;
            }

            if (!fail && !_lines.SequenceEqual(File.ReadAllLines(cfgpath)))
            {
                MainWindowViewModel.WarningText = "CS:GO Game State Integration file is corrupted.";
                fail = true;
            }

            MainWindowViewModel.WarningVisibility = !fail
                ? Visibility.Hidden
                : Visibility.Visible;
        }
    }
}

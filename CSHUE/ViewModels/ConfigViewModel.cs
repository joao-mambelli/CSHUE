using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Linq;

namespace CSHUE.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        public MainWindowViewModel mainWindowViewModel = null;

        private string[] lines =
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

        public void CreateConfigFile()
        {
            bool fail = false;

            string path = "";
            string cfgpath = "";
            string file = "";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
            {
                if (key != null)
                {
                    object o = key.GetValue("InstallPath");
                    if (o != null)
                    {
                        path = o as string;
                    }
                    else
                    {
                        fail = true;
                    }
                }
                else
                {
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
                    fail = true;
                }

                if (!fail)
                {
                    Match m = Regex.Match(file, "\"installdir\".*\"(.*)\"");

                    if (m.Groups[1].Value == "")
                    {
                        fail = true;
                    }
                    else
                    {
                        cfgpath = path + "common\\" + m.Groups[1].Value + "\\csgo\\cfg\\gamestate_integration_cshue.cfg";
                    }
                }
            }

            if (fail)
            {
                MessageBox.Show("Failed to locate CS:GO folder. Please select it manually.");

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

                    CommonFileDialogResult result = fbd.ShowDialog();

                    if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                    {
                        cfgpath = fbd.FileName + "\\gamestate_integration_cshue.cfg";

                        File.WriteAllLines(cfgpath, lines);

                        MessageBox.Show("File created at:\n" + cfgpath);
                    }
                }
            }
            else
            {
                File.WriteAllLines(cfgpath, lines);

                MessageBox.Show("File created at:\n" + cfgpath);

                CheckConfigFile();
            }
        }

        public void CheckConfigFile()
        {
            bool fail = false;

            string path = "";
            string cfgpath = "";
            string file = "";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam"))
            {
                if (key != null)
                {
                    object o = key.GetValue("InstallPath");
                    if (o != null)
                    {
                        path = o as string;
                    }
                    else
                    {
                        mainWindowViewModel.WarningText = "Couldn't find Steam path.";
                        fail = true;
                    }
                }
                else
                {
                    mainWindowViewModel.WarningText = "Couldn't find Steam path.";
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
                    mainWindowViewModel.WarningText = "Couldn't find CS:GO path.";
                    fail = true;
                }

                if (!fail)
                {
                    Match m = Regex.Match(file, "\"installdir\".*\"(.*)\"");

                    if (m.Groups[1].Value == "")
                    {
                        mainWindowViewModel.WarningText = "Couldn't find CS:GO path.";
                        fail = true;
                    }
                    else
                    {
                        cfgpath = path + "common\\" + m.Groups[1].Value + "\\csgo\\cfg\\gamestate_integration_cshue.cfg";
                    }
                }
            }

            if (!fail && !File.Exists(cfgpath))
            {
                mainWindowViewModel.WarningText = "Couldn't find CS:GO Game State Integration file.";
                fail = true;
            }

            if (!fail && !lines.SequenceEqual(File.ReadAllLines(cfgpath)))
            {
                mainWindowViewModel.WarningText = "CS:GO Game State Integration file is corrupted.";
                fail = true;
            }

            mainWindowViewModel.WarningVisibility = !fail ? Visibility.Hidden : Visibility.Visible;
        }
    }
}

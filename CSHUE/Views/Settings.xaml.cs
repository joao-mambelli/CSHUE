using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CSHUE.Controls;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using CSHUE.Helpers;
using Q42.HueApi;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        #region Fields

        public SettingsViewModel ViewModel = new SettingsViewModel();

        #endregion

        #region Initializers

        public Settings()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;

            ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_SelectionChanged;

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        #endregion

        #region Events Handlers

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ViewModel.UpdateGradient(e.PropertyName);
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.MainWindowViewModel.Resetting) return;

            if (((ComboBox)sender).SelectedIndex == -1)
            {
                var culture = CultureResources.SupportedCultures.Contains(CultureInfo.InstalledUICulture)
                    ? CultureInfo.InstalledUICulture
                    : new CultureInfo("en-US");

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                CultureResources.ChangeCulture(culture);

                Properties.Settings.Default.Language = Converters.GetIndexFromCultureInfo(culture);
            }
            else
            {
                var culture = Converters.GetCultureInfoFromIndex(((ComboBox)sender).SelectedIndex);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                Process.Start(Application.ResourceAssembly.Location, App.Resetting ? "-reset" : "-lang");

                Application.Current.Shutdown();
            }
        }

        private async void Default_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = new CustomMessageBox
            {
                Text1 = Cultures.Resources.Yes,
                Text2 = Cultures.Resources.No,
                Message = Cultures.Resources.AreYouSure,
                Owner = Window.GetWindow(this)
            };
            messageBox.ShowDialog();

            if (messageBox.DialogResult != true) return;

            ComboBoxLanguage.SelectionChanged -= ComboBoxLanguage_SelectionChanged;
            Properties.Settings.Default.Reset();
            ViewModel.MainWindowViewModel.ConfigPage.ViewModel.CheckConfigFile();

            try
            {
                await MainWindowViewModel.SetDefaultLightsSettings();
            }
            catch
            {
                Properties.Settings.Default.AppKey = "";
            }

            Process.Start(Application.ResourceAssembly.Location, "-reset");
            Application.Current.Shutdown();
        }

        private void RunOnStartupCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.RunOnStartup)
                ViewModel.AddStartup(Properties.Settings.Default.RunOnStartupMinimized);
            else
                ViewModel.RemoveStartup();

            Save(sender, e);
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            List<Light> allLights;
            try
            {
                allLights = (await MainWindowViewModel.Client.GetLightsAsync()).ToList();
            }
            catch
            {
                allLights = null;
            }

            if (allLights == null || !allLights.Any())
            {
                new CustomMessageBox
                {
                    Text1 = Cultures.Resources.Ok,
                    Text2 = null,
                    Message = Cultures.Resources.NoLightsFound,
                    Owner = Window.GetWindow(this)
                }.ShowDialog();
                return;
            }

            var title = Cultures.Resources.LightSelector + " - ";
            EventProperty property = null;
            EventBrightnessProperty brightnessProperty = null;
            if (((Button)sender).Tag.ToString() == "MainMenu")
            {
                title += Cultures.Resources.MainMenu;
                property = Properties.Settings.Default.MainMenu;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsFlashed")
            {
                title += Cultures.Resources.PlayerGetsFlashed;
                property = Properties.Settings.Default.PlayerGetsFlashed;
            }
            if (((Button)sender).Tag.ToString() == "TerroristsWin")
            {
                title += Cultures.Resources.TerroristsWin;
                property = Properties.Settings.Default.TerroristsWin;
            }
            if (((Button)sender).Tag.ToString() == "CounterTerroristsWin")
            {
                title += Cultures.Resources.CounterTerroristsWin;
                property = Properties.Settings.Default.CounterTerroristsWin;
            }
            if (((Button)sender).Tag.ToString() == "RoundStarts")
            {
                title += Cultures.Resources.RoundStarts;
                property = Properties.Settings.Default.RoundStarts;
            }
            if (((Button)sender).Tag.ToString() == "BombPlanted")
            {
                title += Cultures.Resources.BombHasBeenPlanted;
                property = Properties.Settings.Default.BombPlanted;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsKill")
            {
                title += Cultures.Resources.PlayerGetsKill;
                brightnessProperty = Properties.Settings.Default.PlayerGetsKill;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsKilled")
            {
                title += Cultures.Resources.PlayerGetsKilled;
                brightnessProperty = Properties.Settings.Default.PlayerGetsKilled;
            }
            if (((Button)sender).Tag.ToString() == "FreezeTime")
            {
                title += Cultures.Resources.FreezeTime;
                brightnessProperty = Properties.Settings.Default.FreezeTime;
            }
            if (((Button)sender).Tag.ToString() == "Warmup")
            {
                title += Cultures.Resources.Warmup;
                brightnessProperty = Properties.Settings.Default.Warmup;
            }
            if (((Button)sender).Tag.ToString() == "BombExplodes")
            {
                title += Cultures.Resources.BombExplodes;
                brightnessProperty = Properties.Settings.Default.BombExplodes;
            }
            if (((Button)sender).Tag.ToString() == "BombBlink")
            {
                title += Cultures.Resources.BombBlink;
                brightnessProperty = Properties.Settings.Default.BombBlink;
            }

            ViewModel.LightsBackup = allLights;
            new LightSelector(title)
            {
                AllLights = allLights,
                Property = property,
                BrightnessProperty = brightnessProperty,
                Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
            }.ShowDialog();
            ViewModel.RestoreLights();

            ViewModel.MainWindowViewModel.SettingsPage.ViewModel.UpdateGradients();

            Save(sender, e);
        }

        private void ShowSystemTrayIconCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel.MainWindowViewModel == null) return;

            ViewModel.MainWindowViewModel.NotifyIconVisibility = Properties.Settings.Default.ShowSystemTrayIcon
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Save(object sender, RoutedEventArgs routedEventArgs)
        {
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}

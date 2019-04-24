using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using CSHUE.Helpers;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public SettingsViewModel ViewModel = new SettingsViewModel();

        public Settings()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;

            ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_SelectionChanged;

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ViewModel.UpdateGradient(e.PropertyName);
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

                Process.Start(Application.ResourceAssembly.Location, "-reset");
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
            await MainWindowViewModel.SetDefaultLightsSettings();

            Process.Start(Application.ResourceAssembly.Location, "-reset");
            Application.Current.Shutdown();
        }

        private void RunOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.RunOnStartup)
                ViewModel.AddStartup(Properties.Settings.Default.RunOnStartupMinimized);
            else
                ViewModel.RemoveStartup();
        }
    }
}

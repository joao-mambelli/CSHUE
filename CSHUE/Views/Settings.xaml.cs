using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CSHUE.Cultures;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Settings
    {
        public SettingsViewModel ViewModel = new SettingsViewModel();

        public Settings()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;

            ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_SelectionChanged;
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

                Properties.Settings.Default.Language = Helpers.Converters.GetIndexFromCultureInfo(culture);
            }
            else
            {
                var culture = Helpers.Converters.GetCultureInfoFromIndex(((ComboBox)sender).SelectedIndex);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private async void Default_Click(object sender, RoutedEventArgs e)
        {
            Window messageBox = new CustomMessageBox
            {
                Yes = Cultures.Resources.Yes,
                No = Cultures.Resources.No,
                Message = Cultures.Resources.AreYouSure,
                Owner = Window.GetWindow(this)
            };
            messageBox.ShowDialog();

            if (messageBox.DialogResult != true) return;

            ComboBoxLanguage.SelectionChanged -= ComboBoxLanguage_SelectionChanged;
            Properties.Settings.Default.Reset();
            ViewModel.MainWindowViewModel.ConfigPage.ViewModel.CheckConfigFile();
            await MainWindowViewModel.SetDefaultLightsSettings();

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}

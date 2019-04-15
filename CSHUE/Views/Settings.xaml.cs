using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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
        public SettingsViewModel ViewModel = null;
        private readonly SettingsViewModel _viewModel = new SettingsViewModel();

        public Settings()
        {
            InitializeComponent();
            DataContext = _viewModel;

            ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_SelectionChanged;
        }

        private static void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
            {
                CultureResources.ChangeCulture(
                    CultureResources.SupportedCultures.Contains(CultureInfo.InstalledUICulture)
                        ? CultureInfo.InstalledUICulture
                        : new CultureInfo("en-US"));

                Properties.Settings.Default.Language = Helpers.Converters.GetIndexFromCultureInfo(Cultures.Resources.Culture);
            }
            else
                CultureResources.ChangeCulture(Helpers.Converters.GetCultureInfoFromIndex(((ComboBox)sender).SelectedIndex));
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }
    }
}

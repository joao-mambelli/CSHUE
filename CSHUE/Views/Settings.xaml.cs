using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Markup;
using System.Windows.Navigation;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using MaterialDesignThemes.Wpf;
using Xceed.Wpf.Toolkit.Core.Converters;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using MessageBoxResult = System.Windows.MessageBoxResult;

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
                var culture = Helpers.Converters.GetCultureInfoFromIndex(((ComboBox) sender).SelectedIndex);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            Window messageBox = new CustomMessageBox
            {
                Yes = Cultures.Resources.Yes,
                No = Cultures.Resources.No,
                Message = Cultures.Resources.AreYouSure
            };
            messageBox.Owner = Window.GetWindow(this);
            messageBox.ShowDialog();

            if (messageBox.DialogResult != true) return;
            Properties.Settings.Default.Reset();
            ViewModel.MainWindowViewModel.ConfigViewModel.CheckConfigFile();
        }
    }
}

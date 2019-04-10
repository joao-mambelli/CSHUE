using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;
using CSHUE.Views;

namespace CSHUE.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ConfigViewModel configViewModel = null;
        public DonateViewModel donateViewModel = null;
        public AboutViewModel aboutViewModel = null;
        public HomeViewModel homeViewModel = null;
        public SettingsViewModel settingsViewModel = null;

        public void Navigate(Frame page, object sender)
        {
            if (((Grid) sender).Name == "Config")
                page.Navigate(new Config() { viewModel = configViewModel });

            if (((Grid)sender).Name == "Donate")
                page.Navigate(new Donate() { viewModel = donateViewModel });

            if (((Grid)sender).Name == "Home")
                page.Navigate(new Home() { viewModel = homeViewModel });

            if (((Grid)sender).Name == "Settings")
                page.Navigate(new Settings() { viewModel = settingsViewModel });

            if (((Grid)sender).Name == "About")
                page.Navigate(new About() { viewModel = aboutViewModel });
        }

        public void CreateInstances()
        {
            configViewModel = new ConfigViewModel() { mainWindowViewModel = this };
            donateViewModel = new DonateViewModel() { mainWindowViewModel = this };
            homeViewModel = new HomeViewModel() { mainWindowViewModel = this };
            settingsViewModel = new SettingsViewModel() { mainWindowViewModel = this };
            aboutViewModel = new AboutViewModel() { mainWindowViewModel = this };
        }
    }
}

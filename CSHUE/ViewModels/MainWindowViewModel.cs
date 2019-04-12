using System.Windows.Controls;
using CSHUE.Views;

namespace CSHUE.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ConfigViewModel ConfigViewModel;
        public DonateViewModel DonateViewModel;
        public AboutViewModel AboutViewModel;
        public HomeViewModel HomeViewModel;
        public SettingsViewModel SettingsViewModel;

        public void Navigate(Frame page,
            object sender)
        {
            if (((Grid) sender).Name == "Config")
                page.Navigate(new Config
                {
                    ViewModel = ConfigViewModel
                });

            if (((Grid) sender).Name == "Donate")
                page.Navigate(new Donate
                {
                    ViewModel = DonateViewModel
                });

            if (((Grid) sender).Name == "Home")
                page.Navigate(new Home
                {
                    ViewModel = HomeViewModel
                });

            if (((Grid) sender).Name == "Settings")
                page.Navigate(new Settings
                {
                    ViewModel = SettingsViewModel
                });

            if (((Grid) sender).Name == "About")
                page.Navigate(new About
                {
                    ViewModel = AboutViewModel
                });
        }

        public void CreateInstances()
        {
            ConfigViewModel = new ConfigViewModel
            {
                MainWindowViewModel = this
            };
            DonateViewModel = new DonateViewModel
            {
                MainWindowViewModel = this
            };
            HomeViewModel = new HomeViewModel
            {
                MainWindowViewModel = this
            };
            SettingsViewModel = new SettingsViewModel
            {
                MainWindowViewModel = this
            };
            AboutViewModel = new AboutViewModel
            {
                MainWindowViewModel = this
            };
        }
    }
}

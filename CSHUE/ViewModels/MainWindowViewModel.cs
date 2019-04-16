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

        public void Navigate(Frame page, object sender)
        {
            switch (((Grid)sender).Name)
            {
                case "Config":
                    page.Navigate(new Config
                    {
                        ViewModel = ConfigViewModel
                    });
                    break;
                case "Donate":
                    page.Navigate(new Donate
                    {
                        ViewModel = DonateViewModel
                    });
                    break;
                case "Settings":
                    page.Navigate(new Settings
                    {
                        ViewModel = SettingsViewModel
                    });
                    break;
                case "About":
                    page.Navigate(new About
                    {
                        ViewModel = AboutViewModel
                    });
                    break;
                default:
                    page.Navigate(new Home
                    {
                        ViewModel = HomeViewModel
                    });
                    break;
            }
        }

        public void Navigate(Frame page, string pageName)
        {
            switch (pageName)
            {
                case "Config":
                    page.Navigate(new Config
                    {
                        ViewModel = ConfigViewModel
                    });
                    break;
                case "Donate":
                    page.Navigate(new Donate
                    {
                        ViewModel = DonateViewModel
                    });
                    break;
                case "Settings":
                    page.Navigate(new Settings
                    {
                        ViewModel = SettingsViewModel
                    });
                    break;
                case "About":
                    page.Navigate(new About
                    {
                        ViewModel = AboutViewModel
                    });
                    break;
                default:
                    page.Navigate(new Home
                    {
                        ViewModel = HomeViewModel
                    });
                    break;
            }
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

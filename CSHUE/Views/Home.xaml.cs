using System.Linq;
using System.Windows;
using CSHUE.ViewModels;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {
        public HomeViewModel ViewModel = new HomeViewModel();

        public Home()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainWindowViewModel.HueAsync();
        }
    }
}

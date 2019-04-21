using System.Linq;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Home
    {
        public HomeViewModel ViewModel = new HomeViewModel();

        public Home()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }
    }
}

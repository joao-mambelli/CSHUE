using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using CSHUE.ViewModels;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public AboutViewModel ViewModel = new AboutViewModel();

        public About()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        private void Hyperlink_RequestNavigate(object sender,
            RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

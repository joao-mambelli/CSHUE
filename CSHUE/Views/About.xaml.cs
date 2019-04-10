using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        public AboutViewModel viewModel = null;

        public About()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

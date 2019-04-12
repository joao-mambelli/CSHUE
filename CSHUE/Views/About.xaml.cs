using System.Diagnostics;
using System.Windows.Navigation;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class About
    {
        public AboutViewModel ViewModel = null;

        public About()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Hyperlink_RequestNavigate(object sender,
            RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

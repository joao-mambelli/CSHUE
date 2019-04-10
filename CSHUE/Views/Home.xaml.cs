using System.Windows.Controls;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public HomeViewModel viewModel = null;

        public Home()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

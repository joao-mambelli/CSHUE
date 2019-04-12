using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Home
    {
        public HomeViewModel ViewModel = null;

        public Home()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

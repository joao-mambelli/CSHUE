using System.Text.RegularExpressions;
using System.Windows.Input;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Settings
    {
        public SettingsViewModel ViewModel = null;

        public Settings()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

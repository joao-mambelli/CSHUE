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

        private void AcceptsNumber(object sender,
            TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

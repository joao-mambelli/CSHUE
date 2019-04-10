using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public SettingsViewModel viewModel = null;

        public Settings()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void AcceptsNumber(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

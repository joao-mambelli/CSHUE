using System;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config
    {
        public ConfigViewModel ViewModel = new ConfigViewModel();

        public Config()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        private void Config_Click(object sender,
            EventArgs e)
        {
            ViewModel.CreateConfigFile();
        }
    }
}

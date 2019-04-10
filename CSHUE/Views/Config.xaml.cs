using System;
using System.Windows.Controls;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        public ConfigViewModel viewModel = null;

        public Config()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Config_Click(object sender, EventArgs e)
        {
            viewModel.CreateConfigFile();
        }
    }
}

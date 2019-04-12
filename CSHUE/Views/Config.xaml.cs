using System;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Config
    {
        public ConfigViewModel ViewModel = null;

        public Config()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Config_Click(object sender,
            EventArgs e)
        {
            ViewModel.CreateConfigFile();
        }
    }
}

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
        #region Fields

        public ConfigViewModel ViewModel = new ConfigViewModel();

        #endregion

        #region Initializers

        public Config()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        #endregion

        #region Events Handlers

        private void CreateCfg_OnClick(object sender, EventArgs e)
        {
            ViewModel.CreateConfigFile();
        }

        private void ChangeFolder_OnClick(object sender, EventArgs e)
        {
            ViewModel.ChangeFolder();
        }

        #endregion
    }
}

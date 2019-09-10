using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help
    {
        #region Fields

        /// <summary>
        /// ViewModel field.
        /// </summary>
        public HelpViewModel ViewModel = new HelpViewModel();

        #endregion

        #region Initializers

        /// <summary>
        /// Help initializer.
        /// </summary>
        public Help()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        #endregion

        #region Events Handlers
        
        /// <summary>
        /// Report button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Report_OnClick(object sender, EventArgs e)
        {
            Process.Start("https://github.com/joao7yt/CSHUE/issues/new");
        }

        #endregion
    }
}

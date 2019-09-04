using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        #region Fields

        /// <summary>
        /// ViewModel field.
        /// </summary>
        public AboutViewModel ViewModel = new AboutViewModel();

        #endregion

        #region Initializers

        /// <summary>
        /// About initializer.
        /// </summary>
        public About()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        #endregion

        #region Events Handlers

        /// <summary>
        /// Hyperlink request navigation event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

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

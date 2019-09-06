using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update
    {
        #region Fields

        /// <summary>
        /// ViewModel field.
        /// </summary>
        public UpdateViewModel ViewModel = new UpdateViewModel();

        #endregion

        #region Initializers

        /// <summary>
        /// Update initializer.
        /// </summary>
        public Update()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;

            ViewModel.Link = $"https://github.com/joao7yt/CSHUE/releases/tag/{Properties.Settings.Default.LatestVersionCheck}";
        }

        #endregion

        #region Events Handlers

        /// <summary>
        /// ShowInBrowser button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowInBrowser_OnClick(object sender, EventArgs e)
        {
            Process.Start($"https://github.com/joao7yt/CSHUE/releases/tag/{Properties.Settings.Default.LatestVersionCheck}");
        }

        /// <summary>
        /// CheckUpdate button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdate_OnClick(object sender, EventArgs e)
        {
            ViewModel.AllowCheck = false;

            new Thread(() =>
                {
                    Properties.Settings.Default.LatestVersionCheck = ViewModel.MainWindowViewModel.GetLastVersion();

                    ViewModel.AllowCheck = true;

                    ViewModel.WarningUpdateVisibility = ViewModel.MainWindowViewModel.WarningUpdateVisibility;
                })
            { IsBackground = true }.Start();
        }

        #endregion
    }
}

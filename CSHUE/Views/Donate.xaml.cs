using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Donate.xaml
    /// </summary>
    public partial class Donate
    {
        #region Fields

        /// <summary>
        /// ViewModel field.
        /// </summary>
        public DonateViewModel ViewModel = new DonateViewModel();

        #endregion

        #region Initializers

        /// <summary>
        /// Donate initializer.
        /// </summary>
        public Donate()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        #endregion

        #region Events Handlers

        /// <summary>
        /// Donate button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Donate_OnClick(object sender, EventArgs e)
        {
            var url = "";
            
            url += "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=joao7yt@gmail.com&lc=" +
                   Cultures.Resources.CountryIso2 +
                   "&item_name=CSHUE&currency_code=USD&bn=PP%2dDonationsBF";

            Process.Start(url);
        }

        #endregion
    }
}

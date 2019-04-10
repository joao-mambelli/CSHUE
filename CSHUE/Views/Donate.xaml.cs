using System;
using System.Windows.Controls;
using System.Diagnostics;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Donate.xaml
    /// </summary>
    public partial class Donate : Page
    {
        public DonateViewModel viewModel = null;

        public Donate()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Donate_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "joao7yt@gmail.com";  // your paypal email
            string description = "CSHUE%20Donation";            // '%20' represents a space. remember HTML!
            string country = "US";                  // AU, US, etc.
            string currency = "USD";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            Process.Start(url);
        }
    }
}

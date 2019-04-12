using System;
using System.Diagnostics;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Donate.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class Donate
    {
        public DonateViewModel ViewModel = null;

        public Donate()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Donate_Click(object sender,
            EventArgs e)
        {
            var url = "";

            const string business = "joao7yt@gmail.com";
            const string description = "CSHUE%20Donation";
            const string country = "US";
            const string currency = "USD";

            url += "https://www.paypal.com/cgi-bin/webscr" +
                   "?cmd=" +
                   "_donations" +
                   "&business=" +
                   business +
                   "&lc=" +
                   country +
                   "&item_name=" +
                   description +
                   "&currency_code=" +
                   currency +
                   "&bn=" +
                   "PP%2dDonationsBF";

            Process.Start(url);
        }
    }
}

using System.Linq;
using System.Threading;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home
    {
        #region Fields

        public HomeViewModel ViewModel = new HomeViewModel();

        #endregion

        #region Initializers

        public Home()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;
        }

        public void StartLightsChecking()
        {
            new Thread(async () =>
                {
                    while (!ViewModel.MainWindowViewModel.WindowMinimized)
                    {
                        await ViewModel.RefreshLights();

                        Thread.Sleep(500);
                    }
                })
                { IsBackground = true }.Start();
        }

        #endregion

        #region Events Handlers

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainWindowViewModel.HueAsync();
        }

        private void RunCsgo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainWindowViewModel.RunCsgo();
        }

        #endregion
    }
}

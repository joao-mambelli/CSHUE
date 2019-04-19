using System.Windows;

namespace CSHUE.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;
        
        private Visibility _loadingVisibility = Visibility.Hidden;
        public Visibility LoadingVisibility
        {
            get =>
                _loadingVisibility;
            set
            {
                _loadingVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningNoHub = Visibility.Collapsed;
        public Visibility WarningNoHub
        {
            get =>
                _warningNoHub;
            set
            {
                _warningNoHub = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningLink = Visibility.Collapsed;
        public Visibility WarningLink
        {
            get =>
                _warningLink;
            set
            {
                _warningLink = value;
                OnPropertyChanged();
            }
        }
    }
}

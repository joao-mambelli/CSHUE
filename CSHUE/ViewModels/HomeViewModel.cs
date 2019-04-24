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

        private bool _isLoading;
        public bool IsLoading
        {
            get =>
                _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private Visibility _retryVisibility = Visibility.Hidden;
        public Visibility RetryVisibility
        {
            get =>
                _retryVisibility;
            set
            {
                _retryVisibility = value;
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

        private Visibility _warningValidating = Visibility.Collapsed;
        public Visibility WarningValidating
        {
            get =>
                _warningValidating;
            set
            {
                _warningValidating = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningNoReachableHubs = Visibility.Collapsed;
        public Visibility WarningNoReachableHubs
        {
            get =>
                _warningNoReachableHubs;
            set
            {
                _warningNoReachableHubs = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningHubNotAvailable = Visibility.Collapsed;
        public Visibility WarningHubNotAvailable
        {
            get =>
                _warningHubNotAvailable;
            set
            {
                _warningHubNotAvailable = value;
                OnPropertyChanged();
            }
        }

        public void SetWarningNoHub()
        {
            WarningNoHub = Visibility.Visible;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningLink()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Visible;
            WarningValidating = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningValidating()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Visible;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningNoReachableHubs()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Visible;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningHubNotAvailable()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Visible;

            SetRetry();
        }

        public void SetLoading()
        {
            IsLoading = true;
            LoadingVisibility = Visibility.Visible;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcess = Visibility.Visible;
            InProcess = Visibility.Visible;
        }

        public void SetRetry()
        {
            IsLoading = false;
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Visible;
            MainWindowViewModel.InProcess = Visibility.Visible;
            InProcess = Visibility.Visible;
        }

        public void SetDone()
        {
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcess = Visibility.Collapsed;
            InProcess = Visibility.Collapsed;
        }
    }
}

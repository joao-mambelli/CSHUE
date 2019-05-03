using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CSHUE.Cultures;
using CSHUE.Helpers;
using CSHUE.Views;

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

        private LoadingSpinner.SpinnerStates _state;
        public LoadingSpinner.SpinnerStates State
        {
            get =>
                _state;
            set
            {
                _state = value;
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

        private Visibility _warningSearching = Visibility.Collapsed;
        public Visibility WarningSearching
        {
            get =>
                _warningSearching;
            set
            {
                _warningSearching = value;
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
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningLink()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Visible;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningValidating()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Visible;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningSearching()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Visible;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningNoReachableHubs()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Visible;
            WarningHubNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningHubNotAvailable()
        {
            WarningNoHub = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableHubs = Visibility.Collapsed;
            WarningHubNotAvailable = Visibility.Visible;

            SetRetry();
        }

        public void SetLoading()
        {
            State = LoadingSpinner.SpinnerStates.Loading;
            LoadingVisibility = Visibility.Visible;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcess = Visibility.Visible;
            InProcess = Visibility.Visible;
        }

        public void SetRetry()
        {
            State = LoadingSpinner.SpinnerStates.Hanging;
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Visible;
            MainWindowViewModel.InProcess = Visibility.Visible;
            InProcess = Visibility.Visible;
        }

        public void SetDone()
        {
            State = LoadingSpinner.SpinnerStates.Disabled;
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcess = Visibility.Collapsed;
            InProcess = Visibility.Collapsed;
        }

        private List<LightStateViewModel> _list;
        public List<LightStateViewModel> List
        {
            get =>
                _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        public async Task RefreshLights()
        {
            if (MainWindowViewModel.Client == null) return;

            List = new List<LightStateViewModel>();

            var allLights = (await MainWindowViewModel.Client.GetLightsAsync()).ToList();

            foreach (var l in allLights)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (l.State.Hue != null && l.State.Saturation != null)
                        List.Add(new LightStateViewModel
                        {
                            Content = new LightStateCell
                            {
                                On = l.State.On,
                                Text = l.State.On ? l.Name : l.Name + " (" + Resources.LightOff + ")",
                                Color = l.State.On
                                    ? ColorConverters.Hs((double) l.State.Hue / 65535 * Math.PI * 2, 
                                        (double) l.State.Saturation / 255)
                                    : Colors.Black,
                                Brightness = (double)(l.State.Brightness + 1) / 255
                            }
                        });
                });
            }
        }
    }
}

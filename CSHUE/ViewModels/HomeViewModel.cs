using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CSHUE.Controls;
using CSHUE.Core.Utilities;
using CSHUE.Cultures;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties

        private Visibility _loadingVisibility = Visibility.Hidden;
        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set
            {
                _loadingVisibility = value;
                OnPropertyChanged();
            }
        }

        private LoadingSpinner.SpinnerState _state;
        public LoadingSpinner.SpinnerState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        private Visibility _retryVisibility = Visibility.Hidden;
        public Visibility RetryVisibility
        {
            get => _retryVisibility;
            set
            {
                _retryVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningNoBridge = Visibility.Collapsed;
        public Visibility WarningNoBridge
        {
            get => _warningNoBridge;
            set
            {
                _warningNoBridge = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningLink = Visibility.Collapsed;
        public Visibility WarningLink
        {
            get => _warningLink;
            set
            {
                _warningLink = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningValidating = Visibility.Collapsed;
        public Visibility WarningValidating
        {
            get => _warningValidating;
            set
            {
                _warningValidating = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningSearching = Visibility.Collapsed;
        public Visibility WarningSearching
        {
            get => _warningSearching;
            set
            {
                _warningSearching = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningNoReachableBridges = Visibility.Collapsed;
        public Visibility WarningNoReachableBridges
        {
            get => _warningNoReachableBridges;
            set
            {
                _warningNoReachableBridges = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningBridgeNotAvailable = Visibility.Collapsed;
        public Visibility WarningBridgeNotAvailable
        {
            get => _warningBridgeNotAvailable;
            set
            {
                _warningBridgeNotAvailable = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<LightStateCell> _list = new ObservableCollection<LightStateCell>();
        public ObservableCollection<LightStateCell> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Fields

        public MainWindowViewModel MainWindowViewModel = null;

        #endregion

        #region Methods

        public void SetWarningNoBridge()
        {
            WarningNoBridge = Visibility.Visible;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableBridges = Visibility.Collapsed;
            WarningBridgeNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningLink()
        {
            WarningNoBridge = Visibility.Collapsed;
            WarningLink = Visibility.Visible;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableBridges = Visibility.Collapsed;
            WarningBridgeNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningValidating()
        {
            WarningNoBridge = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Visible;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableBridges = Visibility.Collapsed;
            WarningBridgeNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningSearching()
        {
            WarningNoBridge = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Visible;
            WarningNoReachableBridges = Visibility.Collapsed;
            WarningBridgeNotAvailable = Visibility.Collapsed;

            SetLoading();
        }

        public void SetWarningNoReachableBridges()
        {
            WarningNoBridge = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableBridges = Visibility.Visible;
            WarningBridgeNotAvailable = Visibility.Collapsed;

            SetRetry();
        }

        public void SetWarningBridgeNotAvailable()
        {
            WarningNoBridge = Visibility.Collapsed;
            WarningLink = Visibility.Collapsed;
            WarningValidating = Visibility.Collapsed;
            WarningSearching = Visibility.Collapsed;
            WarningNoReachableBridges = Visibility.Collapsed;
            WarningBridgeNotAvailable = Visibility.Visible;

            SetRetry();
        }

        public void SetLoading()
        {
            State = LoadingSpinner.SpinnerState.Loading;
            LoadingVisibility = Visibility.Visible;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcessVisibility = Visibility.Visible;
            InProcessVisibility = Visibility.Visible;
        }

        public void SetRetry()
        {
            State = LoadingSpinner.SpinnerState.Hanging;
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Visible;
            MainWindowViewModel.InProcessVisibility = Visibility.Visible;
            InProcessVisibility = Visibility.Visible;
        }

        public void SetDone()
        {
            List = new ObservableCollection<LightStateCell>();

            State = LoadingSpinner.SpinnerState.Disabled;
            LoadingVisibility = Visibility.Collapsed;
            RetryVisibility = Visibility.Collapsed;
            MainWindowViewModel.InProcessVisibility = Visibility.Collapsed;
            InProcessVisibility = Visibility.Collapsed;
        }
        
        public async Task RefreshLights()
        {
            if (MainWindowViewModel.Client == null)
                return;

            var tempList = List.ToList();

            List<Light> allLights;

            try
            {
                allLights = (await MainWindowViewModel.Client.GetLightsAsync()).ToList();
            }
            catch
            {
                SetWarningValidating();
                return;
            }

            if (State != LoadingSpinner.SpinnerState.Disabled)
                SetDone();

            foreach (var l in allLights)
            {
                Application.Current.Dispatcher?.Invoke(delegate
                {
                    if (l.State.Hue == null || l.State.Saturation == null)
                        return;

                    var existingElement = tempList.Find(x => x.UniqueId == l.UniqueId);

                    if (l.State.IsReachable == null || l.State.ColorTemperature == null)
                        return;

                    if (existingElement != null)
                    {
                        existingElement.On = l.State.On && l.State.IsReachable.Value;
                        existingElement.Text = l.State.On && l.State.IsReachable.Value
                            ? l.Name
                            : l.Name + " (" + (!l.State.IsReachable.Value
                                  ? Resources.UnreachableState
                                  : Resources.OffLightState) + ")";
                        existingElement.Color = l.State.On && l.State.IsReachable.Value
                            ? l.Capabilities.Control.ColorGamut == null
                                ? ColorConverters.HueSaturation((double) l.State.Hue / 65535 * Math.PI * 2,
                                    (double) l.State.Saturation / 255)
                                : ColorConverters.ColorTemperatue(
                                    (int) Math.Round((l.State.ColorTemperature.Value - 654.222) / -0.077111))
                            : Colors.Black;
                        existingElement.Brightness = (double) (l.State.Brightness + 1) / 255;
                    }
                    else
                    {
                        tempList.Add(new LightStateCell
                        {
                            On = l.State.On && l.State.IsReachable.Value,
                            Text = l.State.On && l.State.IsReachable.Value
                                ? l.Name
                                : l.Name + " (" + (!l.State.IsReachable.Value
                                      ? Resources.UnreachableState
                                      : Resources.OffLightState) + ")",
                            Color = l.State.On && l.State.IsReachable.Value
                                ? l.Capabilities.Control.ColorGamut == null
                                    ? ColorConverters.HueSaturation((double) l.State.Hue / 65535 * Math.PI * 2,
                                        (double) l.State.Saturation / 255)
                                    : ColorConverters.ColorTemperatue(
                                        (int) Math.Round((l.State.ColorTemperature.Value - 654.222) / -0.077111))
                                : Colors.Black,
                            Brightness = (double) (l.State.Brightness + 1) / 255,
                            UniqueId = l.UniqueId
                        });
                    }
                });
            }

            List = new ObservableCollection<LightStateCell>(tempList);
        }

        #endregion
    }
}

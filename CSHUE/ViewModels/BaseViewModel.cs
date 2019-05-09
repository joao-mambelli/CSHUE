using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CSHUE.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        public Visibility WarningVisibility
        {
            get
            {
                if (WarningCsgoVisibility == Visibility.Visible
                    || WarningSteamVisibility == Visibility.Visible
                    || WarningGsiVisibility == Visibility.Visible
                    || WarningGsiCorruptedVisibility == Visibility.Visible)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        private Visibility _warningCsgoVisibility;
        public Visibility WarningCsgoVisibility
        {
            get =>
                _warningCsgoVisibility;
            set
            {
                _warningCsgoVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningSteamVisibility;
        public Visibility WarningSteamVisibility
        {
            get =>
                _warningSteamVisibility;
            set
            {
                _warningSteamVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningGsiVisibility;
        public Visibility WarningGsiVisibility
        {
            get =>
                _warningGsiVisibility;
            set
            {
                _warningGsiVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningGsiCorruptedVisibility;
        public Visibility WarningGsiCorruptedVisibility
        {
            get =>
                _warningGsiCorruptedVisibility;
            set
            {
                _warningGsiCorruptedVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _inProcess = Visibility.Collapsed;
        public Visibility InProcess
        {
            get =>
                _inProcess;
            set
            {
                _inProcess = value;
                OnPropertyChanged();
            }
        }

        private bool _resetting;
        public bool Resetting
        {
            get =>
                _resetting;
            set
            {
                _resetting = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CSHUE.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
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
    }
}

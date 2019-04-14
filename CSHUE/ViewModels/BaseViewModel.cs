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

        private Visibility _warningCSGOVisibility;
        public Visibility WarningCSGOVisibility
        {
            get =>
                _warningCSGOVisibility;
            set
            {
                _warningCSGOVisibility = value;
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

        private Visibility _warningGSIVisibility;
        public Visibility WarningGSIVisibility
        {
            get =>
                _warningGSIVisibility;
            set
            {
                _warningGSIVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _warningGSICorruptedVisibility;
        public Visibility WarningGSICorruptedVisibility
        {
            get =>
                _warningGSICorruptedVisibility;
            set
            {
                _warningGSICorruptedVisibility = value;
                OnPropertyChanged();
            }
        }
    }
}

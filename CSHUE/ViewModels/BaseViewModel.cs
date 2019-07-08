using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CSHUE.ViewModels
{
    /// <summary>
    /// Base class containing base properties.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Property changed handler property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Property changed handler method.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Warning visibility property.
        /// </summary>
        public Visibility WarningVisibility
        {
            get
            {
                if (WarningCsgoVisibility == Visibility.Visible ||
                    WarningSteamVisibility == Visibility.Visible ||
                    WarningGsiVisibility == Visibility.Visible ||
                    WarningGsiCorruptedVisibility == Visibility.Visible)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Warning CSGO visibility back field.
        /// </summary>
        private Visibility _warningCsgoVisibility;
        /// <summary>
        /// Warning CSGO visibility property.
        /// </summary>
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

        /// <summary>
        /// Warning Steam visibility back field.
        /// </summary>
        private Visibility _warningSteamVisibility;
        /// <summary>
        /// Warning Steam visibility property.
        /// </summary>
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

        /// <summary>
        /// Warning GSI visibility back field.
        /// </summary>
        private Visibility _warningGsiVisibility;
        /// <summary>
        /// Warning GSI visibility property.
        /// </summary>
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

        /// <summary>
        /// Warning GSI corrupted visibility back field.
        /// </summary>
        private Visibility _warningGsiCorruptedVisibility;
        /// <summary>
        /// Warning GSI corrupted visibility property.
        /// </summary>
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

        /// <summary>
        /// In process visibility back field.
        /// </summary>
        private Visibility _inProcessVisibility = Visibility.Collapsed;
        /// <summary>
        /// In process visibility property.
        /// </summary>
        public Visibility InProcessVisibility
        {
            get =>
                _inProcessVisibility;
            set
            {
                _inProcessVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}

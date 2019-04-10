using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CSHUE.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Visibility _warningVisibility = Visibility.Visible;
        public Visibility WarningVisibility
        {
            get => _warningVisibility;
            set
            {
                _warningVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _warningText;
        public string WarningText
        {
            get => _warningText;
            set
            {
                _warningText = value;
                OnPropertyChanged();
            }
        }
    }
}

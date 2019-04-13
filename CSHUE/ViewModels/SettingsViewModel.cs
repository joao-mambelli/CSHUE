namespace CSHUE.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        private bool _isRunWhenStartsChecked;
        public bool IsRunWhenStartsChecked
        {
            get =>
                _isRunWhenStartsChecked;
            set
            {
                _isRunWhenStartsChecked = value;
                OnPropertyChanged();
            }
        }
    }
}

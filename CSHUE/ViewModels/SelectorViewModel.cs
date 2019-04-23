namespace CSHUE.ViewModels
{
    public class SelectorViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        public string ContentText { get; set; }

        public string Title { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get =>
                _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public string Ip { get; set; }
    }
}

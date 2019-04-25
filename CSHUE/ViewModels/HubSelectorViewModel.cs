namespace CSHUE.ViewModels
{
    public class HubSelectorViewModel : BaseViewModel
    {
        public string ContentText { get; set; }

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

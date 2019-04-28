using System.Windows;
using CSHUE.Views;

namespace CSHUE.ViewModels
{
    public class LightSelectorViewModel : BaseViewModel
    {
        public ColorChooser Content { get; set; }

        private bool _isChecked;
        private Visibility _singleOptionVisibility = Visibility.Collapsed;

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

        public string GroupName { get; set; } = "";

        public Visibility SingleOptionVisibility
        {
            get => _singleOptionVisibility;
            set
            {
                if (value == Visibility.Visible)
                    GroupName = "Group";

                _singleOptionVisibility = value;
            }
        }

        public string UniqueId { get; set; }
    }
}

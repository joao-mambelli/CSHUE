using System.Windows;
using System.Windows.Media;
using CSHUE.Controls;

namespace CSHUE.ViewModels
{
    public class LightSettingCellViewModel : BaseViewModel
    {
        public LightSettingCell Content { get; set; }

        private Visibility _singleOptionVisibility = Visibility.Collapsed;

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

        public int Index { get; set; }

        public Color Color { get; set; }

        public byte Brightness { get; set; }
    }
}

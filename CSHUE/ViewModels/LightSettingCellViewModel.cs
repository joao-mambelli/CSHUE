using System.Windows;
using System.Windows.Media;
using CSHUE.Controls;

namespace CSHUE.ViewModels
{
    public class LightSettingCellViewModel : BaseViewModel
    {
        #region Properties

        public LightSettingCell Content { get; set; }

        public string GroupName { get; set; } = "";

        public string UniqueId { get; set; }

        public string Index { get; set; }

        private Visibility _singleOptionVisibility = Visibility.Collapsed;
        public Visibility SingleOptionVisibility
        {
            get => _singleOptionVisibility;
            set
            {
                if (value == Visibility.Visible)
                    GroupName = "Group";

                _singleOptionVisibility = value;
                OnPropertyChanged();
            }
        }

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

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private byte _brightness;
        public byte Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private bool _onlyBrightness;
        public bool OnlyBrightness
        {
            get => _onlyBrightness;
            set
            {
                _onlyBrightness = value;
                OnPropertyChanged();
            }
        }

        private Visibility _onlyBrightnessVisibility = Visibility.Collapsed;
        public Visibility OnlyBrightnessVisibility
        {
            get => _onlyBrightnessVisibility;
            set
            {
                _onlyBrightnessVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _mainEventText;
        public string MainEventText
        {
            get => _mainEventText;
            set
            {
                _mainEventText = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}

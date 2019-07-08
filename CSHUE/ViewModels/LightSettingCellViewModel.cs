using System.Windows;
using System.Windows.Media;
using CSHUE.Controls;

namespace CSHUE.ViewModels
{
    public class LightSettingCellViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Content property.
        /// </summary>
        public LightSettingCell Content { get; set; }

        /// <summary>
        /// Group name property.
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// Unique Id property.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Property that indicates if the mode is color temperature or not.
        /// </summary>
        public bool IsColorTemperature { get; set; }

        /// <summary>
        /// Id property.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Single option visibility back field.
        /// </summary>
        private Visibility _singleOptionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Single option visibility property.
        /// </summary>
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

        /// <summary>
        /// Is checked back field.
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// Is checked property.
        /// </summary>
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

        /// <summary>
        /// Color temperature back field.
        /// </summary>
        private int _colorTemperature;
        /// <summary>
        /// Color temperature property.
        /// </summary>
        public int ColorTemperature
        {
            get => _colorTemperature;
            set
            {
                _colorTemperature = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Color back field.
        /// </summary>
        private Color _color;
        /// <summary>
        /// Color property.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Brightness back field.
        /// </summary>
        private byte _brightness;
        /// <summary>
        /// Brightness property.
        /// </summary>
        public byte Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Text back field.
        /// </summary>
        private string _text;
        /// <summary>
        /// Text property.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Only brightness back field.
        /// </summary>
        private bool _onlyBrightness;
        /// <summary>
        /// Only brightness property.
        /// </summary>
        public bool OnlyBrightness
        {
            get => _onlyBrightness;
            set
            {
                _onlyBrightness = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Only brightness visibility back field.
        /// </summary>
        private Visibility _onlyBrightnessVisibility = Visibility.Collapsed;
        /// <summary>
        /// Only brightness visibility property.
        /// </summary>
        public Visibility OnlyBrightnessVisibility
        {
            get => _onlyBrightnessVisibility;
            set
            {
                _onlyBrightnessVisibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Main event back field.
        /// </summary>
        private string _mainEventText;
        /// <summary>
        /// Main event property.
        /// </summary>
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

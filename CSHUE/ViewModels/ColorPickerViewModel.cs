using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Q42.HueApi;
using CSHUE.Helpers;

namespace CSHUE.ViewModels
{
    public class ColorPickerViewModel : BaseViewModel
    {
        #region Properties

        public int SlidersHeight { get; } = 270;
        public int ColorWheelSize { get; } = 236;
        public int OutsideColorWheelSize { get; } = 290;

        private bool _isColorTemperature;
        public bool IsColorTemperature
        {
            get => _isColorTemperature;
            set
            {
                _isColorTemperature = value;
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

        private double _hue;
        public double Hue
        {
            get => _hue;
            set
            {
                if (!DontUpdate)
                {
                    var bitMap = new ColorWheel().CreateSaturationImage(SlidersHeight, value);
                    bitMap.Freeze();
                    SaturationSliderBrush = bitMap;

                    SetMousePositionAndColor(false);
                }

                _hue = value;
                OnPropertyChanged();
            }
        }

        private double _saturation;
        public double Saturation
        {
            get => _saturation;
            set
            {
                if (!DontUpdate)
                {
                    var bitMap = new ColorWheel().CreateHueImage(SlidersHeight, value);
                    bitMap.Freeze();
                    HueSliderBrush = bitMap;

                    SetMousePositionAndColor(false);
                }

                _saturation = value;
                OnPropertyChanged();
            }
        }

        private int _colorTemperature;
        public int ColorTemperature
        {
            get => _colorTemperature;
            set
            {
                if (!DontUpdate)
                {
                    DontUpdate = true;

                    Hue = 0;
                    Saturation = ((double) value - 4250) * 2 / 45;

                    DontUpdate = false;
                }

                SetMousePositionAndColor(true);

                _colorTemperature = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap _saturationSliderBrush;
        public WriteableBitmap SaturationSliderBrush
        {
            get => _saturationSliderBrush;
            set
            {
                _saturationSliderBrush = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap _temperatureSliderBrush;
        public WriteableBitmap TemperatureSliderBrush
        {
            get => _temperatureSliderBrush;
            set
            {
                _temperatureSliderBrush = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap _hueSliderBrush;
        public WriteableBitmap HueSliderBrush
        {
            get => _hueSliderBrush;
            set
            {
                _hueSliderBrush = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap _outsideColorWheelBrush;
        public WriteableBitmap OutsideColorWheelBrush
        {
            get => _outsideColorWheelBrush;
            set
            {
                _outsideColorWheelBrush = value;
                OnPropertyChanged();
            }
        }

        private WriteableBitmap _colorWheelBrush;
        public WriteableBitmap ColorWheelBrush
        {
            get => _colorWheelBrush;
            set
            {
                _colorWheelBrush = value;
                OnPropertyChanged();
            }
        }

        private Thickness _mousePosition;
        public Thickness MousePosition
        {
            get => _mousePosition;
            set
            {
                _mousePosition = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Fields

        public bool MovingPicker;
        public bool DontUpdate;

        #endregion

        #region Methods

        public async void SetLightAsync(byte brightness, string id, bool colorTemperature)
        {
            try
            {
                if (colorTemperature)
                {
                    await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                    {
                        On = true,
                        ColorTemperature = (int) Math.Round(ColorTemperature * -0.077111 + 654.222),
                        Brightness = brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(400)
                    }, new List<string> { $"{id}" }).ConfigureAwait(false);
                }
                else
                {
                    await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                    {
                        On = true,
                        Hue = (int) Math.Round(Hue / 360 * 65535),
                        Saturation = (byte) Math.Round(Saturation / 100 * 255),
                        Brightness = brightness,
                        TransitionTime = TimeSpan.FromMilliseconds(400)
                    }, new List<string> { $"{id}" }).ConfigureAwait(false);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void ChangeHueSaturation(Point colorWheelCenterRelativeMousePosition, bool approximate)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) +
                                               Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));
            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.Y, colorWheelCenterRelativeMousePosition.X) +
                        Math.PI / 2;

            if (angle < 0) angle += 2 * Math.PI;
            Hue = (int) Math.Round(approximate
                ? Math.Round(angle / ((double)1 / 18 * Math.PI)) * ((double)1 / 18 * Math.PI) / (2 * Math.PI) * 360
                : angle / (2 * Math.PI) * 360);
            Saturation = distanceFromCenter < (double) ColorWheelSize / 2
                ? (int) Math.Round(distanceFromCenter / ((double) ColorWheelSize / 2) * 100)
                : 100;

            MousePosition = distanceFromCenter < (double) ColorWheelSize / 2
                ? new Thickness(colorWheelCenterRelativeMousePosition.X * 2,
                    colorWheelCenterRelativeMousePosition.Y * 2, 0, 0)
                : new Thickness(ColorWheelSize * Math.Sin(Hue / 360 * Math.PI * 2) * (Saturation / 100), 0, 0,
                    ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100));
            Color = ColorConverters.HueSaturation(Hue / 360 * 2 * Math.PI, Saturation / 100);
        }

        public void ChangeTemperature(Point colorWheelCenterRelativeMousePosition)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) +
                                               Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));

            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.Y, colorWheelCenterRelativeMousePosition.X) +
                        Math.PI / 2;

            DontUpdate = true;

            if (angle < 0) angle += 2 * Math.PI;
            Hue = (int) Math.Round(angle / (2 * Math.PI) * 360);
            Saturation = distanceFromCenter < (double) ColorWheelSize / 2
                ? (int) Math.Round(distanceFromCenter / ((double) ColorWheelSize / 2) * 100)
                : 100;

            if (distanceFromCenter < (double) ColorWheelSize / 2)
                ColorTemperature =
                    (int) Math.Round(
                        (1 - (colorWheelCenterRelativeMousePosition.Y + (double) ColorWheelSize / 2) / ColorWheelSize) *
                        4500 + 2000);
            else
                ColorTemperature =
                    (int) Math.Round(
                        (ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100) / 2 +
                         (double) ColorWheelSize / 2) / ColorWheelSize * 4500 + 2000);

            DontUpdate = false;

            MousePosition = distanceFromCenter < (double) ColorWheelSize / 2
                ? new Thickness(colorWheelCenterRelativeMousePosition.X * 2,
                    colorWheelCenterRelativeMousePosition.Y * 2, 0, 0)
                : new Thickness(ColorWheelSize * Math.Sin(Hue / 360 * Math.PI * 2) * (Saturation / 100), 0, 0,
                    ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100));
            Color = ColorConverters.ColorTemperatue(ColorTemperature);
        }

        private void SetMousePositionAndColor(bool colorTemperature)
        {
            if (MovingPicker)
                return;

            MousePosition = new Thickness(
                ColorWheelSize * Math.Sin(Hue / 360 * Math.PI * 2) * (Saturation / 100), 0,
                0, ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100));

            Color = colorTemperature
                ? ColorConverters.ColorTemperatue(ColorTemperature)
                : ColorConverters.HueSaturation(Hue / 360 * 2 * Math.PI, Saturation / 100);
        }

        #endregion
    }
}

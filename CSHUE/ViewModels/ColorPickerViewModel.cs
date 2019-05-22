using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Q42.HueApi;
using CSHUE.Helpers;
// ReSharper disable PossibleLossOfFraction

namespace CSHUE.ViewModels
{
    public class ColorPickerViewModel : BaseViewModel
    {
        #region Properties

        public int SlidersHeight { get; } = 270;
        public int ColorWheelSize { get; } = 236;
        public int OutsideColorWheelSize { get; } = 290;

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
                var bitMap = new ColorWheel().CreateSaturationImage(SlidersHeight, value);
                bitMap.Freeze();
                SaturationSliderBrush = bitMap;

                SetMousePositionAndColor();

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
                var bitMap = new ColorWheel().CreateHueImage(SlidersHeight, value);
                bitMap.Freeze();
                HueSliderBrush = bitMap;

                SetMousePositionAndColor();

                _saturation = value;
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

        #endregion

        #region Methods

        public async void SetLightAsync(byte brightness, int index)
        {
            if (await MainWindowViewModel.Client.CheckConnection())
                await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                {
                    On = true,
                    Hue = (int)Math.Round(Hue / 360 * 65535),
                    Saturation = (byte)Math.Round(Saturation / 100 * 255),
                    Brightness = brightness,
                    TransitionTime = TimeSpan.FromMilliseconds(400)
                }, new List<string> { $"{index}" }).ConfigureAwait(false);
        }

        public void ChangeHueSaturation(Point colorWheelCenterRelativeMousePosition, bool approximate)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) +
                                               Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));
            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.Y, colorWheelCenterRelativeMousePosition.X) +
                        Math.PI / 2;

            if (angle < 0) angle += 2 * Math.PI;
            Hue = (int)Math.Round(approximate
                ? Math.Round(angle / ((double)1 / 18 * Math.PI)) * ((double)1 / 18 * Math.PI) / (2 * Math.PI) * 360
                : angle / (2 * Math.PI) * 360);
            Saturation = distanceFromCenter < ColorWheelSize / 2
                ? (int)Math.Round(distanceFromCenter / (ColorWheelSize / 2) * 100)
                : 100;

            MousePosition = distanceFromCenter < ColorWheelSize / 2
                ? new Thickness(colorWheelCenterRelativeMousePosition.X * 2,
                    colorWheelCenterRelativeMousePosition.Y * 2, 0, 0)
                : new Thickness(ColorWheelSize * Math.Sin(Hue / 360 * Math.PI * 2) * (Saturation / 100), 0, 0,
                    ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100));
            Color = ColorConverters.Hs(Hue / 360 * 2 * Math.PI, Saturation / 100);
        }

        private void SetMousePositionAndColor()
        {
            if (MovingPicker) return;

            MousePosition = new Thickness(
                ColorWheelSize * Math.Sin(Hue / 360 * Math.PI * 2) * (Saturation / 100), 0,
                0, ColorWheelSize * Math.Cos(Hue / 360 * Math.PI * 2) * (Saturation / 100));
            Color = ColorConverters.Hs(Hue / 360 * 2 * Math.PI, Saturation / 100);
        }

        #endregion
    }
}

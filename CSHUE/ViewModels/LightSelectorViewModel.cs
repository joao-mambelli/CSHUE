using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CSHUE.Views;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class LightSelectorViewModel : BaseViewModel
    {
        public bool IsColorPickerOpened { get; set; }

        public LightSettingCell Content { get; set; }

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

        public int Index { get; set; }

        public async void SetLightsAsync(List<LightSelectorViewModel> list)
        {
            var i = 1;
            foreach (var l in list)
            {
                if (!l.IsChecked)
                {
                    await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                    {
                        On = false
                    }, new List<string> { $"{i++}" }).ConfigureAwait(false);
                }
                else
                {
                    await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                    {
                        On = true,
                        Hue = (int)Math.Round(GetHue(l.Content.Color) / 360 * 65535),
                        Saturation = GetSaturation(l.Content.Color),
                        Brightness = l.Content.Brightness
                    }, new List<string> { $"{i++}" }).ConfigureAwait(false);
                }
            }
        }

        public float GetHue(Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();

        public int GetSaturation(Color color)
        {
            var r = (double)color.R / 255;
            var g = (double)color.G / 255;
            var b = (double)color.B / 255;

            var max = r;

            if (g > max) max = g;
            if (b > max) max = b;

            var min = r;

            if (g < min) min = g;
            if (b < min) min = b;

            var delta = max - min;

            return (int)Math.Round(delta / max * 255);
        }
    }
}

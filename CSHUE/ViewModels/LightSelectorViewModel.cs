using System;
using System.Collections.Generic;
using System.Windows;
using CSHUE.Helpers;
using CSHUE.Controls;
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
                        Hue = (int)Math.Round(ColorConverters.GetHue(l.Content.Color) / 360 * 65535),
                        Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Content.Color) * 255),
                        Brightness = l.Content.Brightness
                    }, new List<string> { $"{i++}" }).ConfigureAwait(false);
                }
            }
        }
    }
}

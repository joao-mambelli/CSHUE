using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CSHUE.Helpers;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class LightSelectorViewModel : BaseViewModel
    {
        #region Properties

        public bool IsColorPickerOpened { get; set; }

        private ObservableCollection<LightSettingCellViewModel> _list;
        public ObservableCollection<LightSettingCellViewModel> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public async void SetLightsAsync()
        {
            foreach (var l in List)
            {
                if (!l.IsChecked)
                {
                    if (await MainWindowViewModel.Client.CheckConnection())
                        await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                        {
                            On = false
                        }, new List<string> { $"{l.Index}" }).ConfigureAwait(false);
                }
                else
                {
                    if (await MainWindowViewModel.Client.CheckConnection())
                        await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(l.Color) / 360 * 65535),
                            Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                            Brightness = l.Brightness
                        }, new List<string> { $"{l.Index}" }).ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}

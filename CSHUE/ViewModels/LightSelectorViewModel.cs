using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
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

        private List<bool> _lastListIsChecked = new List<bool>();
        private List<bool> _lastListOnlyBrightness = new List<bool>();
        private List<byte> _lastListBrightness = new List<byte>();
        private List<Color> _lastListColor = new List<Color>();

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
                try
                {
                    if (l.IsChecked != _lastListIsChecked.ElementAt(List.IndexOf(l)) ||
                        l.OnlyBrightness != _lastListOnlyBrightness.ElementAt(List.IndexOf(l)) ||
                        l.Brightness != _lastListBrightness.ElementAt(List.IndexOf(l)) ||
                        l.Color != _lastListColor.ElementAt(List.IndexOf(l)))
                        break;

                    if (l == List.Last())
                        return;
                }
                catch
                {
                    break;
                }
            }

            _lastListIsChecked = new List<bool>(List.Select(x => x.IsChecked).ToList());
            _lastListOnlyBrightness = new List<bool>(List.Select(x => x.OnlyBrightness).ToList());
            _lastListBrightness = new List<byte>(List.Select(x => x.Brightness).ToList());
            _lastListColor = new List<Color>(List.Select(x => x.Color).ToList());

            foreach (var l in List)
            {
                if (!l.IsChecked)
                {
                    try
                    {
                        await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                        {
                            On = false
                        }, new List<string> { $"{l.Index}" }).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    try
                    {
                        await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                        {
                            On = true,
                            Hue = (int)Math.Round(ColorConverters.GetHue(l.Color) / 360 * 65535),
                            Saturation = (byte)Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                            Brightness = l.Brightness
                        }, new List<string> { $"{l.Index}" }).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        #endregion
    }
}

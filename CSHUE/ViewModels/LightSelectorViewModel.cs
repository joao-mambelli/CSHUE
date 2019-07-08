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

        /// <summary>
        /// Property that tells if the color picker is opened.
        /// </summary>
        public bool IsColorPickerOpened { get; set; }

        /// <summary>
        /// List back field.
        /// </summary>
        private ObservableCollection<LightSettingCellViewModel> _list;
        /// <summary>
        /// List property.
        /// </summary>
        public ObservableCollection<LightSettingCellViewModel> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Title back field.
        /// </summary>
        private string _title;
        /// <summary>
        /// Title property.
        /// </summary>
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

        #region Fields

        // Fields that store last states.
        private List<bool> _lastListIsChecked = new List<bool>();
        private List<bool> _lastListOnlyBrightness = new List<bool>();
        private List<byte> _lastListBrightness = new List<byte>();
        private List<Color> _lastListColor = new List<Color>();

        #endregion

        #region Methods

        /// <summary>
        /// Set light async method.
        /// </summary>
        public async void SetLightsAsync()
        {
            var updateList = new List<LightSettingCellViewModel>();

            foreach (var l in List)
            {
                try
                {
                    if (l.IsChecked != _lastListIsChecked.ElementAt(List.IndexOf(l)) ||
                        l.OnlyBrightness != _lastListOnlyBrightness.ElementAt(List.IndexOf(l)) ||
                        l.Brightness != _lastListBrightness.ElementAt(List.IndexOf(l)) ||
                        l.Color != _lastListColor.ElementAt(List.IndexOf(l)))
                        updateList.Add(l);
                }
                catch
                {
                    updateList.Add(l);
                }
            }

            _lastListIsChecked = new List<bool>(List.Select(x => x.IsChecked).ToList());
            _lastListOnlyBrightness = new List<bool>(List.Select(x => x.OnlyBrightness).ToList());
            _lastListBrightness = new List<byte>(List.Select(x => x.Brightness).ToList());
            _lastListColor = new List<Color>(List.Select(x => x.Color).ToList());

            foreach (var l in updateList)
            {
                if (!l.IsChecked)
                {
                    try
                    {
                        await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                        {
                            On = false
                        }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
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
                        if (l.IsColorTemperature)
                            await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                ColorTemperature = (int) Math.Round(l.ColorTemperature * -0.077111 + 654.222),
                                Brightness = l.Brightness
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
                        else
                            await MainWindowViewModel.Client.SendCommandAsync(new LightCommand
                            {
                                On = true,
                                Hue = (int) Math.Round(ColorConverters.GetHue(l.Color) / 360 * 65535),
                                Saturation = (byte) Math.Round(ColorConverters.GetSaturation(l.Color) * 255),
                                Brightness = l.Brightness
                            }, new List<string> { $"{l.Id}" }).ConfigureAwait(false);
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

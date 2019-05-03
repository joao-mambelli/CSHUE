using System;
using System.Collections.Generic;
using System.Windows.Media;
using Q42.HueApi;
using CSHUE.Helpers;

namespace CSHUE.ViewModels
{
    public class ColorPickerViewModel : BaseViewModel
    {
        public async void SetLightAsync(Color color, byte brightness, int index)
        {
            var command = new LightCommand
            {
                On = true,
                Hue = (int)Math.Round(ColorConverters.GetHue(color) / 360 * 65535),
                Saturation = (byte)Math.Round(ColorConverters.GetSaturation(color) * 255),
                Brightness = brightness,
                TransitionTime = TimeSpan.FromMilliseconds(400)
            };

            await MainWindowViewModel.Client.SendCommandAsync(command, new List<string> {$"{index}"}).ConfigureAwait(false);
        }
    }
}

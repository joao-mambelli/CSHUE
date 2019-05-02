using System;
using System.Collections.Generic;
using System.Windows.Media;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class ColorPickerViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        public Color Hs(double hue, double sat)
        {
            var x = sat * (1 - Math.Abs(hue / (Math.PI / 3) % 2.0 - 1));
            var m = 1 - sat;
            if (hue <= 1 * (Math.PI / 3))
                return Color.FromRgb(255, (byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255));
            if (hue <= 2 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round((m + x) * 255), 255, (byte)Math.Round(m * 255));
            if (hue <= 3 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round(m * 255), 255, (byte)Math.Round((m + x) * 255));
            if (hue <= 4 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255), 255);
            if (hue <= 5 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255), 255);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (hue <= 6 * (Math.PI / 3))
                return Color.FromRgb(255, (byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255));

            return Colors.Transparent;
        }

        public async void SetLightAsync(Color color, byte brightness, int index)
        {
            var command = new LightCommand
            {
                On = true,
                Hue = (int)Math.Round(GetHue(color) / 360 * 65535),
                Saturation = (int)Math.Round((double)GetSaturation(color) / 100 * 255),
                Brightness = brightness,
                TransitionTime = TimeSpan.FromMilliseconds(400)
            };

            await MainWindowViewModel.Client.SendCommandAsync(command, new List<string> {$"{index}"}).ConfigureAwait(false);
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

            return (int)Math.Round(delta / max * 100);
        }
    }
}

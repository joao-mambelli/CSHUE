using System;
using System.Windows.Media;

namespace CSHUE.Helpers
{
    internal class ColorConverters
    {
        #region Methods

        public static Color Hs(double hue, double sat)
        {
            var x = sat * (1 - Math.Abs(hue / (Math.PI / 3) % 2.0 - 1));
            var m = 1 - sat;
            if (hue <= 1 * (Math.PI / 3)) return Color.FromRgb(255, (byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255));
            if (hue <= 2 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round((m + x) * 255), 255, (byte)Math.Round(m * 255));
            if (hue <= 3 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round(m * 255), 255, (byte)Math.Round((m + x) * 255));
            if (hue <= 4 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255), 255);
            if (hue <= 5 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255), 255);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (hue <= 6 * (Math.PI / 3)) return Color.FromRgb(255, (byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255));

            return Colors.Transparent;
        }

        public static Color Ct(int kelvin)
        {
            var temperature = (double)kelvin / 100;

            var green = 99.4708025861 * Math.Log(temperature) - 161.1195681661;
            if (green < 0)
                green = 0;
            if (green > 255)
                green = 255;

            var blue = 138.5177312231 * Math.Log(temperature - 10) - 305.0447927307;
            if (blue < 0)
                blue = 0;
            if (blue > 255)
                blue = 255;

            return Color.FromRgb(255, (byte)Math.Round(green), (byte)Math.Round(blue));
        }

        public static double GetSaturation(Color color)
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

            return delta / max;
        }

        public static float GetHue(Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();

        #endregion
    }
}

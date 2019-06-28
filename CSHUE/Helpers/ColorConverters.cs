using System;
using System.Windows.Media;

namespace CSHUE.Helpers
{
    /// <summary>
    /// Class with some useful color converters helpers.
    /// </summary>
    internal class ColorConverters
    {
        #region Methods

        /// <summary>
        /// Converts a hue and saturation into a pure color.
        /// </summary>
        /// <param name="hue">0 - (2 * PI) <see cref="T:System.Double"/> range.</param>
        /// <param name="sat">0 - 1 <see cref="T:System.Double"/> range.</param>
        /// <returns></returns>
        public static Color HueSaturation(double hue, double sat)
        {
            var x = sat * (1 - Math.Abs(hue / (Math.PI / 3) % 2.0 - 1));
            var m = 1 - sat;
            if (hue <= Math.PI / 3) return Color.FromRgb(255, (byte) Math.Round((m + x) * 255), (byte) Math.Round(m * 255));
            if (hue <= 2 * (Math.PI / 3)) return Color.FromRgb((byte) Math.Round((m + x) * 255), 255, (byte) Math.Round(m * 255));
            if (hue <= 3 * (Math.PI / 3)) return Color.FromRgb((byte) Math.Round(m * 255), 255, (byte) Math.Round((m + x) * 255));
            if (hue <= 4 * (Math.PI / 3)) return Color.FromRgb((byte) Math.Round(m * 255), (byte) Math.Round((m + x) * 255), 255);
            if (hue <= 5 * (Math.PI / 3)) return Color.FromRgb((byte) Math.Round((m + x) * 255), (byte) Math.Round(m * 255), 255);
            return hue <= 6 * (Math.PI / 3)
                ? Color.FromRgb(255, (byte) Math.Round(m * 255), (byte) Math.Round((m + x) * 255))
                : Colors.Transparent;
        }

        /// <summary>
        /// Converts a kelvin value to a <see cref="T:System.Windows.Media.Color"/>.
        /// </summary>
        /// <param name="kelvin">2000 - 65000 <see cref="T:System.Int32"/> range.</param>
        /// <returns></returns>
        public static Color ColorTemperatue(int kelvin)
        {
            var temperature = (double) kelvin / 100;

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

            return Color.FromRgb(255, (byte) Math.Round(green), (byte) Math.Round(blue));
        }

        /// <summary>
        /// Returns a 0 - 1 <see cref="T:System.Double"/> range from a color.
        /// </summary>
        /// <param name="color">Color in question.</param>
        /// <returns></returns>
        public static double GetSaturation(Color color)
        {
            var r = (double) color.R / 255;
            var g = (double) color.G / 255;
            var b = (double) color.B / 255;

            var max = r;

            if (g > max) max = g;
            if (b > max) max = b;

            var min = r;

            if (g < min) min = g;
            if (b < min) min = b;

            var delta = max - min;

            return delta / max;
        }

        /// <summary>
        /// Returns a 0 - 360 <see cref="T:System.Single"/> range from a color.
        /// </summary>
        /// <param name="c">Color in question.</param>
        /// <returns></returns>
        public static float GetHue(Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();

        #endregion
    }
}

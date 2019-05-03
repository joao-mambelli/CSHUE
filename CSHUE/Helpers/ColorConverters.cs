using System;
using System.Windows.Media;

namespace CSHUE.Helpers
{
    internal class ColorConverters
    {
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
    }
}

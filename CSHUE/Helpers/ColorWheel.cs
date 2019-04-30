using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CSHUE.Helpers
{
    public class ColorWheel
    {
        public enum Area
        {
            Outside,
            Wheel
        }

        public struct PickResult
        {
            public Area Area { get; set; }
            public double Hue { get; set; }
            public double Sat { get; set; }
        }

        public int Radius { get; set; }

        public PickResult Pick(int x, int y)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(x - Radius, 2) + Math.Pow(y - Radius, 2));
            if (distanceFromCenter > Radius)
            {
                return new PickResult { Area = Area.Outside };
            }

            var angle = Math.Atan2(y - Radius, x - Radius) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            return new PickResult { Area = Area.Wheel, Hue = angle, Sat = distanceFromCenter / Radius };
        }

        public WriteableBitmap CreateImage()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int)dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(Radius * 2, Radius * 2, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[Radius * 2, Radius * 2, 4];
            for (var y = 0; y < Radius * 2; y++)
            {
                for (var x = 0; x < Radius * 2; x++)
                {
                    var result = Pick(x, y);
                    var color = result.Area == Area.Outside ? Colors.Transparent : Hsb(result.Hue, result.Sat);
                    pixels[y, x, 3] = color.A;
                    pixels[y, x, 2] = color.R;
                    pixels[y, x, 1] = color.G;
                    pixels[y, x, 0] = color.B;
                }
            }

            var pixels1D = new byte[Radius * Radius * 16];
            var index = 0;
            for (var row = 0; row < Radius * 2; row++)
            {
                for (var col = 0; col < Radius * 2; col++)
                {
                    for (var i = 0; i < 4; i++)
                        pixels1D[index++] = pixels[row, col, i];
                }
            }

            var rect = new Int32Rect(0, 0, Radius * 2, Radius * 2);
            img.WritePixels(rect, pixels1D, 4 * Radius * 2, 0);

            return img;
        }

        private static Color Hsb(double hue, double sat)
        {
            var x = sat * (1 - Math.Abs(hue / (Math.PI / 3) % 2.0 - 1));
            var m = 1 - sat;
            if (hue < 1 * (Math.PI / 3)) return Color.FromRgb(255, (byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255));
            if (hue < 2 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round((m + x) * 255), 255, (byte)Math.Round(m * 255));
            if (hue < 3 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round(m * 255), 255, (byte)Math.Round((m + x) * 255));
            if (hue < 4 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255), 255);
            if (hue < 5 * (Math.PI / 3)) return Color.FromRgb((byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255), 255);
            if (hue < 6 * (Math.PI / 3)) return Color.FromRgb(255, (byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255));

            return Colors.Transparent;
        }
    }
}


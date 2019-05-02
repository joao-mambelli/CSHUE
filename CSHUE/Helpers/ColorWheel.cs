using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CSHUE.Helpers
{
    public class ColorWheel
    {
        public Color PickWheelPixelColor(int x, int y, int radius)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(x - radius, 2) + Math.Pow(y - radius, 2));
            if (distanceFromCenter > radius)
            {
                return Colors.Transparent;
            }

            var angle = Math.Atan2(y - radius, x - radius) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            return Hsb(angle, distanceFromCenter / radius);
        }

        public Color PickOutsideWheelPixelColor(int x, int y, int outerRadius, int innerRadius)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(x - outerRadius, 2) + Math.Pow(y - outerRadius, 2));
            if (distanceFromCenter > outerRadius || distanceFromCenter < innerRadius - 2)
            {
                return Colors.Transparent;
            }

            var angle = Math.Atan2(y - outerRadius, x - outerRadius) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            return Hsb(Math.Round(angle / ((double)1 / 18 * Math.PI), MidpointRounding.AwayFromZero) * ((double)1 / 18 * Math.PI), 1);
        }

        public WriteableBitmap CreateWheelImage(int radius)
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

            var img = new WriteableBitmap(radius * 2, radius * 2, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[radius * 2, radius * 2, 4];
            for (var y = 0; y < radius * 2; y++)
            {
                for (var x = 0; x < radius * 2; x++)
                {
                    var color = PickWheelPixelColor(x, y, radius);
                    pixels[y, x, 3] = color.A;
                    pixels[y, x, 2] = color.R;
                    pixels[y, x, 1] = color.G;
                    pixels[y, x, 0] = color.B;
                }
            }

            var pixels1D = new byte[radius * radius * 16];
            var index = 0;
            for (var row = 0; row < radius * 2; row++)
            {
                for (var col = 0; col < radius * 2; col++)
                {
                    for (var i = 0; i < 4; i++)
                        pixels1D[index++] = pixels[row, col, i];
                }
            }

            var rect = new Int32Rect(0, 0, radius * 2, radius * 2);
            img.WritePixels(rect, pixels1D, 4 * radius * 2, 0);

            return img;
        }

        public WriteableBitmap CreateOutsideWheelImage(int outerRadius, int innerRadius)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int)dpiXProperty.GetValue(null, null);
                dpiY = (int)dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(outerRadius * 2, outerRadius * 2, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[outerRadius * 2, outerRadius * 2, 4];
            for (var y = 0; y < outerRadius * 2; y++)
            {
                for (var x = 0; x < outerRadius * 2; x++)
                {
                    var color = PickOutsideWheelPixelColor(x, y, outerRadius, innerRadius);
                    pixels[y, x, 3] = color.A;
                    pixels[y, x, 2] = color.R;
                    pixels[y, x, 1] = color.G;
                    pixels[y, x, 0] = color.B;
                }
            }

            var pixels1D = new byte[outerRadius * outerRadius * 16];
            var index = 0;
            for (var row = 0; row < outerRadius * 2; row++)
            {
                for (var col = 0; col < outerRadius * 2; col++)
                {
                    for (var i = 0; i < 4; i++)
                        pixels1D[index++] = pixels[row, col, i];
                }
            }

            var rect = new Int32Rect(0, 0, outerRadius * 2, outerRadius * 2);
            img.WritePixels(rect, pixels1D, 4 * outerRadius * 2, 0);

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
            if (hue <= 6 * (Math.PI / 3)) return Color.FromRgb(255, (byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255));

            return Colors.Transparent;
        }

        public Color PickHuePixelColor(int y, int height, double sat)
        {
            if (y <= 6 || height - y <= 6) return Colors.Transparent;

            return Hsb(2 * Math.PI - ((double)y - 6) / (height - 12) * (2 * Math.PI), sat / 100);
        }

        public WriteableBitmap CreateHueImage(int height, double sat)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int)dpiXProperty.GetValue(null, null);
                dpiY = (int)dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(1, height, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[1, height, 4];
            for (var y = 0; y < height; y++)
            {
                var color = PickHuePixelColor(y, height, sat);
                pixels[0, y, 3] = color.A;
                pixels[0, y, 2] = color.R;
                pixels[0, y, 1] = color.G;
                pixels[0, y, 0] = color.B;
            }

            var pixels1D = new byte[height * 4];
            var index = 0;
            for (var row = 0; row < height; row++)
            {
                for (var i = 0; i < 4; i++)
                    pixels1D[index++] = pixels[0, row, i];
            }

            var rect = new Int32Rect(0, 0, 1, height);
            img.WritePixels(rect, pixels1D, (img.Format.BitsPerPixel + 7) / 8, 0);

            return img;
        }

        public Color PickSatPixelColor(int y, int height, double hue)
        {
            if (y <= 6 || height - y <= 6) return Colors.Transparent;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Hsb(hue == 360 ? 0 : hue / 360 * (2 * Math.PI), 1 - ((double)y - 6) / (height - 12));
        }

        public WriteableBitmap CreateSatImage(int height, double hue)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int)dpiXProperty.GetValue(null, null);
                dpiY = (int)dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(1, height, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[1, height, 4];
            for (var y = 0; y < height; y++)
            {
                var color = PickSatPixelColor(y, height, hue);
                pixels[0, y, 3] = color.A;
                pixels[0, y, 2] = color.R;
                pixels[0, y, 1] = color.G;
                pixels[0, y, 0] = color.B;
            }

            var pixels1D = new byte[height * 4];
            var index = 0;
            for (var row = 0; row < height; row++)
            {
                for (var i = 0; i < 4; i++)
                    pixels1D[index++] = pixels[0, row, i];
            }

            var rect = new Int32Rect(0, 0, 1, height);
            img.WritePixels(rect, pixels1D, (img.Format.BitsPerPixel + 7) / 8, 0);

            return img;
        }
    }
}


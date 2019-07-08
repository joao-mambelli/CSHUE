using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CSHUE.Helpers
{
    /// <summary>
    /// Class containing methods to get the color wheel bitmaps.
    /// </summary>
    public class ColorWheel
    {
        #region Methods

        /// <summary>
        /// Returns the RGB color from a specific position of the main wheel.
        /// </summary>
        /// <param name="x">The horizontal position from left to right.</param>
        /// <param name="y">The vertical position from up to bottom.</param>
        /// <param name="radius">The radius from the wheel.</param>
        /// <param name="colorTemperature">Color wheel mode.</param>
        /// <returns></returns>
        public Color PickWheelPixelColor(int x, int y, int radius, bool colorTemperature)
        {
            if (colorTemperature)
            {
                return ColorConverters.ColorTemperatue((int) Math.Round((1 - (double) y / (2 * radius)) * 4500 + 2000));
            }

            var distanceFromCenter = Math.Sqrt(Math.Pow(x - radius, 2) + Math.Pow(y - radius, 2));
            if (distanceFromCenter > radius)
            {
                return Colors.Transparent;
            }

            var angle = Math.Atan2(y - radius, x - radius) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            return ColorConverters.HueSaturation(angle, distanceFromCenter / radius);
        }

        /// <summary>
        /// Returns the RGB color from a specific position of the outside wheel.
        /// </summary>
        /// <param name="x">The horizontal position from left to right.</param>
        /// <param name="y">The vertical position from up to bottom.</param>
        /// <param name="outerRadius">Outer radius of the outside wheel.</param>
        /// <param name="innerRadius">Inner radius of the outside wheel.</param>
        /// <returns></returns>
        public Color PickOutsideWheelPixelColor(int x, int y, int outerRadius, int innerRadius)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(x - outerRadius, 2) + Math.Pow(y - outerRadius, 2));
            if (distanceFromCenter > outerRadius || distanceFromCenter < innerRadius - 2)
            {
                return Colors.Transparent;
            }

            var angle = Math.Atan2(y - outerRadius, x - outerRadius) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            return ColorConverters.HueSaturation(Math.Round(angle / ((double)1 / 18 * Math.PI), MidpointRounding.AwayFromZero) * ((double)1 / 18 * Math.PI), 1);
        }

        /// <summary>
        /// Returns the bitmap image of a color wheel of the given radius.
        /// </summary>
        /// <param name="radius">Wheel radius.</param>
        /// <param name="colorTemperature">Color wheel mode.</param>
        /// <returns></returns>
        public WriteableBitmap CreateWheelImage(int radius, bool colorTemperature)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int) dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(radius * 2, radius * 2, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[radius * 2, radius * 2, 4];
            for (var y = 0; y < radius * 2; y++)
            {
                for (var x = 0; x < radius * 2; x++)
                {
                    var color = PickWheelPixelColor(x, y, radius, colorTemperature);
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

        /// <summary>
        /// Returns the bitmap image of the outside color wheel of the given radius.
        /// </summary>
        /// <param name="outerRadius">Wheel outer radius of the outside wheel.</param>
        /// <param name="innerRadius">Wheel inner radius of the outside wheel.</param>
        /// <param name="colorTemperature">Color wheel mode.</param>
        /// <returns></returns>
        public WriteableBitmap CreateOutsideWheelImage(int outerRadius, int innerRadius, bool colorTemperature)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int) dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(outerRadius * 2, outerRadius * 2, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[outerRadius * 2, outerRadius * 2, 4];
            for (var y = 0; y < outerRadius * 2; y++)
            {
                for (var x = 0; x < outerRadius * 2; x++)
                {
                    var color = colorTemperature
                        ? Colors.Transparent
                        : PickOutsideWheelPixelColor(x, y, outerRadius, innerRadius);
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

        /// <summary>
        /// Returns the RGB color from a specific position of the HUE scale.
        /// </summary>
        /// <param name="y">The vertical position from up to bottom.</param>
        /// <param name="height">Total height of the scale.</param>
        /// <param name="sat">Current saturation of the hue scale.</param>
        /// <returns></returns>
        public Color PickHuePixelColor(int y, int height, double sat)
        {
            if (y <= 6 || height - y <= 6) return Colors.Transparent;

            return ColorConverters.HueSaturation(2 * Math.PI - ((double) y - 6) / (height - 12) * (2 * Math.PI), sat / 100);
        }

        /// <summary>
        /// Returns the bitmap image of the HUE scale said a specific saturation.
        /// </summary>
        /// <param name="height">Height of the hue scale.</param>
        /// <param name="sat">Current saturation of the hue scale.</param>
        /// <returns></returns>
        public WriteableBitmap CreateHueImage(int height, double sat)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int) dpiYProperty.GetValue(null, null);
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

        /// <summary>
        /// Returns the RGB color from a specific position of the saturation scale.
        /// </summary>
        /// <param name="y">The vertical position from up to bottom.</param>
        /// <param name="height">Total height of the scale.</param>
        /// <param name="hue">Current HUE of the saturation scale.</param>
        /// <returns></returns>
        public Color PickSaturationPixelColor(int y, int height, double hue)
        {
            if (y <= 6 || height - y <= 6) return Colors.Transparent;
            
            return ColorConverters.HueSaturation(hue == 360 ? 0 : hue / 360 * (2 * Math.PI), 1 - ((double) y - 6) / (height - 12));
        }

        /// <summary>
        /// Returns the bitmap image of the saturation scale said a specific HUE.
        /// </summary>
        /// <param name="height">Height of the saturation scale.</param>
        /// <param name="hue">Current HUE of the saturation scale.</param>
        /// <returns></returns>
        public WriteableBitmap CreateSaturationImage(int height, double hue)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int) dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(1, height, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[1, height, 4];
            for (var y = 0; y < height; y++)
            {
                var color = PickSaturationPixelColor(y, height, hue);
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

        /// <summary>
        /// Returns the RGB color from a specific position of the color temperature scale.
        /// </summary>
        /// <param name="y">The vertical position from up to bottom.</param>
        /// <param name="height">Total height of the scale.</param>
        /// <returns></returns>
        public Color PickTemperaturePixelColor(int y, int height)
        {
            if (y <= 6 || height - y <= 6) return Colors.Transparent;
            
            return ColorConverters.ColorTemperatue((int) Math.Round((1 - (double) y / height) * 4500 + 2000));
        }

        /// <summary>
        /// Returns the bitmap image of the color temperature scale.
        /// </summary>
        /// <param name="height">Height of the color temperature scale.</param>
        /// <returns></returns>
        public WriteableBitmap CreateTemperatureImage(int height)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = 96;
            var dpiY = 96;
            if (dpiXProperty != null && dpiYProperty != null)
            {
                dpiX = (int) dpiXProperty.GetValue(null, null);
                dpiY = (int) dpiYProperty.GetValue(null, null);
            }

            var img = new WriteableBitmap(1, height, dpiX, dpiY, PixelFormats.Bgra32, null);
            var pixels = new byte[1, height, 4];
            for (var y = 0; y < height; y++)
            {
                var color = PickTemperaturePixelColor(y, height);
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

        #endregion
    }
}


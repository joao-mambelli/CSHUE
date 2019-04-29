using System.Drawing;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CSHUE.Helpers
{
    public class ColorWheel
    {
        public int Size { get; set; }

        public int CenterX => Size / 2;
        public int CenterY => Size / 2;
        public int Radius => Size / 2;

        public ColorWheel(int size = 400)
        {
            Size = size;
        }

        public enum Area
        {
            Outside,
            Wheel
        }

        public struct PickResult
        {
            public Area Area { get; set; }
            public double? Hue { get; set; }
            public double? Sat { get; set; }
        }

        public PickResult Pick(double x, double y)
        {
            var distanceFromCenter = Math.Sqrt(Math.Pow(x - CenterX, 2) + Math.Pow(y - CenterY, 2));
            if (distanceFromCenter > Radius)
            {
                return new PickResult { Area = Area.Outside };
            }

            var angle = Math.Atan2(y - CenterY, x - CenterX) + Math.PI / 2;
            if (angle < 0) angle += 2 * Math.PI;
            var hue = angle;
            var sat = Math.Sqrt(Math.Pow(x - CenterX, 2) + Math.Pow(y - CenterY, 2)) / Radius;
            return new PickResult { Area = Area.Wheel, Hue = hue, Sat = sat};
        }

        public WriteableBitmap CreateImage()
        {
            var img = new WriteableBitmap(Size, Size, 72, 72, PixelFormats.Bgra32, null);
            byte[,,] pixels = new byte[Size, Size, 4];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Color color;
                    var result = Pick(x, y);
                    if (result.Area == Area.Outside)
                    {
                        color = Color.Transparent;
                    }
                    else
                    {
                        color = HSB(result.Hue.Value, result.Sat.Value);
                    }
                    pixels[y, x, 3] = color.A;
                    pixels[y, x, 2] = color.R;
                    pixels[y, x, 1] = color.G;
                    pixels[y, x, 0] = color.B;
                }
            }

            byte[] pixels1d = new byte[Size * Size * 4];
            int index = 0;
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, Size, Size);
            int stride = 4 * Size;
            img.WritePixels(rect, pixels1d, stride, 0);

            return img;
        }

        private Color HSB(double hue, double sat)
        {
            var step = Math.PI / 3;
            var interm = sat * (1 - Math.Abs((hue / step) % 2.0 - 1));
            var shift = 1 - sat;
            if (hue < 1 * step) return RGB(shift + sat, shift + interm, shift + 0);
            if (hue < 2 * step) return RGB(shift + interm, shift + sat, shift + 0);
            if (hue < 3 * step) return RGB(shift + 0, shift + sat, shift + interm);
            if (hue < 4 * step) return RGB(shift + 0, shift + interm, shift + sat);
            if (hue < 5 * step) return RGB(shift + interm, shift + 0, shift + sat);
            return RGB(shift + sat, shift + 0, shift + interm);
        }

        private Color RGB(double red, double green, double blue)
        {
            return Color.FromArgb(
                255,
                Math.Min(255, (int)(red * 256)),
                Math.Min(255, (int)(green * 256)),
                Math.Min(255, (int)(blue * 256)));
        }

        public PointD GetWheelPosition(double hue, double sat)
        {
            return new PointD
            {
                X = CenterX + Radius * Math.Sin(hue) * sat,
                Y = CenterY - Radius * Math.Cos(hue) * sat
            };
        }
    }

    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            public double Hue { get; set; }
            public double Sat { get; set; }
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
            var pixels = new byte[Size, Size, 4];
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var result = Pick(x, y);
                    var color = result.Area == Area.Outside ? Colors.Transparent : Hsb(result.Hue, result.Sat);
                    pixels[y, x, 3] = color.A;
                    pixels[y, x, 2] = color.R;
                    pixels[y, x, 1] = color.G;
                    pixels[y, x, 0] = color.B;
                }
            }

            var pixels1D = new byte[Size * Size * 4];
            var index = 0;
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    for (var i = 0; i < 4; i++)
                        pixels1D[index++] = pixels[row, col, i];
                }
            }

            var rect = new Int32Rect(0, 0, Size, Size);
            var stride = 4 * Size;
            img.WritePixels(rect, pixels1D, stride, 0);

            return img;
        }

        private static Color Hsb(double hue, double sat)
        {
            var interm = sat * (1 - Math.Abs(hue / (Math.PI / 3) % 2.0 - 1));
            var shift = 1 - sat;
            if (hue < 1 * (Math.PI / 3)) return Rgb(shift + sat, shift + interm, shift + 0);
            if (hue < 2 * (Math.PI / 3)) return Rgb(shift + interm, shift + sat, shift + 0);
            if (hue < 3 * (Math.PI / 3)) return Rgb(shift + 0, shift + sat, shift + interm);
            if (hue < 4 * (Math.PI / 3)) return Rgb(shift + 0, shift + interm, shift + sat);
            return hue < 5 * (Math.PI / 3) ? Rgb(shift + interm, shift + 0, shift + sat) : Rgb(shift + sat, shift + 0, shift + interm);
        }

        private static Color Rgb(double red, double green, double blue)
        {
            return Color.FromRgb(
                Math.Min((byte)255, (byte)(red * 256)),
                Math.Min((byte)255, (byte)(green * 256)),
                Math.Min((byte)255, (byte)(blue * 256)));
        }
    }
}


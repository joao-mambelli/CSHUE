using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CSHUE.Core.Utilities
{
    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Double" />.
    /// </summary>
    public class BoolToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts from <see cref="T:System.Boolean" /> to <see cref="T:System.Double" />. 1 if <paramref name="value" /> is <see langword="true" />, .5 if <paramref name="value" /> is <see langword="false" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <remarks></remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? 1 : 0.5;
        }

        /// <summary>
        /// Converts from <see cref="T:System.Double" /> to <see cref="T:System.Boolean" />. <see langword="true" /> if <paramref name="value" /> is greater than .5, <see langword="false" /> if <paramref name="value" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (double) value > 0.5;
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.Byte" /> to <see cref="T:System.Int32" />.
    /// </summary>
    public class ByteToInt32Converter : IValueConverter
    {
        /// <summary>
        /// Converts the 0 - 255 range of a <see cref="T:System.Byte" /> into a 0 - 100 range of a <see cref="T:System.Int32" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return (int) Math.Round(double.Parse(value.ToString()) / 255 * 100);

            return 0;
        }

        /// <summary>
        /// Converts the 0 - 100 range of a <see cref="T:System.Int32" /> into a 0 - 255 range of a <see cref="T:System.Byte" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return (byte) Math.Round(double.Parse(value.ToString()) / 100 * 255);

            return 0;
        }
    }

    /// <summary>
    /// Custom inverter from <see langword="Visibility.Visible" /> to <see langword="Visibility.Collapsed" />.
    /// </summary>
    public class VisibilityInverter : IValueConverter
    {
        /// <summary>
        /// Inverts the <see cref="T:System.Windows.Visibility" />, from <see langword="Visibility.Visible" /> to <see langword="Visibility.Collapsed" /> or to <see langword="Visibility.Collapsed" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Inverts the <see cref="T:System.Windows.Visibility" />, from <see langword="Visibility.Collapsed" /> to <see langword="Visibility.Visible" /> or to <see langword="Visibility.Collapsed" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    /// <summary>
    /// Custom inverter from <see langword="false" /> to <see langword="true" />.
    /// </summary>
    public class BooleanInverter : IValueConverter
    {
        /// <summary>
        /// Inverts the <see cref="T:System.Boolean" />, from <see langword="false" /> to <see langword="true" /> or to <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool) value;
        }

        /// <summary>
        /// Inverts the <see cref="T:System.Boolean" />, from <see langword="false" /> to <see langword="true" /> or to <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool) value;
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.String" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns <see langword="Visibility.Collapsed" /> if <see cref="T:System.String" /> is empty or <see langword="null" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        /// <summary>
        /// Returns an empty <see cref="T:System.String" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns <see langword="Visibility.Visible" /> if <see cref="T:System.Boolean" /> is <see langword="true" />, <see langword="Visibility.Collapsed" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool) value)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Returns <see langword="true" /> if <see langword="Visibility.Visible" /> , <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Visible;
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class BooleanToCollapsedConverter : IValueConverter
    {
        /// <summary>
        /// Returns <see langword="Visibility.Collapsed" /> if <see cref="T:System.Boolean" /> is <see langword="true" />, <see langword="Visibility.Visible" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool) value)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        /// <summary>
        /// Returns <see langword="true" /> if <see langword="Visibility.Collapsed" /> , <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.Double" /> to <see cref="T:System.Windows.Media.Color" />.
    /// </summary>
    public class DoubleToColorConverter : IValueConverter
    {
        /// <summary>
        /// Returns the relative white shade color from the 0 - 1 range of a <see cref="T:System.Double" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Color.FromRgb((byte) Math.Round((double) value * 255),
                    (byte) Math.Round((double) value * 255),
                    (byte) Math.Round((double) value * 255));

            return Colors.Black;
        }

        /// <summary>
        /// Returns a black color always.
        /// </summary>
        /// <param name="value">ignored</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Colors.Black;
        }
    }

    /// <summary>
    /// Custom converter from <see cref="T:System.Double" /> to <see cref="T:System.Int32" />.
    /// </summary>
    public class DoubleToIntConverter : IValueConverter
    {
        /// <summary>
        /// Returns the relative 0 - 100 range of a <see cref="T:System.Int32" /> from a 0 - 1 range of a <see cref="T:System.Double" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return (int) Math.Round((double) value * 100);

            return 0;
        }

        /// <summary>
        /// Returns the relative 0 - 1 range of a <see cref="T:System.Double" /> from a 0 - 100 range of a <see cref="T:System.Int32" />.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return (double) value / 100;

            return 0;
        }
    }

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

            if (hue <= Math.PI / 3)
                return Color.FromRgb(255, (byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255));

            if (hue <= 2 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round((m + x) * 255), 255, (byte)Math.Round(m * 255));

            if (hue <= 3 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round(m * 255), 255, (byte)Math.Round((m + x) * 255));

            if (hue <= 4 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255), 255);

            if (hue <= 5 * (Math.PI / 3))
                return Color.FromRgb((byte)Math.Round((m + x) * 255), (byte)Math.Round(m * 255), 255);

            if (hue <= 6 * (Math.PI / 3))
                return Color.FromRgb(255, (byte)Math.Round(m * 255), (byte)Math.Round((m + x) * 255));

            return Colors.Transparent;
        }

        /// <summary>
        /// Converts a kelvin value to a <see cref="T:System.Windows.Media.Color"/>.
        /// </summary>
        /// <param name="kelvin">2000 - 65000 <see cref="T:System.Int32"/> range.</param>
        /// <returns></returns>
        public static Color ColorTemperatue(int kelvin)
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

        /// <summary>
        /// Returns a 0 - 1 <see cref="T:System.Double"/> range from a color.
        /// </summary>
        /// <param name="color">Color in question.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a 0 - 360 <see cref="T:System.Single"/> range from a color.
        /// </summary>
        /// <param name="c">Color in question.</param>
        /// <returns></returns>
        public static float GetHue(Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CSHUE.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Double" />.
    /// </summary>
    public class BoolToDoubleConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Byte" /> to <see cref="T:System.Int32" />.
    /// </summary>
    public class ByteToInt32Converter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom inverter from <see langword="Visibility.Visible" /> to <see langword="Visibility.Collapsed" />.
    /// </summary>
    public class VisibilityInverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom inverter from <see langword="false" /> to <see langword="true" />.
    /// </summary>
    public class BooleanInverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.String" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Boolean" /> to <see cref="T:System.Windows.Visibility" />.
    /// </summary>
    public class BooleanToCollapsedConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Double" /> to <see cref="T:System.Windows.Media.Color" />.
    /// </summary>
    public class DoubleToColorConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    /// Custom converter from <see cref="T:System.Double" /> to <see cref="T:System.Int32" />.
    /// </summary>
    public class DoubleToIntConverter : IValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CSHUE.Cultures;

namespace CSHUE.Helpers
{
    public static class Converters
    {
        public static CultureInfo GetCultureInfoFromIndex(int index)
        {
            return CultureResources.SupportedCultures.ElementAt(index);
        }

        public static int GetIndexFromCultureInfo(CultureInfo cultureInfo)
        {
            return CultureResources.SupportedCultures.IndexOf(cultureInfo);
        }
    }

    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? 1 : 0.5;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ByteToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null) return (int)Math.Round(double.Parse(value.ToString()) / 255 * 100);
            return 0;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null) return (byte)Math.Round(double.Parse(value.ToString()) / 100 * 255);
            return 0;
        }
    }

    public class VisibleToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class FalseToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }
    }
}

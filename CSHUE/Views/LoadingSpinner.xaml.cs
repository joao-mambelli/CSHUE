using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class LoadingSpinner
    {
        public LoadingSpinner()
        {
            InitializeComponent();
        }
        
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                "IsLoading",
                typeof(bool),
                typeof(LoadingSpinner),
                new PropertyMetadata(default(bool), OnIsLoadingPropertyChanged));

        public static double Angle { get; set; }

        public static double Angle1 => Angle - 360;

        public static double Angle2 => Angle + 276;

        public static double Angle3 => Angle + 552;

        public static double Angle4 => Angle + 828;

        public static double Angle5 => Angle + 1104;

        public static double Angle6 => Angle + 1380;

        public static double Angle7 => Angle + 1380;

        public static double Angle8 => Angle + 1500;

        private static void OnIsLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Angle = ((LoadingSpinner) d).RotateCanvas.Angle;

            VisualStateManager.GoToState((LoadingSpinner)d, (bool)e.NewValue ? "Loading" : "Hanging", true);

            if ((bool) e.NewValue)
            {
                ((LoadingSpinner) d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseHighColorBrush");
            }
            else
            {
                ((LoadingSpinner) d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseMediumColorBrush");
            }
        }
    }

    public class DatabindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}

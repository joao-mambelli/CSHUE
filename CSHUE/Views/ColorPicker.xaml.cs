using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CSHUE.Helpers;
using MessageBox = System.Windows.MessageBox;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker
    {
        private const int Radius = 125;

        public string Text1
        {
            get => (string)GetValue(Text1Property);
            set => SetValue(Text1Property, value);
        }
        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(ColorPicker));

        public string Text2
        {
            get => (string)GetValue(Text2Property);
            set => SetValue(Text2Property, value);
        }
        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(string), typeof(ColorPicker));

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ColorPicker_OnLoaded(object sender, RoutedEventArgs e)
        {
            ImageBrush.ImageSource = new ColorWheel { Radius = Radius } .CreateImage();
        }

        private void ColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            InitializeCursorMonitoring();
        }

        public Thickness MousePosition
        {
            get => (Thickness)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }
        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(Thickness), typeof(ColorPicker));

        private void InitializeCursorMonitoring()
        {
            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new Point(0, 0));

            new Thread(() =>
                {
                    while (Control.MouseButtons == MouseButtons.Left)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            var colorWheelCenterRelativeMousePosition = new Point(System.Windows.Forms.Cursor.Position.X - Left - colorWheelPos.X - Radius,
                                System.Windows.Forms.Cursor.Position.Y - Top - colorWheelPos.Y - Radius);
                            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) + Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));
                            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.X, colorWheelCenterRelativeMousePosition.Y) + Math.PI / 2;

                            if (angle < 0) angle += 2 * Math.PI;

                            if (distanceFromCenter < Radius)
                            {
                                Hue = (int)Math.Round(angle / (2 * Math.PI) * 360);
                                Sat = (int)Math.Round(distanceFromCenter / Radius * 100);
                            }
                            else
                            {
                                Hue = (int)Math.Round(angle / (2 * Math.PI) * 360);
                                Sat = 100;
                            }

                            //MousePosition = new Thickness(colorWheelPos.X + Radius - 6 + colorWheelCenterRelativeMousePosition.X, colorWheelPos.Y + Radius - 7 + colorWheelCenterRelativeMousePosition.Y, 0, 0);
                        });
                        
                        Thread.Sleep(16);
                    }
                })
                { IsBackground = true }.Start();
        }

        public int Hue
        {
            get => (int)GetValue(HueProperty);
            set
            {
                SetValue(HueProperty, value);
                var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new Point(0, 0));
                MousePosition = new Thickness(colorWheelPos.X + Radius - 6 - Radius * Math.Cos((double)Hue / 360 * Math.PI * 2) * ((double)Sat / 100), colorWheelPos.Y + Radius - 7 + Radius * Math.Sin((double)Hue / 360 * Math.PI * 2) * ((double)Sat / 100), 0, 0);
            }
        }
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(int), typeof(LightSettingCell));

        public int Sat
        {
            get => (int)GetValue(SatProperty);
            set
            {
                SetValue(SatProperty, value);
                var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new Point(0, 0));
                MousePosition = new Thickness(colorWheelPos.X + Radius - 6 - Radius * Math.Cos((double)Hue / 360 * Math.PI * 2) * ((double)Sat / 100), colorWheelPos.Y + Radius - 7 + Radius * Math.Sin((double)Hue / 360 * Math.PI * 2) * ((double)Sat / 100), 0, 0);
            }
        }

        public static readonly DependencyProperty SatProperty =
            DependencyProperty.Register("Sat", typeof(int), typeof(LightSettingCell));
    }
}

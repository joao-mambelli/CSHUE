using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using CSHUE.Helpers;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
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
            DialogResult = true;
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private ColorWheel _wheel = new ColorWheel(size);

        private void ColorPicker_OnLoaded(object sender, RoutedEventArgs e)
        {
            ImageBrush.ImageSource = _wheel.CreateImage();
        }

        private static int size = 250;
        private bool donothing;
        private bool _movementStartedInside;
        private void ColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _movementStartedInside = true;
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
            new Thread(() =>
            {
                while (_movementStartedInside)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MousePosition = new Thickness(System.Windows.Forms.Cursor.Position.X - Left - 6, System.Windows.Forms.Cursor.Position.Y - Top - 7, 0, 0);
                    });

                    Thread.Sleep(16);
                }
            })
            { IsBackground = true }.Start();
        }

        private void ColorWheel_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _movementStartedInside = false;
        }
    }
}

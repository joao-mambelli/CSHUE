using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CSHUE.Controls;
using CSHUE.Helpers;
using CSHUE.ViewModels;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker
    {
        #region Low Level Mouse Hook

        private static LowLevelMouseProc _proc;

        private static IntPtr _hookId = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(14, proc, GetModuleHandle(curModule?.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (MouseMessages.WmLbuttonup == (MouseMessages) wParam)
            {
                UnhookWindowsHookEx(_hookId);
                _movingPicker = false;
            }

            if (nCode < 0 || MouseMessages.WmMousemove != (MouseMessages) wParam)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            var hookStruct = (Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(Msllhookstruct));

            ChangeHueSat(hookStruct.Position.X, hookStruct.Position.Y, _approximate);

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        private enum MouseMessages
        {
            WmLbuttondown = 0x0201,
            WmLbuttonup = 0x0202,
            WmMousemove = 0x0200,
            WmMousewheel = 0x020A,
            WmRbuttondown = 0x0204,
            WmRbuttonup = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public readonly int X;
            public readonly int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Msllhookstruct
        {
            public readonly Point Position;
            private readonly uint mouseData;
            private readonly uint flags;
            private readonly uint time;
            private readonly IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Properties

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

        public WriteableBitmap HueSliderBrush
        {
            get => (WriteableBitmap)GetValue(HueSliderBrushProperty);
            set => SetValue(HueSliderBrushProperty, value);
        }
        public static readonly DependencyProperty HueSliderBrushProperty =
            DependencyProperty.Register("HueSliderBrush", typeof(WriteableBitmap), typeof(ColorPicker));

        public WriteableBitmap SatSliderBrush
        {
            get => (WriteableBitmap)GetValue(SatSliderBrushProperty);
            set => SetValue(SatSliderBrushProperty, value);
        }
        public static readonly DependencyProperty SatSliderBrushProperty =
            DependencyProperty.Register("SatSliderBrush", typeof(WriteableBitmap), typeof(ColorPicker));

        public Thickness MousePosition
        {
            get => (Thickness)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }
        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(Thickness), typeof(ColorPicker));

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(LightSettingCell));

        public double Sat
        {
            get => (double)GetValue(SatProperty);
            set => SetValue(SatProperty, value);
        }
        public static readonly DependencyProperty SatProperty =
            DependencyProperty.Register("Sat", typeof(double), typeof(LightSettingCell));

        public Color Color { get; set; }

        public int Index { get; set; }

        public byte Brightness { get; set; }

        #endregion

        #region Globals

        public ColorPickerViewModel ViewModel = new ColorPickerViewModel();

        private bool _movingPicker;

        private bool _approximate;

        #endregion

        #region Event Handlers

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            _isWindowOpened = false;
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _isWindowOpened = false;
            Close();
        }

        private void HueSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Hue = e.NewValue;
            var bitMap = new ColorWheel().CreateSatImage((int)SatSlider.ActualHeight, e.NewValue);
            bitMap.Freeze();
            SatSliderBrush = bitMap;

            if (_movingPicker) return;
            MousePosition = new Thickness(ColorWheel.ActualWidth * Math.Sin(Hue / 360 * Math.PI * 2) * (Sat / 100), 0,
                0, ColorWheel.ActualWidth * Math.Cos(Hue / 360 * Math.PI * 2) * (Sat / 100));
            Color = ColorConverters.Hs(Hue / 360 * 2 * Math.PI, Sat / 100);
            SelectedColor.Background = new SolidColorBrush(Color);
        }

        private void SatSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Sat = e.NewValue;
            var bitMap = new ColorWheel().CreateHueImage((int)HueSlider.ActualHeight, e.NewValue);
            bitMap.Freeze();
            HueSliderBrush = bitMap;

            if (_movingPicker) return;
            MousePosition = new Thickness(ColorWheel.ActualWidth * Math.Sin(Hue / 360 * Math.PI * 2) * (Sat / 100), 0,
                0, ColorWheel.ActualWidth * Math.Cos(Hue / 360 * Math.PI * 2) * (Sat / 100));
            Color = ColorConverters.Hs(Hue / 360 * 2 * Math.PI, Sat / 100);
            SelectedColor.Background = new SolidColorBrush(Color);
        }

        private void OutsideColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _approximate = true;
            _hookId = SetHook(_proc);

            ChangeHueSat((int)Math.Round(PointToScreen(e.GetPosition(this)).X),
                (int)Math.Round(PointToScreen(e.GetPosition(this)).Y), true);
        }

        private void ColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _approximate = false;
            _hookId = SetHook(_proc);
            _movingPicker = true;

            ChangeHueSat((int)Math.Round(PointToScreen(e.GetPosition(this)).X),
                (int)Math.Round(PointToScreen(e.GetPosition(this)).Y), false);
        }

        #endregion

        #region Initializers

        private bool _isWindowOpened = true;
        public ColorPicker()
        {
            InitializeComponent();
            _proc = HookCallback;

            new Thread(() =>
            {
                while (_isWindowOpened && Properties.Settings.Default.PreviewLights)
                {
                    ViewModel.SetLightAsync(Color, Brightness, Index);

                    Thread.Sleep(500);
                }
            })
            { IsBackground = true }.Start();
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private void ColorPicker_OnLoaded(object sender, RoutedEventArgs e)
        {
            ColorWheelBrush.ImageSource = new ColorWheel().CreateWheelImage((int)(ColorWheel.ActualWidth / 2));
            OutsideColorWheelBrush.ImageSource =
                new ColorWheel().CreateOutsideWheelImage((int)OutsideColorWheel.ActualWidth / 2,
                    (int)ColorWheel.ActualWidth / 2);

            _movingPicker = true;
            HueSlider.Value = 360;
            SatSlider.Value = 100;
            
            HueSlider.Value = Math.Round(ColorConverters.GetHue(Color));
            SatSlider.Value = Math.Round(ColorConverters.GetSaturation(Color) * 100);

            SelectedColor.Background = new SolidColorBrush(Color);
            _movingPicker = false;

            MousePosition = new Thickness(ColorWheel.ActualWidth * Math.Sin(Hue / 360 * Math.PI * 2) * (Sat / 100), 0,
                0, ColorWheel.ActualWidth * Math.Cos(Hue / 360 * Math.PI * 2) * (Sat / 100));
        }

        #endregion

        #region Methods

        private void ChangeHueSat(int x, int y, bool approximate)
        {
            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            var colorWheelCenterRelativeMousePosition = new System.Windows.Point(
                x - Left - colorWheelPos.X - ColorWheel.ActualWidth / 2,
                y - Top - colorWheelPos.Y - ColorWheel.ActualWidth / 2);
            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) +
                                               Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));
            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.Y, colorWheelCenterRelativeMousePosition.X) +
                        Math.PI / 2;

            if (angle < 0) angle += 2 * Math.PI;

            HueSlider.Value = (int)Math.Round(approximate
                ? Math.Round(angle / ((double)1 / 18 * Math.PI)) * ((double)1 / 18 * Math.PI) / (2 * Math.PI) * 360
                : angle / (2 * Math.PI) * 360);
            SatSlider.Value = distanceFromCenter < ColorWheel.ActualWidth / 2
                ? (int)Math.Round(distanceFromCenter / (ColorWheel.ActualWidth / 2) * 100)
                : 100;

            MousePosition = distanceFromCenter < ColorWheel.ActualWidth / 2
                ? new Thickness(colorWheelCenterRelativeMousePosition.X * 2,
                    colorWheelCenterRelativeMousePosition.Y * 2, 0, 0)
                : new Thickness(ColorWheel.ActualWidth * Math.Sin(Hue / 360 * Math.PI * 2) * (Sat / 100), 0, 0,
                    ColorWheel.ActualWidth * Math.Cos(Hue / 360 * Math.PI * 2) * (Sat / 100));
            Color = ColorConverters.Hs(Hue / 360 * 2 * Math.PI, Sat / 100);
            SelectedColor.Background = new SolidColorBrush(Color);
        }

        #endregion
    }
}

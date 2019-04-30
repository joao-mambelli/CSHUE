using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CSHUE.Helpers;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker
    {
        private static LowLevelMouseProc _proc;

        private static IntPtr _hookId = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhMouseLl, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (MouseMessages.WmLbuttonup == (MouseMessages) wParam)
            {
                UnhookWindowsHookEx(_hookId);
            }

            if (nCode >= 0 && MouseMessages.WmMousemove == (MouseMessages)wParam)
            {
                Msllhookstruct hookStruct = (Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(Msllhookstruct));

                ChangeHueSat(hookStruct.Position.X, hookStruct.Position.Y);
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private const int WhMouseLl = 14;

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

        private const int Radius = 125;
        private const int ImageHeight = 270;

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
            _proc = HookCallback;
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
            ImageBrush.ImageSource = new ColorWheel().CreateWheelImage(Radius);
            HueSliderBrush.ImageSource = new ColorWheel().CreateHueImage(ImageHeight, Sat);

            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            MousePosition = new Thickness(colorWheelPos.X + Radius - 6 - Radius * Math.Cos(Hue / 360 * Math.PI * 2) * (Sat / 100), colorWheelPos.Y + Radius - 7 + Radius * Math.Sin(Hue / 360 * Math.PI * 2) * (Sat / 100), 0, 0);
        }

        private void ColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _hookId = SetHook(_proc);

            ChangeHueSat((int)Math.Round(PointToScreen(e.GetPosition(this)).X), (int)Math.Round(PointToScreen(e.GetPosition(this)).Y));
        }

        public Thickness MousePosition
        {
            get => (Thickness)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }
        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(Thickness), typeof(ColorPicker));

        private void ChangeHueSat(int x, int y)
        {
            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            var colorWheelCenterRelativeMousePosition = new System.Windows.Point(x - Left - colorWheelPos.X - Radius,
                y - Top - colorWheelPos.Y - Radius);
            var distanceFromCenter = Math.Sqrt(Math.Pow(colorWheelCenterRelativeMousePosition.X, 2) + Math.Pow(colorWheelCenterRelativeMousePosition.Y, 2));
            var angle = Math.Atan2(colorWheelCenterRelativeMousePosition.X, colorWheelCenterRelativeMousePosition.Y) + Math.PI / 2;

            if (angle < 0) angle += 2 * Math.PI;

            HueSlider.Value = (int)Math.Round(angle / (2 * Math.PI) * 360);
            SatSlider.Value = distanceFromCenter < Radius ? (int)Math.Round(distanceFromCenter / Radius * 100) : 100;
        }

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

        private void HueSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Hue = e.NewValue;
            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            MousePosition = new Thickness(colorWheelPos.X + Radius - 6 - Radius * Math.Cos(e.NewValue / 360 * Math.PI * 2) * (Sat / 100), colorWheelPos.Y + Radius - 7 + Radius * Math.Sin(e.NewValue / 360 * Math.PI * 2) * (Sat / 100), 0, 0);

        }

        private void SatSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Sat = e.NewValue;
            var colorWheelPos = ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            MousePosition = new Thickness(colorWheelPos.X + Radius - 6 - Radius * Math.Cos(Hue / 360 * Math.PI * 2) * (e.NewValue / 100), colorWheelPos.Y + Radius - 7 + Radius * Math.Sin(Hue / 360 * Math.PI * 2) * (e.NewValue / 100), 0, 0);
            HueSliderBrush.ImageSource = new ColorWheel().CreateHueImage(ImageHeight, e.NewValue);
        }
    }
}

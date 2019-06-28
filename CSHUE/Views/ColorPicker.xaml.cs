using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CSHUE.Helpers;
using CSHUE.ViewModels;
// ReSharper disable PossibleLossOfFraction

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
                ViewModel.MovingPicker = false;
            }

            if (nCode < 0 || MouseMessages.WmMousemove != (MouseMessages) wParam)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            var hookStruct = (Msllhookstruct) Marshal.PtrToStructure(lParam, typeof(Msllhookstruct));

            if (IsColorTemperature)
            {
                ViewModel.ChangeTemperature(new System.Windows.Point(
                        hookStruct.Position.X - Left -
                        ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).X -
                        ViewModel.ColorWheelSize / 2,
                        hookStruct.Position.Y - Top -
                        ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).Y -
                        ViewModel.ColorWheelSize / 2));
            }
            else
            {
                ViewModel.ChangeHueSaturation(new System.Windows.Point(
                        hookStruct.Position.X - Left -
                        ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).X -
                        ViewModel.ColorWheelSize / 2,
                        hookStruct.Position.Y - Top -
                        ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).Y -
                        ViewModel.ColorWheelSize / 2),
                    _approximate);
            }

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

        public Color Color { get; set; }

        public string Id { get; set; }

        public byte Brightness { get; set; }

        public bool IsColorTemperature { get; set; }

        public int ColorTemperature { get; set; }

        public string Text1 { get; set; }

        public string Text2 { get; set; }

        #endregion

        #region Fields

        public ColorPickerViewModel ViewModel = new ColorPickerViewModel();

        private bool _approximate;

        private bool _isWindowOpened = true;

        #endregion

        #region Events Handlers

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            _isWindowOpened = false;
            Close();
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            Color = ViewModel.Color;
            ColorTemperature = ViewModel.ColorTemperature;
            _isWindowOpened = false;
            Close();
        }

        private void OutsideColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsColorTemperature)
                return;

            _approximate = true;
            _hookId = SetHook(_proc);

            ViewModel.ChangeHueSaturation(new System.Windows.Point(
                (int) Math.Round(PointToScreen(e.GetPosition(this)).X) - Left -
                ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).X -
                ViewModel.ColorWheelSize / 2,
                (int) Math.Round(PointToScreen(e.GetPosition(this)).Y) - Top -
                ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).Y -
                ViewModel.ColorWheelSize / 2), _approximate);
        }

        private void ColorWheel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _hookId = SetHook(_proc);
            ViewModel.MovingPicker = true;

            if (IsColorTemperature)
            {
                ViewModel.ChangeTemperature(new System.Windows.Point(
                    (int) Math.Round(PointToScreen(e.GetPosition(this)).X) - Left -
                    ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).X -
                    ViewModel.ColorWheelSize / 2,
                    (int) Math.Round(PointToScreen(e.GetPosition(this)).Y) - Top -
                    ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).Y -
                    ViewModel.ColorWheelSize / 2));
            }
            else
            {
                _approximate = false;

                ViewModel.ChangeHueSaturation(new System.Windows.Point(
                    (int) Math.Round(PointToScreen(e.GetPosition(this)).X) - Left -
                    ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).X -
                    ViewModel.ColorWheelSize / 2,
                    (int) Math.Round(PointToScreen(e.GetPosition(this)).Y) - Top -
                    ColorWheel.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0)).Y -
                    ViewModel.ColorWheelSize / 2), _approximate);
            }
        }

        #endregion

        #region Initializers

        public ColorPicker()
        {
            InitializeComponent();
            DataContext = ViewModel;

            _proc = HookCallback;

            new Thread(() =>
            {
                while (_isWindowOpened && Properties.Settings.Default.PreviewLights)
                {
                    Thread.Sleep(500);

                    ViewModel.SetLightAsync(Brightness, Id, IsColorTemperature);
                }
            })
            { IsBackground = true }.Start();
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ColorWheelBrush =
                new ColorWheel().CreateWheelImage(ViewModel.ColorWheelSize / 2, IsColorTemperature);
            ViewModel.OutsideColorWheelBrush =
                new ColorWheel().CreateOutsideWheelImage(ViewModel.OutsideColorWheelSize / 2,
                    ViewModel.ColorWheelSize / 2, IsColorTemperature);

            if (IsColorTemperature)
                ViewModel.TemperatureSliderBrush = new ColorWheel().CreateTemperatureImage(ViewModel.SlidersHeight);

            ViewModel.MovingPicker = true;

            if (IsColorTemperature)
                ViewModel.ColorTemperature = ColorTemperature;
            else
            {
                ViewModel.Hue = Math.Round(ColorConverters.GetHue(Color));
                ViewModel.Saturation = Math.Round(ColorConverters.GetSaturation(Color) * 100);
            }

            ViewModel.Color = Color;
            ViewModel.IsColorTemperature = IsColorTemperature;
            ViewModel.MovingPicker = false;

            ViewModel.MousePosition = new Thickness(
                ViewModel.ColorWheelSize * Math.Sin(ViewModel.Hue / 360 * Math.PI * 2) * (ViewModel.Saturation / 100),
                0,
                0,
                ViewModel.ColorWheelSize * Math.Cos(ViewModel.Hue / 360 * Math.PI * 2) * (ViewModel.Saturation / 100));
        }

        #endregion
    }
}

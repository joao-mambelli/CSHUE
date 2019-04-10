using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Shell;
using MaterialDesignThemes.Wpf;
using SourceChord.FluentWPF;
using CSHUE.ViewModels;

namespace CSHUE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            SystemEvents.UserPreferenceChanged +=
                new UserPreferenceChangedEventHandler(PreferenceChangedHandler);
            PreferenceChangedHandler(new object { },
                new UserPreferenceChangedEventArgs(new UserPreferenceCategory { }));

            Home.MouseLeave += Control_MouseLeave;
            Config.MouseLeave += Control_MouseLeave;
            Donate.MouseLeave += Control_MouseLeave;
            About.MouseLeave += Control_MouseLeave;
            Settings.MouseLeave += Control_MouseLeave;

            MinimizeButton.MouseEnter += TopControls_MouseEnter;
            MinimizeButton.MouseLeave += TopControls_MouseLeave;
            MaximizeButton.MouseEnter += TopControls_MouseEnter;
            MaximizeButton.MouseLeave += TopControls_MouseLeave;
            CloseButton.MouseEnter += TopControls_MouseEnter;
            CloseButton.MouseLeave += TopControls_MouseLeave;

            Page.Navigated += Page_Navigated;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));

            viewModel.CreateInstances();
            
            viewModel.configViewModel.CheckConfigFile();
        }

        private IntPtr WindowProc(IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MaxHeight = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Window).Handle).WorkingArea.Height;

            GetCursorPos(out POINT lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0),
                MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition,
                MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam,
                typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right -
                    lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom -
                    lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right -
                    lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom -
                    lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            private POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            private POINT ptMinTrackSize;
            private POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MONITORINFO
        {
            private readonly int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            private readonly int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        private bool ButtonClickable = false;

        private bool? IsModeDark = null;
        private bool? IsTransparencyTrue = null;
        private void PreferenceChangedHandler(object sender,
            UserPreferenceChangedEventArgs e)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                {
                    if (key != null)
                    {
                        bool ChangeMenuColor = false;

                        if (key.GetValue("AppsUseLightTheme") != null)
                        {
                            bool DarkMode = !Convert.ToBoolean(key.GetValue(
                                "AppsUseLightTheme"));

                            if (IsModeDark != false && !DarkMode)
                            {
                                if (IsTransparencyTrue != true) ChangeMenuColor = true;
                            }
                            else if (IsModeDark != true && DarkMode)
                            {
                                if (IsTransparencyTrue != true) ChangeMenuColor = true;
                            }

                            IsModeDark = DarkMode;
                        }

                        if (key.GetValue("EnableTransparency") != null)
                        {
                            bool Transparency = Convert.ToBoolean(key.GetValue("EnableTransparency"));

                            if (IsTransparencyTrue != true && Transparency)
                                Menus.SetResourceReference(BackgroundProperty, "SystemAltMediumColorBrush");
                            else if ((IsTransparencyTrue != false && !Transparency) || ChangeMenuColor)
                                Menus.Background = new SolidColorBrush(IsModeDark == true ?
                                    Color.FromArgb(255, 31, 31, 31) :
                                    Color.FromArgb(255, 230, 230, 230));

                            IsTransparencyTrue = Transparency;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(64, 102, 102, 102));
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\DWM"))
            {
                if (key != null)
                {
                    object o = key.GetValue("ColorPrevalence");
                    if (o != null && Convert.ToBoolean(o))
                    {
                        BorderBrush = AccentColors.ImmersiveSystemAccentBrush;
                    }
                    else
                    {
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 102, 102, 102));
                    }
                }
            }
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid)sender).ClearValue(BackgroundProperty);

            ButtonClickable = false;
        }

        void Page_Navigated(object sender, NavigationEventArgs e)
        {
            Page.NavigationService.RemoveBackEntry();
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush(SystemTheme.Theme == ApplicationTheme.Dark ?
                Color.FromArgb(102, 255, 255, 255) :
                Color.FromArgb(102, 0, 0, 0));

            ButtonClickable = true;
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ButtonClickable)
            {
                return;
            }

            ((Grid)sender).ClearValue(BackgroundProperty);

            viewModel.Navigate(Page, sender);

            foreach (StackPanel f in Menus.Children.OfType<StackPanel>())
                foreach (Grid g in f.Children.OfType<Grid>())
                    foreach (StackPanel h in g.Children.OfType<StackPanel>())
                        foreach (Grid i in h.Children.OfType<Grid>())
                            if (i.Name == ((Grid)sender).Name + "Indicator")
                                i.Visibility = Visibility.Visible;
                            else if (i.Name.Contains("Indicator"))
                                i.Visibility = Visibility.Hidden;
        }
        
        private void TopControls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonClickable = true;

            if (((Grid)sender).Name == "CloseButton")
            {
                ((Grid)sender).Background = new SolidColorBrush(
                    Color.FromArgb(255, 241, 112, 123));
                PackIcon child = VisualTreeHelper.GetChild((Grid)sender, 0) as PackIcon;
                child.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                ((Grid)sender).Background = new SolidColorBrush(AccentColors.ImmersiveSystemAccentDark1);
            }
        }

        private void TopControls_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ButtonClickable)
            {
                return;
            }

            GetType().GetMethod(((Grid)sender).Name + "_Click").Invoke(this, null);
        }

        private void TopControls_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((Grid)sender).Name == "CloseButton")
            {
                ((Grid)sender).Background = new SolidColorBrush(
                    Color.FromArgb(255, 232, 17, 35));
            }
            else
            {
                ((Grid)sender).Background = AccentColors.ImmersiveSystemAccentBrush;
            }
        }

        private void TopControls_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonClickable = false;

            ((Grid)sender).Background = new SolidColorBrush(Colors.Transparent);

            if (((Grid)sender).Name == "CloseButton")
            {
                PackIcon child = VisualTreeHelper.GetChild((Grid)sender, 0) as PackIcon;
                child.ClearValue(ForegroundProperty);
            }
        }

        public void MinimizeButton_Click()
        {
            WindowState = WindowState.Minimized;
        }

        public void MaximizeButton_Click()
        {
            WindowState = WindowState == WindowState.Normal ?
                WindowState.Maximized :
                WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            BorderThickness = new Thickness(
                WindowState == WindowState.Maximized ? 0 : 1);

            WindowChrome.SetWindowChrome(Window, new WindowChrome()
            {
                ResizeBorderThickness = new Thickness(
                    WindowState == WindowState.Maximized ? 0 : 4)
            });

            Resources["MaximizeRestoreIcon"] =
                (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(
                    typeof(MaterialDesignThemes.Wpf.PackIconKind),
                    WindowState == WindowState.Normal ?
                    "CropSquare" : "WindowRestore");

            base.OnStateChanged(e);
        }

        public void CloseButton_Click()
        {
            Close();
        }
    }
}

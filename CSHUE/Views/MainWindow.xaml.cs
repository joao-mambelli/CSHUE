using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shell;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using SourceChord.FluentWPF;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable UnusedMember.Global
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public readonly MainWindowViewModel ViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            if (Properties.Settings.Default.Top == -1
                && Properties.Settings.Default.Left == -1)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
            {
                Top = Properties.Settings.Default.Top;
                Left = Properties.Settings.Default.Left;
            }

            InitializeComponent();
            DataContext = ViewModel;
            SystemEvents.UserPreferenceChanged += PreferenceChangedHandler;
            PreferenceChangedHandler(new object(), new UserPreferenceChangedEventArgs(new UserPreferenceCategory()));
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

            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            ViewModel.CreateInstances();

            SetLanguage();

            if (Properties.Settings.Default.RunOnStartup)
                ViewModel.SettingsPage.ViewModel.AddStartup(Properties.Settings.Default.RunOnStartupMinimized);
            else
                ViewModel.SettingsPage.ViewModel.RemoveStartup();

            var arguments = Environment.GetCommandLineArgs();
            foreach (var s in arguments)
            {
                switch (s)
                {
                    case "-silent":
                        if (Properties.Settings.Default.RunOnStartupMinimized && Properties.Settings.Default.MinimizeToSystemTray)
                        {
                            WindowState = WindowState.Minimized;
                            Hide();
                            NotifyIcon.Visibility = Visibility.Visible;
                        }
                        else if (Properties.Settings.Default.RunOnStartupMinimized)
                            WindowState = WindowState.Minimized;

                        ViewModel.Navigate(Page, "Home");
                        break;
                    case "-lang":
                        ViewModel.Navigate(Page, "Settings");
                        break;
                    case "-reset":
                        ViewModel.Resetting = true;
                        ViewModel.Navigate(Page, "Home");
                        break;
                    default:
                        ViewModel.Navigate(Page, "Home");
                        break;
                }
            }

            ViewModel.HueAsync();

            ViewModel.Csgo();

            ViewModel.SettingsPage.ViewModel.UpdateGradients();
        }

        private static void SetLanguage()
        {
            if (Properties.Settings.Default.Language == -1)
            {
                var culture = CultureResources.SupportedCultures.Contains(CultureInfo.InstalledUICulture)
                    ? CultureInfo.InstalledUICulture
                    : new CultureInfo("en-US");

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                CultureResources.ChangeCulture(culture);

                Properties.Settings.Default.Language = Helpers.Converters.GetIndexFromCultureInfo(culture);
            }
            else
            {
                var culture = Helpers.Converters.GetCultureInfoFromIndex(Properties.Settings.Default.Language);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                CultureResources.ChangeCulture(culture);
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var mWindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(mWindowHandle)
                ?.AddHook(WindowProc);

            if (Properties.Settings.Default.Maximized)
                WindowState = WindowState.Maximized;
        }

        private IntPtr WindowProc(IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (msg == 0x0024)
                WmGetMinMaxInfo(lParam);
            if (msg == NativeMethods.WmShowme)
                ShowMe();
            return IntPtr.Zero;
        }

        private void ShowMe()
        {
            Show();
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            NotifyIcon.Visibility = Visibility.Collapsed;
            var top = Topmost;
            Topmost = true;
            Topmost = top;
        }

        private void WmGetMinMaxInfo(IntPtr lParam)
        {
            MaxHeight = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Window).Handle).WorkingArea.Height;
            GetCursorPos(out var lMousePosition);
            var lPrimaryScreen = MonitorFromPoint(new Point(0, 0), MonitorOptions.MonitorDefaulttoprimary);
            var lPrimaryScreenInfo = new Monitorinfo();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false) return;
            var lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MonitorDefaulttonearest);
            var lMmi = (Minmaxinfo) Marshal.PtrToStructure(lParam, typeof(Minmaxinfo));
            if (lPrimaryScreen.Equals(lCurrentScreen))
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(Point pt, MonitorOptions dwFlags);

        private enum MonitorOptions : uint
        {
            //MonitorDefaulttonull = 0x00000000,
            MonitorDefaulttoprimary = 0x00000001,
            MonitorDefaulttonearest = 0x00000002
        }

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, Monitorinfo lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Minmaxinfo
        {
            private readonly Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            private readonly Point ptMinTrackSize;
            private readonly Point ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class Monitorinfo
        {
            // ReSharper disable once UnusedMember.Local
            private readonly int cbSize = Marshal.SizeOf(typeof(Monitorinfo));
            public readonly Rect rcMonitor = new Rect();
            public readonly Rect rcWork = new Rect();
            #pragma warning disable 169
            #pragma warning disable 414
            private readonly int dwFlags = 0;
            #pragma warning restore 414
            #pragma warning restore 169
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left, Top, Right, Bottom;
        }

        private bool _buttonClickable;
        private bool? _isModeDark;
        private bool? _isTransparencyTrue;

        private void PreferenceChangedHandler(object sender, UserPreferenceChangedEventArgs e)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                {
                    if (key == null)
                    {
                        Menus.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                        return;
                    }
                    var changeMenuColor = false;
                    if (key.GetValue("AppsUseLightTheme") != null)
                    {
                        var darkMode = !Convert.ToBoolean(key.GetValue("AppsUseLightTheme"));
                        if (_isModeDark != false && !darkMode)
                        {
                            if (_isTransparencyTrue != true)
                                changeMenuColor = true;
                        }
                        else if (_isModeDark != true && darkMode)
                        {
                            if (_isTransparencyTrue != true)
                                changeMenuColor = true;
                        }

                        _isModeDark = darkMode;
                    }

                    if (key.GetValue("EnableTransparency") == null)
                    {
                        Menus.SetResourceReference(BackgroundProperty, "SystemAltMediumColorBrush");
                        return;
                    }
                    var transparency = Convert.ToBoolean(key.GetValue("EnableTransparency"));
                    if (_isTransparencyTrue != true && transparency)
                        Menus.SetResourceReference(BackgroundProperty, "SystemAltMediumColorBrush");
                    else if (_isTransparencyTrue != false && !transparency || changeMenuColor)
                        Menus.Background = new SolidColorBrush(_isModeDark == true
                            ? Color.FromRgb(31, 31, 31)
                            : Color.FromRgb(230, 230, 230));
                    _isTransparencyTrue = transparency;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(64, 102, 102, 102));
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\DWM"))
            {
                if (key == null) return;
                var o = key.GetValue("ColorPrevalence");
                if (o != null && Convert.ToBoolean(o))
                    BorderBrush = AccentColors.ImmersiveSystemAccentBrush;
                else
                    BorderBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            }
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid) sender).ClearValue(BackgroundProperty);
            _buttonClickable = false;
        }

        private void Page_Navigated(object sender, NavigationEventArgs e)
        {
            Page.NavigationService.RemoveBackEntry();
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Grid) sender).Background = new SolidColorBrush(SystemTheme.Theme == ApplicationTheme.Dark
                ? Color.FromArgb(102, 255, 255, 255)
                : Color.FromArgb(102, 0, 0, 0));
            _buttonClickable = true;
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_buttonClickable) return;
            ((Grid) sender).ClearValue(BackgroundProperty);
            ViewModel.Navigate(Page, sender);
            foreach (var f in Menus.Children.OfType<StackPanel>())
                foreach (var g in f.Children.OfType<Grid>())
                    foreach (var h in g.Children.OfType<StackPanel>())
                        foreach (var i in h.Children.OfType<Grid>())
                            if (i.Name == ((Grid) sender).Name + "Indicator")
                                i.Visibility = Visibility.Visible;
                            else if (i.Name.Contains("Indicator"))
                                i.Visibility = Visibility.Hidden;
        }

        private void TopControls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _buttonClickable = true;
            if (((Grid) sender).Name == "CloseButton")
            {
                ((Grid) sender).Background = new SolidColorBrush(Color.FromRgb(241, 112, 123));
                if (VisualTreeHelper.GetChild((Grid) sender, 0) is PackIcon child)
                    child.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
                ((Grid) sender).Background = new SolidColorBrush(AccentColors.ImmersiveSystemAccentDark1);
        }

        private void TopControls_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_buttonClickable)
                return;
            GetType().GetMethod(((Grid) sender).Name + "_Click")?.Invoke(this, null);
        }

        private static void TopControls_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Grid) sender).Background = ((Grid) sender).Name == "CloseButton"
                ? new SolidColorBrush(Color.FromRgb(232, 17, 35))
                : AccentColors.ImmersiveSystemAccentBrush;
        }

        private void TopControls_MouseLeave(object sender, MouseEventArgs e)
        {
            _buttonClickable = false;
            ((Grid) sender).Background = new SolidColorBrush(Colors.Transparent);
            if (((Grid) sender).Name != "CloseButton") return;
            if (VisualTreeHelper.GetChild((Grid) sender, 0) is PackIcon child)
                child.ClearValue(ForegroundProperty);
        }

        public void MinimizeButton_Click()
        {
            WindowState = WindowState.Minimized;
        }

        public void MaximizeButton_Click()
        {
            WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        protected sealed override void OnStateChanged(EventArgs e)
        {
            BorderThickness = new Thickness(WindowState == WindowState.Maximized ? 0 : 1);
            WindowChrome.SetWindowChrome(Window,
                new WindowChrome
                {
                    ResizeBorderThickness = new Thickness(WindowState == WindowState.Maximized
                        ? 0
                        : 4)
                });
            Resources["MaximizeRestoreIcon"] = (PackIconKind) Enum.Parse(typeof(PackIconKind),
                WindowState == WindowState.Normal
                    ? "CropSquare"
                    : "WindowRestore");

            if (WindowState == WindowState.Minimized && Properties.Settings.Default.MinimizeToSystemTray)
            {
                Hide();
                NotifyIcon.Visibility = Visibility.Visible;
            }

            base.OnStateChanged(e);
        }

        public void CloseButton_Click()
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.RestoreLights();

            NotifyIcon.Visibility = Visibility.Collapsed;

            if (WindowState == WindowState.Maximized
                || WindowState == WindowState.Minimized)
            {
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = WindowState == WindowState.Maximized;
            }
            else
            {
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Height = Height;
                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            NotifyIcon.Visibility = Visibility.Collapsed;
        }

        private void MenuNavigate_Click(object sender, RoutedEventArgs e)
        {
            NotifyIcon_TrayMouseDoubleClick(sender, e);

            ViewModel.Navigate(Page, ((MenuItem)sender).Tag.ToString());
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ((ContextMenu)sender).Items[0] = new MenuItem
            {
                Header = Cultures.Resources.Home,
                Style = (Style)FindResource("Windows10MenuItemAlertHome"),
                Tag = "Home"
            };
            ((MenuItem)((ContextMenu)sender).Items[0]).Click += MenuNavigate_Click;

            ((ContextMenu)sender).Items[1] = new MenuItem
            {
                Header = Cultures.Resources.CSGOGSI,
                Style = (Style)FindResource("Windows10MenuItemAlertConfig"),
                Tag = "Config"
            };
            ((MenuItem)((ContextMenu)sender).Items[1]).Click += MenuNavigate_Click;
        }
    }
}

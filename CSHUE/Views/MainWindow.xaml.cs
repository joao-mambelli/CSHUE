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
using System.Windows.Shapes;
using CSHUE.Controls;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using Microsoft.Win32;
using SourceChord.FluentWPF;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Low Level Window Hook

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0024)
                WmGetMinMaxInfo(lParam);

            if (msg == NativeMethods.WmShowme)
                ShowMe();

            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(IntPtr lParam)
        {
            MaxHeight = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Height;
            GetCursorPos(out var lMousePosition);

            var lPrimaryScreen = MonitorFromPoint(new Point(0, 0), MonitorOptions.MonitorDefaulttoprimary);
            var lPrimaryScreenInfo = new Monitorinfo();

            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                return;

            var lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MonitorDefaulttonearest);
            var lMmi = (Minmaxinfo)Marshal.PtrToStructure(lParam, typeof(Minmaxinfo));

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

        #endregion

        #region Fields

        public readonly MainWindowViewModel ViewModel = new MainWindowViewModel();

        private bool _buttonClickable;
        private SplashScreen _splashScreen;

        #endregion

        #region Initializers

        public MainWindow()
        {
            SetLanguage();

            if (Properties.Settings.Default.Top == -1 &&
                Properties.Settings.Default.Left == -1)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
            {
                Top = Properties.Settings.Default.Top;
                Left = Properties.Settings.Default.Left;
            }

            InitializeComponent();
            DataContext = ViewModel;
            SystemEvents.UserPreferenceChanged += OnPreferenceChangedHandler;
            OnPreferenceChangedHandler(new object(), new UserPreferenceChangedEventArgs(new UserPreferenceCategory()));
            Home.MouseLeave += Control_OnMouseLeave;
            Config.MouseLeave += Control_OnMouseLeave;
            Donate.MouseLeave += Control_OnMouseLeave;
            About.MouseLeave += Control_OnMouseLeave;
            Help.MouseLeave += Control_OnMouseLeave;
            Update.MouseLeave += Control_OnMouseLeave;
            Settings.MouseLeave += Control_OnMouseLeave;
            MinimizeButton.MouseEnter += TopControls_OnMouseEnter;
            MinimizeButton.MouseLeave += TopControls_OnMouseLeave;
            MaximizeButton.MouseEnter += TopControls_OnMouseEnter;
            MaximizeButton.MouseLeave += TopControls_OnMouseLeave;
            CloseButton.MouseEnter += TopControls_OnMouseEnter;
            CloseButton.MouseLeave += TopControls_OnMouseLeave;

            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            ViewModel.CreateInstances();

            if (Properties.Settings.Default.RunOnStartup)
                ViewModel.Settings.ViewModel.AddStartup(Properties.Settings.Default.RunOnStartupMinimized);
            else
                ViewModel.Settings.ViewModel.RemoveStartup();

            if (Properties.Settings.Default.ShowSystemTrayIcon)
                ViewModel.NotifyIconVisibility = Visibility.Visible;

            if (Properties.Settings.Default.FirstLaunch)
                ViewModel.Navigate("Home");
            else
            {
                var arguments = Environment.GetCommandLineArgs();
                foreach (var s in arguments)
                {
                    switch (s)
                    {
                        case "-silent":
                            if (Properties.Settings.Default.RunOnStartupMinimized &&
                                Properties.Settings.Default.MinimizeToSystemTray &&
                                Properties.Settings.Default.ShowSystemTrayIcon)
                            {
                                WindowState = WindowState.Minimized;
                                Hide();
                            }
                            else if (Properties.Settings.Default.RunOnStartupMinimized)
                                WindowState = WindowState.Minimized;

                            ViewModel.Navigate("Home");
                            break;
                        case "-lang":
                            ViewModel.Navigate("Settings");
                            UpdateIndicatorPosition("Settings");
                            break;
                        default:
                            ViewModel.Navigate("Home");
                            Topmost = true;
                            Topmost = false;
                            break;
                    }
                }
            }

            ViewModel.HueAsync();

            ViewModel.Csgo();

            ViewModel.Settings.ViewModel.UpdateGradients();
        }

        private static void SetLanguage()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.LanguageName))
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

                Properties.Settings.Default.LanguageName = CultureResources.SupportedCultures.Find(x => Equals(x, culture)).NativeName;
            }
            else
            {
                var culture = CultureResources.SupportedCultures.Find(x => x.NativeName == Properties.Settings.Default.LanguageName);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                CultureResources.ChangeCulture(culture);
            }
        }

        #endregion

        #region Events Handlers

        private void OnInitialized(object sender, EventArgs e)
        {
            var lastVersion = ViewModel.GetLastVersion();

            if (lastVersion != "" && Properties.Settings.Default.LatestVersionCheck != lastVersion)
            {
                new CustomMessageBox
                {
                    Button1 = new CustomButton
                    {
                        Text = Cultures.Resources.Ok
                    },
                    Button2 = new CustomButton
                    {
                        Text = Cultures.Resources.ShowInBrowser,
                        Path = $"https://github.com/joao7yt/CSHUE/releases/tag/{lastVersion}",
                        DialogResult = null
                    },
                    Message = string.Format(Cultures.Resources.NewVersionMessage, lastVersion),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ShowInTaskbar = true
                }.ShowDialog();

                Properties.Settings.Default.LatestVersionCheck = lastVersion;
            }

            var arguments = Environment.GetCommandLineArgs();
            if (!arguments.Contains("-silent") && !arguments.Contains("-lang"))
                try
                {
                    _splashScreen = new SplashScreen("logo.png");
                    _splashScreen.Show(false, true);
                    Thread.Sleep(500);
                }
                catch
                {
                    // ignored
                }

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            HwndSource.FromHwnd(helper.Handle)?.AddHook(WindowProc);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Maximized)
                WindowState = WindowState.Maximized;

            _splashScreen?.Close(TimeSpan.FromSeconds(.3));
        }

        private void OnPreferenceChangedHandler(object sender, UserPreferenceChangedEventArgs e)
        {
            switch (Properties.Settings.Default.Theme)
            {
                case 0:
                    ResourceDictionaryEx.GlobalTheme = ElementTheme.Default;
                    break;
                case 1:
                    ResourceDictionaryEx.GlobalTheme = ElementTheme.Dark;
                    break;
                default:
                    ResourceDictionaryEx.GlobalTheme = ElementTheme.Light;
                    break;
            }

            if (ResourceDictionaryEx.GlobalTheme == ElementTheme.Default)
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(
                        "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                    {
                        if (key == null)
                        {
                            if (Properties.Settings.Default.Transparency == 0 || Properties.Settings.Default.Transparency == 1)
                            {
                                ViewModel.BackgroundColor = (Color)FindResource("SystemAltLowColor");
                                ViewModel.IsModeDark = false;
                                ViewModel.IsTransparencyTrue = true;
                            }
                            else
                            {
                                ViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                                ViewModel.IsModeDark = false;
                                ViewModel.IsTransparencyTrue = false;
                            }

                            return;
                        }

                        ViewModel.IsModeDark = key.GetValue("AppsUseLightTheme") != null && !Convert.ToBoolean(key.GetValue("AppsUseLightTheme"));

                        if (Properties.Settings.Default.Transparency == 0)
                            ViewModel.IsTransparencyTrue = key.GetValue("EnableTransparency") == null || Convert.ToBoolean(key.GetValue("EnableTransparency"));
                        else
                            ViewModel.IsTransparencyTrue = Properties.Settings.Default.Transparency == 1;

                        if (ViewModel.IsTransparencyTrue == true)
                            ViewModel.BackgroundColor = (Color)FindResource("SystemAltLowColor");
                        else if (ViewModel.IsModeDark == true)
                            ViewModel.BackgroundColor = Color.FromRgb(31, 31, 31);
                        else
                            ViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                    }
                }
                catch
                {
                    // ignored
                }

            if (Properties.Settings.Default.Transparency == 0)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(
                        "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                    {
                        if (key == null)
                        {
                            ViewModel.BackgroundColor =
                                ViewModel.IsModeDark == true
                                    ? Color.FromArgb(51, 0, 0, 0)
                                    : Color.FromArgb(51, 255, 255, 255);

                            ViewModel.IsTransparencyTrue = true;

                            return;
                        }

                        ViewModel.IsTransparencyTrue = key.GetValue("EnableTransparency") == null || Convert.ToBoolean(key.GetValue("EnableTransparency"));

                        if (ViewModel.IsTransparencyTrue == true)
                            ViewModel.BackgroundColor =
                                ViewModel.IsModeDark == true
                                    ? Color.FromArgb(51, 0, 0, 0)
                                    : Color.FromArgb(51, 255, 255, 255);
                        else
                            ViewModel.BackgroundColor =
                                ViewModel.IsModeDark == true
                                    ? Color.FromRgb(31, 31, 31)
                                    : Color.FromRgb(230, 230, 230);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            BorderBrush = new SolidColorBrush(Color.FromArgb(64, 102, 102, 102));
        }

        private void OnActivated(object sender, EventArgs e)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\DWM"))
            {
                if (key != null)
                {
                    var o = key.GetValue("ColorPrevalence");
                    if (o != null && Convert.ToBoolean(o))
                        BorderBrush = AccentColors.ImmersiveSystemAccentBrush;
                    else
                        BorderBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102));
                }
            }
        }

        private void Control_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid)sender).ClearValue(BackgroundProperty);
            _buttonClickable = false;
        }

        private void Control_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ResourceDictionaryEx.GlobalTheme == ElementTheme.Default)
                ((Grid)sender).Background = new SolidColorBrush(SystemTheme.Theme == ApplicationTheme.Dark
                    ? Color.FromArgb(102, 255, 255, 255)
                    : Color.FromArgb(102, 0, 0, 0));
            else
                ((Grid)sender).Background = new SolidColorBrush(ResourceDictionaryEx.GlobalTheme == ElementTheme.Dark
                    ? Color.FromArgb(102, 255, 255, 255)
                    : Color.FromArgb(102, 0, 0, 0));

            _buttonClickable = true;
        }

        private void Control_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_buttonClickable)
            {
                ((Grid)sender).ClearValue(BackgroundProperty);
                ViewModel.Navigate(((Grid)sender).Name);

                if (((Grid)sender).Name == "Settings" && Properties.Settings.Default.FirstLaunch)
                {
                    Properties.Settings.Default.FirstLaunch = false;
                    Properties.Settings.Default.Save();
                }

                UpdateIndicatorPosition(((Grid)sender).Name);
            }
        }

        private void TopControls_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _buttonClickable = true;
            if (((Grid)sender).Name == "CloseButton")
            {
                ((Grid)sender).Background = new SolidColorBrush(Color.FromRgb(241, 112, 123));
                if (VisualTreeHelper.GetChild((Grid)sender, 0) is Path child)
                    child.Stroke = new SolidColorBrush(Colors.Black);
            }
            else
                ((Grid)sender).Background = new SolidColorBrush(AccentColors.ImmersiveSystemAccentLight1);
        }

        private void TopControls_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_buttonClickable)
                GetType().GetMethod(((Grid)sender).Name + "_Click")?.Invoke(this, null);
        }

        private static void TopControls_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ((Grid)sender).Background = ((Grid)sender).Name == "CloseButton"
                ? new SolidColorBrush(Color.FromRgb(232, 17, 35))
                : AccentColors.ImmersiveSystemAccentBrush;

            if (VisualTreeHelper.GetChild((Grid)sender, 0) is Path child)
                child.Stroke = new SolidColorBrush(Colors.White);
        }

        private void TopControls_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _buttonClickable = false;
            ((Grid)sender).Background = new SolidColorBrush(Colors.Transparent);
            if (VisualTreeHelper.GetChild((Grid)sender, 0) is Path child)
                child.SetResourceReference(Shape.StrokeProperty, "SystemBaseHighColorBrush");
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
            WindowChrome.ResizeBorderThickness = new Thickness(WindowState == WindowState.Maximized ? 0 : 4);
            WindowChrome.CaptionHeight = WindowState == WindowState.Maximized ? 33 : 30;

            ViewModel.MaximizeRestore = WindowState == WindowState.Maximized
                ? Geometry.Parse((string)Application.Current.Resources["Restore"])
                : Geometry.Parse((string)Application.Current.Resources["Maximize"]);

            if (WindowState == WindowState.Minimized &&
                Properties.Settings.Default.MinimizeToSystemTray &&
                Properties.Settings.Default.ShowSystemTrayIcon)
                Hide();

            if (WindowState == WindowState.Minimized)
            {
                ViewModel.WindowMinimized = true;
            }
            else
            {
                ViewModel.WindowMinimized = false;

                if (ViewModel.AllowStartLightsChecking)
                    ViewModel.Home.StartLightsChecking();
            }

            base.OnStateChanged(e);
        }

        public void CloseButton_Click()
        {
            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            ViewModel.RestoreLights();
            ViewModel.NotifyIconVisibility = Visibility.Collapsed;

            if (WindowState == WindowState.Maximized ||
                WindowState == WindowState.Minimized)
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

            foreach (var w in Application.Current.Windows)
            {
                ((Window)w).Hide();
            }

            Environment.Exit(0);
        }

        private void NotifyIcon_OnTrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            ShowMe();
        }

        private void MenuNavigate_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMe();

            ViewModel.Navigate(((MenuItem)sender).Tag.ToString());

            UpdateIndicatorPosition(((MenuItem)sender).Tag.ToString());
        }

        private void MenuActivated_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Activated = !Properties.Settings.Default.Activated;
        }

        private void MenuExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            ((ContextMenu)sender).Items[0] = new MenuItem
            {
                Header = Cultures.Resources.Home,
                Style = (Style)FindResource("CustomMenuItemAlertHome"),
                Tag = "Home"
            };
            ((MenuItem)((ContextMenu)sender).Items[0]).Click += MenuNavigate_OnClick;

            ((ContextMenu)sender).Items[1] = new MenuItem
            {
                Header = Cultures.Resources.CSGOGSI,
                Style = (Style)FindResource("CustomMenuItemAlertConfig"),
                Tag = "Config"
            };
            ((MenuItem)((ContextMenu)sender).Items[1]).Click += MenuNavigate_OnClick;

            ((ContextMenu)sender).Items[3] = new MenuItem
            {
                Header = Cultures.Resources.Update,
                Style = (Style)FindResource("CustomMenuItemAlertUpdate"),
                Tag = "Update"
            };
            ((MenuItem)((ContextMenu)sender).Items[3]).Click += MenuNavigate_OnClick;

            ((ContextMenu)sender).Items[11] = new MenuItem
            {
                Header = Properties.Settings.Default.Activated
                    ? Cultures.Resources.TurnOff
                    : Cultures.Resources.TurnOn,
                Style = Properties.Settings.Default.Activated
                    ? (Style)FindResource("CustomMenuItemOn")
                    : (Style)FindResource("CustomMenuItemOff")
            };
            ((MenuItem)((ContextMenu)sender).Items[11]).Click += MenuActivated_OnClick;
        }

        private void RunCsgo_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RunCsgo();
        }

        #endregion

        #region Methods

        private void ShowMe()
        {
            Hide();
            Show();
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            Topmost = false;
            Topmost = true;
            Topmost = false;
        }

        public void UpdateIndicatorPosition(string name)
        {
            foreach (var f in Menus.Children.OfType<StackPanel>())
                foreach (var g in f.Children.OfType<Grid>())
                    foreach (var h in g.Children.OfType<StackPanel>())
                        foreach (var i in h.Children.OfType<Grid>())
                            if (i.Name == name + "Indicator")
                                i.Visibility = Visibility.Visible;
                            else if (i.Name.Contains("Indicator"))
                                i.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}

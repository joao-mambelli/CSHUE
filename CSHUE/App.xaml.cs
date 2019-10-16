using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CSHUE.Controls;

namespace CSHUE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Fields

        private static readonly Mutex Mutex = new Mutex(true, "CSHUE");

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterLogException();

            base.OnStartup(e);
        }

        private void RegisterLogException()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, exception) => LogUnhandledException((Exception)exception.ExceptionObject);

            DispatcherUnhandledException += (s, exception) => DispatcherUnhandled(exception);

            TaskScheduler.UnobservedTaskException += (s, exception) => LogUnhandledException(exception.Exception);
        }

        private static void DispatcherUnhandled(DispatcherUnhandledExceptionEventArgs exception)
        {
            LogUnhandledException(exception.Exception);

            if (exception.Exception is COMException comException && comException.ErrorCode == -2147221040)
                exception.Handled = true;
        }

        private static void LogUnhandledException(Exception exception)
        {
            var tempException = exception;
            var tempDefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
            var tempDefaultThreadCurrentUiCulture = CultureInfo.DefaultThreadCurrentUICulture;
            var tempCurrentCulture = Thread.CurrentThread.CurrentCulture;
            var tempCurrentUiCulture = Thread.CurrentThread.CurrentUICulture;

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Directory.CreateDirectory(baseDirectory + "\\logs");

            var errorContent = $"Version:    {Core.Utilities.Version.CurrentVersion}\n";
            errorContent += $"HResult:    {exception.HResult}\n";
            errorContent += $"HelpLink:   {exception.HelpLink}\n";
            errorContent += $"Message:    {exception.Message}\n";
            errorContent += $"Source:     {exception.Source}\n";
            errorContent += $"StackTrace: {exception.StackTrace}\n\n";

            errorContent = Regex.Replace(errorContent, @"\n   (\w)", "\n            $1");

            var tabs = "\t";
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                errorContent += $"{tabs}HResult:    {exception.HResult}\n";
                errorContent += $"{tabs}HelpLink:   {exception.HelpLink}\n";
                errorContent += $"{tabs}Message:    {exception.Message}\n";
                errorContent += $"{tabs}Source:     {exception.Source}\n";
                errorContent += $"{tabs}StackTrace: {exception.StackTrace}\n\n";

                errorContent = Regex.Replace(errorContent, @"\n   (\w)", $"\n{tabs}\t            $1");

                tabs += "\t";
            }

            errorContent = Regex.Replace(errorContent, "(StackTrace: )   ", "$1").Trim();

            var file = baseDirectory + $"logs\\log-{DateTime.Now.ToUniversalTime():yyyy_MM_dd_HH_mm_ss}";
            var sur = "";
            var id = 2;
            while (File.Exists($"{file}{sur}.log"))
            {
                sur = sur == "" ? "_(1)" : $"_({id++})";
            }

            File.AppendAllText($"{file}{sur}.log", errorContent);

            exception = tempException;

            errorContent = $"HResult:    {exception.HResult}\n";
            errorContent += $"HelpLink:   {exception.HelpLink}\n";
            errorContent += $"Message:    {exception.Message}\n";
            errorContent += $"Source:     {exception.Source}\n\n";

            errorContent = Regex.Replace(errorContent, @"\n   (\w)", "\n            $1");

            tabs = "\t";
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                errorContent += $"{tabs}HResult:    {exception.HResult}\n";
                errorContent += $"{tabs}HelpLink:   {exception.HelpLink}\n";
                errorContent += $"{tabs}Message:    {exception.Message}\n";
                errorContent += $"{tabs}Source:     {exception.Source}\n\n";

                errorContent = Regex.Replace(errorContent, @"\n   (\w)", $"\n{tabs}\t            $1");

                tabs += "\t";
            }

            CultureInfo.DefaultThreadCurrentCulture = tempDefaultThreadCurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = tempDefaultThreadCurrentUiCulture;
            Thread.CurrentThread.CurrentCulture = tempCurrentCulture;
            Thread.CurrentThread.CurrentUICulture = tempCurrentUiCulture;

            var url = "https://github.com/joao7yt/CSHUE/issues/new?title=";
            url += Uri.EscapeDataString($"Crash report. UTC: {DateTime.Now.ToUniversalTime():yyyy/MM/dd HH:mm:ss}");
            url += "&body=";
            url += Uri.EscapeDataString($"({Cultures.Resources.ExtraInfo})\n```\n{errorContent.Trim()}\n```");

            new CustomMessageBox
            {
                Button1 = new CustomButton
                {
                    Text = Cultures.Resources.Ok
                },
                Button2 = new CustomButton
                {
                    Text = Cultures.Resources.CreateIssue,
                    Path = url
                },
                Button3 = new CustomButton
                {
                    Text = Cultures.Resources.ShowInFolder,
                    Path = $"{file}{sur}.log",
                    ShowInFolder = true,
                    DialogResult = null
                },
                Message = string.Format(Cultures.Resources.CrashMessage, $"{file}{sur}.log"),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowInTaskbar = true
            }.ShowDialog();
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThreadAttribute]
        [DebuggerNonUserCodeAttribute]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            var allow = false;
            foreach (var s in Environment.GetCommandLineArgs())
            {
                if (s != "-reset" && s != "-lang")
                    continue;

                allow = true;
            }

            if (allow || Mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
                Mutex.ReleaseMutex();
            }
            else
            {
                NativeMethods.PostMessage((IntPtr) NativeMethods.HwndBroadcast, NativeMethods.WmShowme, IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }

    internal class NativeMethods
    {
        public const int HwndBroadcast = 0xffff;
        public static readonly int WmShowme = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}

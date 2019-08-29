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
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");

            var errorContent =
                $"HResult:    {exception.HResult}\nHelpLink:   {exception.HelpLink}\nMessage:    {exception.Message}\nSource:     {exception.Source}\nStackTrace: {exception.StackTrace}";

            errorContent = Regex.Replace(errorContent, @"\n   (\w)", "\n            $1");

            var tabs = "";
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                errorContent +=
                    $"\n\n{tabs}\tHResult:    {exception.HResult}\n{tabs}\tHelpLink:   {exception.HelpLink}\n{tabs}\tMessage:    {exception.Message}\n{tabs}\tSource:     {exception.Source}\n{tabs}\tStackTrace: {exception.StackTrace}";

                errorContent = Regex.Replace(errorContent, @"\n   (\w)", $"\n{tabs}\t            $1");

                tabs += "\t";
            }

            errorContent = Regex.Replace(errorContent, "(StackTrace: )   ", "$1");

            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "logs\\cshue-log.log", $"{errorContent}\n\n-------------------------------------------------------\n\n");

            Process.Start("https://github.com/joao7yt/CSHUE/issues/new?title=Crash+report&labels=crash&body=" + Uri.EscapeDataString($"```\nUTC: {DateTime.Now.ToUniversalTime():yyyy/MM/dd HH:mm:ss}\n\n{errorContent}\n```"));
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

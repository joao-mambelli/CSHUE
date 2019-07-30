using System;
using System.IO;
using System.Runtime.InteropServices;
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
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/log/cshue-log.log");

            var errorContent =
                string.Format("HResult: {1}{0} HelpLink: {2}{0} Message: {3}{0} Source: {4}{0} StackTrace: {5}{0} {0}",
                    Environment.NewLine,
                    exception.HResult,
                    exception.HelpLink,
                    exception.Message,
                    exception.Source,
                    exception.StackTrace);

            File.AppendAllText(fileName, errorContent);
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThreadAttribute]
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
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

using System;
using System.Runtime.InteropServices;
using System.Threading;
// ReSharper disable InheritdocConsiderUsage

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
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HwndBroadcast,
                    NativeMethods.WmShowme,
                    IntPtr.Zero,
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

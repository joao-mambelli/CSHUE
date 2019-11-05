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

            Directory.CreateDirectory(baseDirectory + "logs");

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

                errorContent = Regex.Replace(errorContent, @"\n   (\w)", $"\n{tabs}\t    $1");

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

            errorContent = $"Version:    {Core.Utilities.Version.CurrentVersion}\n";
            errorContent += $"HResult:    {exception.HResult}\n";
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

                errorContent = Regex.Replace(errorContent, @"\n   (\w)", $"\n{tabs}\t    $1");

                tabs += "\t";
            }

            CultureInfo.DefaultThreadCurrentCulture = tempDefaultThreadCurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = tempDefaultThreadCurrentUiCulture;
            Thread.CurrentThread.CurrentCulture = tempCurrentCulture;
            Thread.CurrentThread.CurrentUICulture = tempCurrentUiCulture;

            var url = "https://github.com/joao7yt/CSHUE/issues/new?title=";
            url += Uri.EscapeDataString($"Crash report. UTC: {DateTime.Now.ToUniversalTime():yyyy/MM/dd HH:mm:ss}");
            url += "&body=";
            url += Uri.EscapeDataString($"(Extra information here)\n```\n{errorContent.Trim()}\n```");

            new CustomMessageBox
            {
                Button1 = new CustomButton
                {
                    Text = Cultures.Resources.OkButton
                },
                Button2 = new CustomButton
                {
                    Text = Cultures.Resources.CreateIssueButton,
                    Path = url
                },
                Button3 = new CustomButton
                {
                    Text = Cultures.Resources.ShowInFolderButton,
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
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (!File.Exists($"{baseDirectory}CSHUE.exe.config"))
                {
                    File.AppendAllText($"{baseDirectory}CSHUE.exe.config", @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <configSections>
    <sectionGroup name=""userSettings"" type=""System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
      <section name=""CSHUE.Properties.Settings"" type=""System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" allowExeDefinition=""MachineToLocalUser"" requirePermission=""false""/>
    </sectionGroup>
  </configSections>
  <startup>
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.7.2""/>
  </startup>
  <userSettings>
    <CSHUE.Properties.Settings>
      <setting name=""RunOnStartup"" serializeAs=""String"">
        <value>False</value>
      </setting>
      <setting name=""Activated"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""Top"" serializeAs=""String"">
        <value>-1</value>
      </setting>
      <setting name=""Left"" serializeAs=""String"">
        <value>-1</value>
      </setting>
      <setting name=""Height"" serializeAs=""String"">
        <value>450</value>
      </setting>
      <setting name=""Width"" serializeAs=""String"">
        <value>650</value>
      </setting>
      <setting name=""Maximized"" serializeAs=""String"">
        <value>False</value>
      </setting>
      <setting name=""PlayerGetsKillDuration"" serializeAs=""String"">
        <value>1.5</value>
      </setting>
      <setting name=""PlayerGetsKilledDuration"" serializeAs=""String"">
        <value>3</value>
      </setting>
      <setting name=""RunOnStartupMinimized"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""MinimizeToSystemTray"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""AutoMinimize"" serializeAs=""String"">
        <value>False</value>
      </setting>
      <setting name=""TriggerSpecEvents"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""RememberLightsStates"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""AutoActivate"" serializeAs=""String"">
        <value>False</value>
      </setting>
      <setting name=""AutoActivateStart"" serializeAs=""String"">
        <value>18:00</value>
      </setting>
      <setting name=""AutoActivateEnd"" serializeAs=""String"">
        <value>06:00</value>
      </setting>
      <setting name=""RunCsgo"" serializeAs=""String"">
        <value>False</value>
      </setting>
      <setting name=""LaunchOptions"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""LanguageName"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""CsgoFolder"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""SteamFolder"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""AppKey"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""PreviewLights"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""ShowSystemTrayIcon"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""AutoDeactivate"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""LatestVersionCheck"" serializeAs=""String"">
        <value />
      </setting>
      <setting name=""FirstLaunch"" serializeAs=""String"">
        <value>True</value>
      </setting>
      <setting name=""Theme"" serializeAs=""String"">
        <value>0</value>
      </setting>
      <setting name=""Transparency"" serializeAs=""String"">
        <value>0</value>
      </setting>
      <setting name=""BrightnessModifier"" serializeAs=""String"">
        <value>100</value>
      </setting>
      <setting name=""BridgeId"" serializeAs=""String"">
        <value />
      </setting>
    </CSHUE.Properties.Settings>
  </userSettings>
</configuration>
");
                }

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

using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Media;

namespace CSHUE.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public MainWindowViewModel MainWindowViewModel = null;

        public void AddStartup(bool silent)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (silent)
                rk.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" -silent");
            else
                rk.SetValue("CSHUE", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\"");
        }

        public void RemoveStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.DeleteValue("CSHUE", false);
        }

        private GradientStopCollection _gradientStopsMainMenu;
        public GradientStopCollection GradientStopsMainMenu
        {
            get =>
                _gradientStopsMainMenu;
            set
            {
                _gradientStopsMainMenu = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsKill;
        public GradientStopCollection GradientStopsPlayerGetsKill
        {
            get =>
                _gradientStopsPlayerGetsKill;
            set
            {
                _gradientStopsPlayerGetsKill = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsKilled;
        public GradientStopCollection GradientStopsPlayerGetsKilled
        {
            get =>
                _gradientStopsPlayerGetsKilled;
            set
            {
                _gradientStopsPlayerGetsKilled = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsPlayerGetsFlashed;
        public GradientStopCollection GradientStopsPlayerGetsFlashed
        {
            get =>
                _gradientStopsPlayerGetsFlashed;
            set
            {
                _gradientStopsPlayerGetsFlashed = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsTerroristsWin;
        public GradientStopCollection GradientStopsTerroristsWin
        {
            get =>
                _gradientStopsTerroristsWin;
            set
            {
                _gradientStopsTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsCounterTerroristsWin;
        public GradientStopCollection GradientStopsCounterTerroristsWin
        {
            get =>
                _gradientStopsCounterTerroristsWin;
            set
            {
                _gradientStopsCounterTerroristsWin = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsRoundStarts;
        public GradientStopCollection GradientStopsRoundStarts
        {
            get =>
                _gradientStopsRoundStarts;
            set
            {
                _gradientStopsRoundStarts = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsFreezeTime;
        public GradientStopCollection GradientStopsFreezeTime
        {
            get =>
                _gradientStopsFreezeTime;
            set
            {
                _gradientStopsFreezeTime = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsWarmup;
        public GradientStopCollection GradientStopsWarmup
        {
            get =>
                _gradientStopsWarmup;
            set
            {
                _gradientStopsWarmup = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombExplodes;
        public GradientStopCollection GradientStopsBombExplodes
        {
            get =>
                _gradientStopsBombExplodes;
            set
            {
                _gradientStopsBombExplodes = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombPlanted;
        public GradientStopCollection GradientStopsBombPlanted
        {
            get =>
                _gradientStopsBombPlanted;
            set
            {
                _gradientStopsBombPlanted = value;
                OnPropertyChanged();
            }
        }

        private GradientStopCollection _gradientStopsBombBlink;
        public GradientStopCollection GradientStopsBombBlink
        {
            get =>
                _gradientStopsBombBlink;
            set
            {
                _gradientStopsBombBlink = value;
                OnPropertyChanged();
            }
        }
    }
}

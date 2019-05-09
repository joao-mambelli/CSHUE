using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace CSHUE.Cultures
{
    /// <summary>
    /// Wraps up XAML access to instance of WPFLocalize.Properties.Resources, list of available cultures, and method to change culture
    /// </summary>
    public class CultureResources
    {
        #region Fields

        private static readonly bool BFoundInstalledCultures;

        #endregion

        #region Properties

        /// <summary>
        /// List of available cultures, enumerated at startup
        /// </summary>
        public static List<CultureInfo> SupportedCultures { get; } = new List<CultureInfo>();

        /// <summary>
        /// List of available cultures, enumerated at startup
        /// </summary>
        public static List<string> SupportedCulturesFullNames { get; } = new List<string>();

        private static ObjectDataProvider _mProvider;
        public static ObjectDataProvider ResourceProvider => _mProvider ?? (_mProvider = (ObjectDataProvider)System.Windows.Application.Current.FindResource("Resources"));

        #endregion

        #region Methods

        /// <summary>
        /// The Resources ObjectDataProvider uses this method to get an instance of the WPFLocalize.Properties.Resources class
        /// </summary>
        /// <returns></returns>
        public Resources GetResourceInstance() => new Resources();

        static CultureResources()
        {
            if (BFoundInstalledCultures) return;
            foreach (var dir in Directory.GetDirectories(System.Windows.Forms.Application.StartupPath))
            {
                try
                {
                    var dirinfo = new DirectoryInfo(dir);
                    var tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);

                    if (dirinfo.GetFiles(
                            Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath) +
                            ".resources.dll").Length <= 0) continue;
                    SupportedCulturesFullNames.Add(tCulture.NativeName);
                    SupportedCultures.Add(tCulture);
                }
                catch (ArgumentException)
                {

                }
            }
            BFoundInstalledCultures = true;
        }

        /// <summary>
        /// Change the current culture used in the application.
        /// If the desired culture is available all localized elements are updated.
        /// </summary>
        /// <param name="culture">Culture to change to</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (!SupportedCultures.Contains(culture)) return;
            Resources.Culture = culture;
            ResourceProvider.Refresh();
        }

        #endregion
    }
}

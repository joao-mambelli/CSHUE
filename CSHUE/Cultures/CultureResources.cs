﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Forms;

namespace CSHUE.Cultures
{
    /// <summary>
    /// Wraps up XAML access to instance of WPFLocalize.Properties.Resources, list of available cultures, and method to change culture
    /// </summary>
    public class CultureResources
    {
        private static readonly bool BFoundInstalledCultures = false;

        private static List<CultureInfo> pSupportedCultures = new List<CultureInfo>();
        /// <summary>
        /// List of available cultures, enumerated at startup
        /// </summary>
        public static List<CultureInfo> SupportedCultures => pSupportedCultures;

        private static List<string> pSupportedCulturesFullNames = new List<string>();
        /// <summary>
        /// List of available cultures, enumerated at startup
        /// </summary>
        public static List<string> SupportedCulturesFullNames => pSupportedCulturesFullNames;

        static CultureResources()
        {
            if (BFoundInstalledCultures) return;
            foreach (var dir in Directory.GetDirectories(Application.StartupPath))
            {
                try
                {
                    var dirinfo = new DirectoryInfo(dir);
                    var tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);

                    if (dirinfo.GetFiles(
                            Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".resources.dll").Length > 0)
                    {
                        pSupportedCulturesFullNames.Add(tCulture.NativeName);
                        pSupportedCultures.Add(tCulture);
                    }
                }
                catch (ArgumentException)
                {

                }
            }
            BFoundInstalledCultures = true;
        }

        /// <summary>
        /// The Resources ObjectDataProvider uses this method to get an instance of the WPFLocalize.Properties.Resources class
        /// </summary>
        /// <returns></returns>
        public Resources GetResourceInstance()
        {
            return new Resources();
        }

        private static ObjectDataProvider _mProvider;
        public static ObjectDataProvider ResourceProvider => _mProvider ?? (_mProvider = (ObjectDataProvider) System.Windows.Application.Current.FindResource("Resources"));

        /// <summary>
        /// Change the current culture used in the application.
        /// If the desired culture is available all localized elements are updated.
        /// </summary>
        /// <param name="culture">Culture to change to</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (!pSupportedCultures.Contains(culture)) return;
            Resources.Culture = culture;
            ResourceProvider.Refresh();
        }
    }
}
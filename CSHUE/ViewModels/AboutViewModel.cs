using System;
using System.IO;
using System.Reflection;

namespace CSHUE.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        #region Fields

        public MainWindowViewModel MainWindowViewModel = null;

        public string Version
        {
            get
            {
                var date = GetLinkerTimestamp().ToUniversalTime();

                return date.Year + "." + date.DayOfYear + "." + (date.Hour * 60 + date.Minute) + "." + date.Second;
            }
        }

        #endregion

        #region Methods

        private static DateTime GetLinkerTimestamp()
        {
            var filePath = Assembly.GetCallingAssembly().Location;

            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            var bPeHeader = new byte[2048];

            Stream fs = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fs.Read(bPeHeader, 0, 2048);
            }
            finally
            {
                fs?.Close();
            }

            var i = BitConverter.ToInt32(bPeHeader, cPeHeaderOffset);
            var secondsFrom1970 = BitConverter.ToInt32(bPeHeader, i + cLinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsFrom1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

            var fileDt = File.GetLastWriteTime(filePath);

            if (DateTime.Now.Year >= 2038 & dt.Year != fileDt.Year)
                return fileDt;

            return dt;
        }

        #endregion
    }
}

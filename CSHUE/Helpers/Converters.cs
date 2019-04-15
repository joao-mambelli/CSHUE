using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHUE.Cultures;

namespace CSHUE.Helpers
{
    public static class Converters
    {
        public static CultureInfo GetCultureInfoFromIndex(int index)
        {
            return CultureResources.SupportedCultures.ElementAt(index);
        }

        public static int GetIndexFromCultureInfo(CultureInfo cultureInfo)
        {
            return CultureResources.SupportedCultures.IndexOf(cultureInfo);
        }
    }
}

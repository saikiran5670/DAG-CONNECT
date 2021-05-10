using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.vehicledataservice.Common
{
    public class Common
    {
        public static bool IsValidDate(string dateTime)
        {
            string dateformat = "yyyy-mm-dd";
            return DateTime.TryParseExact(dateTime, dateformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}

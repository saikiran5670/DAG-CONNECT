using System;
using System.Globalization;

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

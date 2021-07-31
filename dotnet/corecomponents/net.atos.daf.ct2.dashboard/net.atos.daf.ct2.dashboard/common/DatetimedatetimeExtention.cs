using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.dashboard.common
{
    public static class DatetimedatetimeExtention
    {
        /// <summary>
        /// Convert a DateTime to a unix timestamp
        /// </summary>
        /// <param name="myDateTime">The DateTime object to convert into a Unix Time</param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime myDateTime)
        {
            TimeSpan timeSpan = myDateTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)timeSpan.TotalSeconds;
        }
        /// <summary>
        /// Convert a DateTime to a unix timestamp
        /// </summary>
        /// <param name="myDateTime">The DateTime object to convert into a Unix Time</param>
        /// <returns></returns>
        public static long ToUnixMiliSecTime(this DateTime myDateTime)
        {
            TimeSpan timeSpan = myDateTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)timeSpan.TotalSeconds * 1000;
        }
    }
}

using System;
using TimeZoneConverter;
namespace net.atos.daf.ct2.utilities
{
    public static class UTCHandling
    {
        //  public DateTime GetConvertedDateTime(DateTime utcTime, string stimeZone)
        //    {
        //        DateTime convertedDateTime=new DateTime();
        //        TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(stimeZone);
        //        DateTime  dDateTime = DateTime.Parse(utcTime.ToString());      
        //        DateTimeOffset utcTimeOffSet = new DateTimeOffset(dDateTime, TimeZoneInfo.Local.GetUtcOffset(dDateTime));
        //        long dtunixTime = utcTimeOffSet.ToUnixTimeSeconds();
        //        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(dtunixTime));
        //        DateTime dtzone = TimeZoneInfo.ConvertTimeFromUtc(dt, tzinfo); 

        //        if (tzinfo.IsDaylightSavingTime(dtzone))
        //         {
        //            convertedDateTime = dtzone.AddHours(1);
        //         }
        //         else
        //         {
        //             convertedDateTime = dtzone;
        //         }
        //         return convertedDateTime;   
        //     }


        /// <summary>
        /// This method is used to convert the UTC value in datetime based on timezone. 
        /// The distination datetime format will be pass as parameter.
        /// </summary>
        /// <param name="utctimemilleseconds"></param>
        /// <param name="timezoneName"></param>
        /// <param name="dateformat"></param>
        /// <returns></returns>
        public static string GetConvertedDateTimeFromUTC(long utctimemilleseconds, string timezoneName, string dateformat)
        {

            string sConverteddateTime = string.Empty;
            DateTime date = (new DateTime(1970, 1, 1)).AddMilliseconds(utctimemilleseconds);
            //  TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
            TimeZoneInfo tzinfo = TZConvert.GetTimeZoneInfo(timezoneName);
            // TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
            DateTimeOffset utcTime = new DateTimeOffset(date, TimeZoneInfo.Local.GetUtcOffset(date));
            long dtunixTime = utcTime.ToUnixTimeMilliseconds();
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(dtunixTime));
            DateTime dtzone = TimeZoneInfo.ConvertTimeFromUtc(dt, tzinfo);

            // Add the local UTC offset value to UTC time, e.g for IST, Add +5:30 hours to UTC time
            dtzone = dtzone + TimeZoneInfo.Local.GetUtcOffset(date);

            if (tzinfo.IsDaylightSavingTime(dtzone).ToString() == "True")
            {
                dtzone = dtzone.AddHours(1);
            }
            if (dateformat == "MM/DD/YYYY")  //05/29/2015
            {
                sConverteddateTime = dtzone.ToString("MM/dd/yyyy");
            }
            else if (dateformat == "dddd, dd MMMM yyyy")  //Friday, 29 May 2015
            {
                sConverteddateTime = dtzone.ToString("dddd, dd MMMM yyyy");
            }
            else if (dateformat == "dddd, dd MMMM yyyy HH:mm:ss") //Wednesday, 23 December 2020 07:58:06
            {
                sConverteddateTime = dtzone.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            }
            else if (dateformat == "MM/dd/yyyy HH:mm")   //12/23/2020 07:58
            {
                sConverteddateTime = dtzone.ToString("MM/dd/yyyy HH:mm");
            }
            else if (dateformat == "MM/dd/yyyy hh:mm tt")   //12/23/2020 07:58 AM
            {
                sConverteddateTime = dtzone.ToString("MM/dd/yyyy hh:mm tt");
            }
            else if (dateformat == "MM/dd/yyyy HH:mm:ss")  //12/23/2020 07:58:06
            {
                sConverteddateTime = dtzone.ToString("MM/dd/yyyy HH:mm:ss");
            }
            else if (dateformat == "yyyy-MM-ddTHH:mm:ss")  //2020-12-23T07:58:06
            {
                sConverteddateTime = dtzone.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else if (dateformat == "HH:mm") //07:58
            {
                sConverteddateTime = dtzone.ToString("HH:mm");
            }
            else if (dateformat == "hh:mm tt")  //07:58 AM
            {
                sConverteddateTime = dtzone.ToString("hh:mm tt");
            }
            else if (dateformat == "HH:mm:ss")  //07:58:06
            {
                sConverteddateTime = dtzone.ToString("HH:mm:ss");
            }
            else if (dateformat == "yyyy-MM-ddTHH:mm:ss.fffz")  //2021-04-26T05:44:42.341+0
            {
                sConverteddateTime = dtzone.ToString("yyyy-MM-ddTHH:mm:ss.fffz");
            }
            else
            {
                sConverteddateTime = dtzone.ToString();
            }
            return sConverteddateTime;
        }

        /// <summary>
        /// This methods is used to get the UTC value form datetime object
        /// </summary>
        /// <param name="sourceDateTime"></param>
        /// <returns></returns>
        public static long GetUTCFromDateTime(DateTime sourceDateTime)
        {
            DateTimeOffset utcTime = new DateTimeOffset(sourceDateTime, TimeZoneInfo.Local.GetUtcOffset(sourceDateTime));
            long dtunixTime = utcTime.ToUnixTimeMilliseconds();
            return dtunixTime;
        }

        /// <summary>
        /// This methods is used to get the UTC value form datetime string
        /// </summary>
        /// <param name="sourceDateTime"></param>
        /// <returns></returns>
        public static long GetUTCFromDateTime(string sourceDateTime)
        {
            DateTimeOffset utcTime = new DateTimeOffset(Convert.ToDateTime(sourceDateTime), TimeZoneInfo.Local.GetUtcOffset(Convert.ToDateTime(sourceDateTime)));
            long dtunixTime = utcTime.ToUnixTimeMilliseconds();
            return dtunixTime;
        }

        /// <summary>
        /// This methid is used to get UTC values from datetime string and timezone mame as input parameters
        /// </summary>
        /// <param name="sourceDateTime"></param>
        /// <param name="timezonename"></param>
        /// <returns></returns>
        public static long GetUTCFromDateTime(string sourceDateTime, string timezonename)
        {
            // TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(timezonename);    
            TimeZoneInfo tzinfo = TZConvert.GetTimeZoneInfo(timezonename);
            DateTimeOffset utcTime = new DateTimeOffset(Convert.ToDateTime(sourceDateTime), tzinfo.GetUtcOffset(Convert.ToDateTime(sourceDateTime)));
            long dtunixTime = utcTime.ToUnixTimeMilliseconds();
            return dtunixTime;
        }

        //  public static long GetUTCFromDateTime(long utctimemilleseconds, string timezoneName, string dateformat)
        // {        
        //     string sConverteddateTime = string.Empty;          
        //     DateTime date = (new DateTime(1970, 1, 1)).AddMilliseconds(utctimemilleseconds);    
        //    // TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
        //     TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo(timezoneName);
        //     DateTimeOffset utcTime = new DateTimeOffset(date, TimeZoneInfo.Local.GetUtcOffset(date));
        //     long dtunixTime = utcTime.ToUnixTimeMilliseconds();   
        //     DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(dtunixTime));
        //     DateTime dtzone = TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);

        //     return dtunixTime;
        // }  

    }
}

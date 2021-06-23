using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public static class TimeZoneHelper
    {
        public static DateTime GetDateTimeFromUTC(long utcDate, string timeZone)
        {
            try
            {
                return Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(utcDate, timeZone, "yyyy-MM-ddTHH:mm:ss"));
            }
            catch (Exception)
            {
                return Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(utcDate, "UTC", "yyyy-MM-ddTHH:mm:ss"));
            }            
        }
    }

    public class TimeZoneSingleton
    {
        private static TimeZoneSingleton _instance;
        private static IEnumerable<UserTimeZone> _userTimeZone;
        private static Object root;
        private TimeZoneSingleton()
        {

        }

        public static TimeZoneSingleton GetInstance(IReportSchedulerRepository reportSchedularRepository)
        {
            lock (root)
            {
                if (_instance == null)
                {
                    _userTimeZone =  reportSchedularRepository.GetUserTimeZone().Result;
                    _instance = new TimeZoneSingleton();
                }
            }
            return _instance;
        }

        public string GetTimeZoneName(int timezoneId)
        {
            return _userTimeZone.Where(w => w.Id == timezoneId).FirstOrDefault().Name ?? "UTC";
        }

    }
}

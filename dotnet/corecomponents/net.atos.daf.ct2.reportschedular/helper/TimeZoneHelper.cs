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
        public static string GetDateTimeFromUTC(long utcDate, string timeZone, string datetimeFormat)
        {
            try
            {
                return UTCHandling.GetConvertedDateTimeFromUTC(utcDate, timeZone, datetimeFormat);
            }
            catch (Exception)
            {
                return UTCHandling.GetConvertedDateTimeFromUTC(utcDate, "UTC", datetimeFormat);
            }
        }
    }

    public class TimeZoneSingleton
    {
        private static TimeZoneSingleton _instance;
        private IEnumerable<UserTimeZone> _userTimeZone;
        private static readonly Object _root = new object();
        private TimeZoneSingleton()
        {
        }

        public static TimeZoneSingleton GetInstance(IReportSchedulerRepository reportSchedularRepository)
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _instance = new TimeZoneSingleton();
                    _instance._userTimeZone = reportSchedularRepository.GetUserTimeZone().Result;
                }
            }
            return _instance;
        }

        public string GetTimeZoneName(int timezoneId)
        {
            return _userTimeZone.Where(w => w.Id == timezoneId).FirstOrDefault()?.Name ?? TimeConstants.UTC;
        }

    }
}

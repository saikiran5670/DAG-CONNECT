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

    public class TimeFormatSingleton
    {
        private static TimeFormatSingleton _instance;
        private static IEnumerable<UserTimeFormat> _userTimeFormat;
        private static readonly Object _root = new object();
        private TimeFormatSingleton()
        {
        }

        public static TimeFormatSingleton GetInstance(IReportSchedulerRepository reportSchedularRepository)
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _userTimeFormat = reportSchedularRepository.GetUserTimeFormat().Result;
                    _instance = new TimeFormatSingleton();
                }
            }
            return _instance;
        }

        public string GetTimeFormatName(int timeFormatId)
        {
            var formatName = _userTimeFormat.Where(w => w.Id == timeFormatId).FirstOrDefault()?.Key ?? FormatConstants.DATE_FORMAT_LABLE;
            return formatName == FormatConstants.DATE_FORMAT_LABLE ? FormatConstants.TIME_FORMAT_24 : FormatConstants.TIME_FORMAT_12;
        }

    }
}

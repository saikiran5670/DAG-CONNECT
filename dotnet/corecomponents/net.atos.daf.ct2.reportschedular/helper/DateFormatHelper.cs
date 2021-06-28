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

    public class DateFormatSingleton
    {
        private static DateFormatSingleton _instance;
        private static IEnumerable<UserTimeFormat> _userDateFormat;
        private static readonly Object _root = new object();
        private DateFormatSingleton()
        {
        }

        public static DateFormatSingleton GetInstance(IReportSchedulerRepository reportSchedularRepository)
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _userDateFormat = reportSchedularRepository.GetUserTimeFormat().Result;
                    _instance = new DateFormatSingleton();
                }
            }
            return _instance;
        }

        public string GetDateFormatName(int dateFormatId)
        {
            return _userDateFormat.Where(w => w.Id == dateFormatId).FirstOrDefault()?.Name ?? FormatConstants.DATE_FORMAT;
        }

    }
}

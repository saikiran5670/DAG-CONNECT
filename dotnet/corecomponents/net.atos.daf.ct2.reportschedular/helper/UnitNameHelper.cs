using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public class UnitNameSingleton
    {
        private static UnitNameSingleton _instance;
        private static IEnumerable<UnitName> _unitName;
        private static readonly Object _root = new object();
        private UnitNameSingleton()
        {
        }

        public static UnitNameSingleton GetInstance(IReportSchedulerRepository reportSchedularRepository)
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _unitName = reportSchedularRepository.GetUnitName().Result;
                    _instance = new UnitNameSingleton();
                }
            }
            return _instance;
        }

        public UnitToConvert GetUnitName(int unitId)
        {
            switch (_unitName.Where(w => w.Id == unitId).FirstOrDefault()?.Key)
            {
                case FormatConstants.UNIT_IMPERIAL_KEY:
                    return UnitToConvert.Imperial;
                case FormatConstants.UNIT_METRIC_KEY:
                    return UnitToConvert.Metric;
                default:
                    return UnitToConvert.Metric;
            }
        }

    }
}

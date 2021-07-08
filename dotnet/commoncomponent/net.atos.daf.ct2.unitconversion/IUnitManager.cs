using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{
    public interface IUnitManager
    {
        Task<string> GetDistanceUnit(UnitToConvert convertTo);
        Task<string> GetSpeedUnit(UnitToConvert convertTo);
        Task<string> GetVolumeUnit(UnitToConvert convertTo);
        Task<string> GetVolumePerDistanceUnit(UnitToConvert convertTo);
        Task<string> GetWeightUnit(UnitToConvert convertTo);
        Task<string> GetTimeSpanUnit(UnitToConvert convertTo);
    }
}

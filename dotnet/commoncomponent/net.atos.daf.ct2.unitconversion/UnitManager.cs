using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.entity;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{
    public class UnitManager : IUnitManager
    {
        public Task<string> GetDistanceUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;

            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = DistanceConstants.MILES;
                    break;
                case UnitToConvert.Metric:
                    result = DistanceConstants.KM;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetSpeedUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = SpeedConstants.MILES_PER_HOUR;
                    break;
                case UnitToConvert.Metric:
                    result = SpeedConstants.KM_PER_HOUR;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetTimeSpanUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                case UnitToConvert.Metric:
                    result = TimeSpanConstants.HH_MM;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetVolumePerDistanceUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = VolumePerDistanceConstants.GALLON_PER_MILES;
                    break;
                case UnitToConvert.Metric:
                    result = VolumePerDistanceConstants.LITER_PER_KM;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetVolumePer100KmUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = VolumePer100KmConstants.MPG;
                    break;
                case UnitToConvert.Metric:
                    result = VolumePer100KmConstants.LITER_PER_100KM;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetVolumeUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = VolumeConstants.GALLON;
                    break;
                case UnitToConvert.Metric:
                    result = VolumeConstants.LITER;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<string> GetWeightUnit(UnitToConvert convertTo)
        {
            string result = string.Empty;
            switch (convertTo)
            {
                case UnitToConvert.Imperial:
                    result = WeightConstants.TON_IMPERIAL;
                    break;
                case UnitToConvert.Metric:
                    result = WeightConstants.TON_METRIC;
                    break;
            }
            return Task.FromResult(result);
        }
    }
}

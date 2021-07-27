using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{
    public interface IUnitConversionManager
    {
        Task<double> GetDistance(double value, DistanceUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetSpeed(double value, SpeedUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetTime(double value, TimeUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetVolume(double value, VolumeUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetVolumePerDistance(double value, VolumePerDistanceUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetVolumePer100Km(double value, VolumeUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<double> GetWeight(double value, WeightUnit inputUnit, UnitToConvert convertTo, int decimals = 2);
        Task<string> GetTimeSpan(double value, TimeUnit inputUnit, UnitToConvert convertTo);
    }
}
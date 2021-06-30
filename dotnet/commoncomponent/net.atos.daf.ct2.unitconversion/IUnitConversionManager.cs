using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{
    public interface IUnitConversionManager
    {
        Task<double> GetDistance(double value, DistanceUnit inputUnit, UnitToConvert convertTo);
        Task<double> GetSpeed(double value, SpeedUnit inputUnit, UnitToConvert convertTo);
        Task<double> GetTime(double value, TimeUnit inputUnit, UnitToConvert convertTo);
        Task<double> GetVolume(double value, VolumeUnit inputUnit, UnitToConvert convertTo);
        Task<double> GetVolumePerDistance(double value, VolumePerDistanceUnit inputUnit, UnitToConvert convertTo);
        Task<double> GetWeight(double value, WeightUnit inputUnit, UnitToConvert convertTo);
    }
}
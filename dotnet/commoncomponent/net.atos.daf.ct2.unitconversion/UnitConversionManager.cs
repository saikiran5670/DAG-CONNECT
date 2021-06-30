using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.entity;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{
    public class UnitConversionManager : IUnitConversionManager
    {
        public Task<double> GetDistance(double value, DistanceUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case DistanceUnit.Meter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 0.001;
                            break;
                        case UnitToConvert.Metric:
                            result = value;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<double> GetSpeed(double value, SpeedUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case SpeedUnit.MeterPerMilliSec:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value * 2236.94;
                            break;
                        case UnitToConvert.Metric:
                            result = value * 3600;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<double> GetTime(double value, TimeUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case TimeUnit.Seconds:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                        case UnitToConvert.Metric:
                            result = value * 0.0166667;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<double> GetVolume(double value, VolumeUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case VolumeUnit.MilliLiter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 4546;
                            break;
                        case UnitToConvert.Metric:
                            result = value / 1e+6; //need to check
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<double> GetVolumePerDistance(double value, VolumePerDistanceUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case VolumePerDistanceUnit.MilliLiterPerMeter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 2.352;
                            break;
                        case UnitToConvert.Metric:
                            result = value;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }

        public Task<double> GetWeight(double value, WeightUnit inputUnit, UnitToConvert convertTo)
        {
            double result = value;
            switch (inputUnit)
            {
                case WeightUnit.KiloGram:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value * 2.205;
                            break;
                        case UnitToConvert.Metric:
                            result = value;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(result);
        }
    }
}

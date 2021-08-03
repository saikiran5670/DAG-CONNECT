using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.unitconversion.entity;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion
{

    public class UnitConversionManager : IUnitConversionManager
    {
        public Task<double> GetDistance(double value, DistanceUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
        {
            double result = value;
            switch (inputUnit)
            {
                case DistanceUnit.Meter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 1609.344;
                            break;
                        case UnitToConvert.Metric:
                            result = value / 1000;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetSpeed(double value, SpeedUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
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
                case SpeedUnit.KmPerHour:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 1.609;
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
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetTime(double value, TimeUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
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
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetVolume(double value, VolumeUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
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
                            result = value / 1000;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetVolumePerDistance(double value, VolumePerDistanceUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
        {
            double result = value;
            switch (inputUnit)
            {
                case VolumePerDistanceUnit.MilliLiterPerMeter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 2.825;
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
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetVolumePer100Km(double distance, double fuelconsumed, VolumeUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
        {
            double result = 0;
            switch (inputUnit)
            {
                case VolumeUnit.MilliLiter:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = fuelconsumed > 0 ? ((distance / 1609.344) / (fuelconsumed / 4546)) : 0;
                            break;
                        case UnitToConvert.Metric:
                            result = distance > 0 ? ((fuelconsumed / 1000) / (distance / 1000)) * 100 : 0;
                            break;
                    }
                    break;
                default:
                    result = 0;
                    break;
            }
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<double> GetWeight(double value, WeightUnit inputUnit, UnitToConvert convertTo, int decimals = 2)
        {
            double result = value;
            switch (inputUnit)
            {
                case WeightUnit.KiloGram:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value / 907;
                            break;
                        case UnitToConvert.Metric:
                            result = value / 1000;
                            break;
                    }
                    break;
                case WeightUnit.Tons:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                            result = value;
                            break;
                        case UnitToConvert.Metric:
                            result = value / 1.102;
                            break;
                    }
                    break;
                default:
                    result = value;
                    break;
            }
            return Task.FromResult(Math.Round(result, decimals));
        }

        public Task<string> GetTimeSpan(double value, TimeUnit inputUnit, UnitToConvert convertTo)
        {
            string result = "00:00";
            switch (inputUnit)
            {
                case TimeUnit.Seconds:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                        case UnitToConvert.Metric:
                            TimeSpan t = TimeSpan.FromSeconds(value);

                            result = string.Format("{0:D2}:{1:D2}",
                                            t.Hours,
                                            t.Minutes);
                            break;
                    }
                    break;
                case TimeUnit.MiliSeconds:
                    switch (convertTo)
                    {
                        case UnitToConvert.Imperial:
                        case UnitToConvert.Metric:
                            TimeSpan t = TimeSpan.FromMilliseconds(value);

                            result = string.Format("{0:D2}:{1:D2}",
                                            t.Hours,
                                            t.Minutes);
                            break;
                    }
                    break;
                default:
                    result = "00:00";
                    break;
            }
            return Task.FromResult(result);
        }
    }
}

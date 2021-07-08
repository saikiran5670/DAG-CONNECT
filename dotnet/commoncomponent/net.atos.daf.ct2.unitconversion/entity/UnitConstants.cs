using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.unitconversion.entity
{
    public static class ConverterConstants
    {
        public const string IMPERIAL = "Imperial";
        public const string METRIC = "Metric";
    }

    public static class DistanceConstants
    {
        public const string MILES = "mi";
        public const string KM = "km";
    }

    public static class TimeSpanConstants
    {
        public const string HH_MM = "hh:mm";
    }

    public static class SpeedConstants
    {
        public const string MILES_PER_HOUR = "mph";
        public const string KM_PER_HOUR = "km/h";
    }

    public static class WeightConstants
    {
        public const string TON_IMPERIAL = "ton";
        public const string TON_METRIC = "t";
    }

    public static class VolumeConstants
    {
        public const string GALLON = "gal";
        public const string LITER = "l";
    }

    public static class VolumePerDistanceConstants
    {
        public const string GALLON_PER_MILES = "gal/mi";
        public const string LITER_PER_KM = "l/km";
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.unitconversion.ENUM
{
    public enum UnitToConvert
    {
        Imperial = 0,
        Metric = 1
    }

    public enum DistanceUnit
    {
        Meter = 0,
        KiloMeter = 1
    }

    public enum TimeUnit
    {
        Seconds = 0,
        MiliSeconds = 1
    }

    public enum SpeedUnit
    {
        MeterPerMilliSec = 0
    }

    public enum WeightUnit
    {
        KiloGram = 0
    }

    public enum VolumeUnit
    {
        MilliLiter = 0
    }

    public enum VolumePerDistanceUnit
    {
        MilliLiterPerMeter = 0
    }
}

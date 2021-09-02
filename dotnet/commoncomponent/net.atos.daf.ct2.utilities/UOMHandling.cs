using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.utilities
{
    public static class UOMHandling
    {
        public static double GetConvertedThresholdValue(double actualValue, string expectedUnit)
        {
            double expectedThresholdValue = actualValue;
            if (expectedThresholdValue > 0)
            {
                switch (expectedUnit)
                {
                    case "K":
                        expectedThresholdValue = actualValue / 1000;
                        break;
                    case "L":
                        expectedThresholdValue = actualValue / 1609;
                        break;
                    case "F":
                        expectedThresholdValue = actualValue * Convert.ToDouble(3.281);
                        break;
                    case "A":
                        expectedThresholdValue = actualValue * Convert.ToDouble(3.6);
                        break;
                    case "B":
                        expectedThresholdValue = actualValue * Convert.ToDouble(2.237);
                        break;
                    default:
                        break;
                }
            }
            return Math.Round(expectedThresholdValue, 2, MidpointRounding.AwayFromZero);
        }
        public static string GetConvertedTimeBasedThreshold(double actualValue, string expectedUnit)
        {
            string expectedThresholdValue = "0";
            TimeSpan timespan = TimeSpan.FromSeconds(actualValue);
            switch (expectedUnit)
            {
                case "H":
                    expectedThresholdValue = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                            timespan.Hours,
                            timespan.Minutes,
                            timespan.Seconds);
                    break;
                case "T":
                    expectedThresholdValue = string.Format("{0:D2}m:{1:D2}s",
                           (timespan.Hours * 60) + timespan.Minutes,
                            timespan.Seconds);
                    break;
                default:
                    break;
            }
            return expectedThresholdValue;
        }
        public static string GetUnitName(string unitEnum)
        {
            string unitName = string.Empty;
            switch (unitEnum)
            {
                case "H":
                    unitName = "Hours";
                    break;
                case "T":
                    unitName = "Minutes";
                    break;
                case "K":
                    unitName = "KiloMeter";
                    break;
                case "L":
                    unitName = "Miles";
                    break;
                case "F":
                    unitName = "Feet";
                    break;
                case "A":
                    unitName = "Km/h";
                    break;
                case "B":
                    unitName = "Miles/h";
                    break;
                case "S":
                    unitName = "Seconds";
                    break;
                case "P":
                    unitName = "%";
                    break;
                default:
                    break;
            }

            return unitName;
        }
    }
}

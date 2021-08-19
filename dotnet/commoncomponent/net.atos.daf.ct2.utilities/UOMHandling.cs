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
            switch (expectedUnit)
            {
                case "H":
                    expectedThresholdValue = actualValue / 3600;
                    break;
                case "T":
                    expectedThresholdValue = actualValue / 60;
                    break;
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

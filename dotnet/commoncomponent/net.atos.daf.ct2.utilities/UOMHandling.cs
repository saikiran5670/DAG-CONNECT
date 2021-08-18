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
    }
}

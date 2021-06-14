using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.map.entity
{
    public class MapHelper
    {
        private List<double> GetRange(double value) // as per user story range should be +/-1 2 of 5th decimal value
        {
            int decimalPoint = GigitsAfterDecimalPoint(value);

            string trimDecimalValue = TrimDecimal(1 / Math.Pow(10, decimalPoint));
            double decimalValue = Convert.ToDouble(trimDecimalValue);

            var reangeData = new List<double>(5) {
                                      Math.Round((value+ decimalValue),decimalPoint),
                                      Math.Round(value+ decimalValue+decimalValue,decimalPoint),
                                      value,
                                      Math.Round( value - decimalValue,decimalPoint),
                                      Math.Round(value - decimalValue - decimalValue,decimalPoint),
                                };
            return reangeData;
        }


        private int GigitsAfterDecimalPoint(double num)
        {
            int count = 0;
            int i = 0;
            string n = num.ToString();
            int len = n.Length;
            while (n[i] != '.')
            {
                i++;
            }
            int after_decimal = len - i - 1;
            int total_Count = count + after_decimal;
            return total_Count;
        }


        private string TrimDecimal(double value)
        {
            string result = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return result.IndexOf('.') == -1 ? result : result.TrimEnd('0', '.');
        }


        public MapLatLngRange GetLatLonRange(double lat, double lan)
        {
            var mapLatLngRange = new MapLatLngRange()
            {
                Latitude = GetRange(lat),
                Longitude = GetRange(lan)
            };
            return mapLatLngRange;
        }



    }
}

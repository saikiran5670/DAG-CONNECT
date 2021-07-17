using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class TripAlert
    {
        public int Id { get; set; }
        public string Tripid { get; set; }
        public string Vin { get; set; }
        public string CategoryType { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Alertid { get; set; }
        public double ThresholdValue { get; set; }
        public string ThresholdValueUnitType { get; set; }
        public double ValueAtAlertTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long AlertGeneratedTime { get; set; }
        public long MessageTimestamp { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
        public string UrgencyLevelType { get; set; }
    }
}

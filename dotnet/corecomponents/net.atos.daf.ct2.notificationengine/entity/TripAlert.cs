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
        public int VehicleId { get; set; }
        public long FromDateThresholdValue { get; set; }
        public long ToDateThresholdValue { get; set; }
        public string TimePeriodType { get; set; }
        public bool[] DayThresholdValue { get; set; } = new bool[7];
        public long FromDayTimeThresholdValue { get; set; }
        public long ToDayTimeThresholdValue { get; set; }
        public long DateBreachedValue { get; set; }
        public bool[] DayBreachedValue { get; set; } = new bool[7];
        public long DayTimeBreachedValue { get; set; }
        public string ParamType { get; set; }
        public int TrigenThresholdValue { get; set; }
        public string TrigenThresholdValueUnitType { get; set; }
        public int BreachedValue { get; set; }
        public string LandmarkType { get; set; }
        public int LandmarkId { get; set; }
        public string LandmarkName { get; set; }
        public string LandmarkPositionType { get; set; }
        public int LandmarkThresholdValue { get; set; }
        public string LandmarkThresholdValueUnitType { get; set; }
        public int LandmarkBreachedValue { get; set; }
        public string AlertCategoryKey { get; set; }
        public string AlertTypeKey { get; set; }
        public string UrgencyTypeKey { get; set; }
    }
}

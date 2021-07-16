using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.CustomValidators.Report;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class LogbookDetailsFilter
    {

        public string VIN { get; set; }
        public string GroupId { get; set; }
        public string TripId { get; set; }
        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public int Occurrence { get; set; }
        public char AlertLevel { get; set; }
        public char AlertCategory { get; set; }
        public long TripStartTime { get; set; }
        public long TripEndTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Threshold { get; set; }
    }

    public class LogbookFilter
    {
        [MinLength(1, ErrorMessage = "At least one group id is required.")]
        [GroupIdCheck(ErrorMessage = "Group id is invalid.")]
        public List<string> GroupId { get; set; }

        [MinLength(1, ErrorMessage = "At least one VIN id is required.")]
        public List<string> VIN { get; set; }

        [MinLength(1, ErrorMessage = "At least one alert level is required.")]
        [AlertLevelCheck(ErrorMessage = "Alert level is invalid.")]
        public List<string> AlertLevel { get; set; }

        [MinLength(1, ErrorMessage = "At least one alert category is required.")]
        [AlertCategoryCheck(ErrorMessage = "Alert category is invalid.")]
        public List<string> AlertCategory { get; set; }

        [MinLength(1, ErrorMessage = "At least one VIN id is required.")]
        public List<string> AlertType { get; set; }

        [Required]
        public long Start_Time { get; set; }
        [Required]
        public long End_time { get; set; }


    }
}


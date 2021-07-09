using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Report;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class FleetOverviewFilter
    {
        [MinLength(1, ErrorMessage = "At least one group id is required.")]
        [GroupIdCheck(ErrorMessage = "Group id is invalid.")]
        public List<string> GroupId { get; set; }
        [MinLength(1, ErrorMessage = "At least one alert level is required.")]
        [AlertLevelCheck(ErrorMessage = "Alert level is invalid.")]
        public List<string> AlertLevel { get; set; }
        [MinLength(1, ErrorMessage = "At least one alert category is required.")]
        [AlertCategoryCheck(ErrorMessage = "Alert category is invalid.")]
        public List<string> AlertCategory { get; set; }
        [MinLength(1, ErrorMessage = "At least one health status is required.")]
        [VehicleHealthStateCheck(ErrorMessage = "Health status is invalid.")]
        public List<string> HealthStatus { get; set; }
        [MinLength(1, ErrorMessage = "At least one other filter is required.")]
        public List<string> OtherFilter { get; set; }
        [MinLength(1, ErrorMessage = "At least one driver id is required.")]
        public List<string> DriverId { get; set; }
        [Required(ErrorMessage = "Days is mandatory.")]
        [DaysRangeCheck(ErrorMessage = "Days is invalid.")]
        public int Days { get; set; }
        [Required(ErrorMessage = "Langauguage code is required.")]
        public string LanguageCode { get; set; }
    }
}

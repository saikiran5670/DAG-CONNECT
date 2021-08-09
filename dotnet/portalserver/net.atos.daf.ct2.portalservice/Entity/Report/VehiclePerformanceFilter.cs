using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class VehiclePerformanceFilter
    {

        [Required]
        public string VIN { get; set; }
        [MinLength(1, ErrorMessage = "PerformanceType should be of single character.")]
        public string PerformanceType { get; set; }
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
    }
}

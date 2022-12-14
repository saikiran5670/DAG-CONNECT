using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class ReportFleetFuelFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
        [Required]
        public string LanguageCode { get; set; }
    }
    public class ReportFleetFuelDriverFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public string VIN { get; set; }

        public string DriverId { get; set; }
    }

    public class FuelDeviationFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
    }
}

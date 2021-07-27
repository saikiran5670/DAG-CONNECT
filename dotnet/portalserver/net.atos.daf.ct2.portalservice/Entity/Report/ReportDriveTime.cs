using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{

    public class DriversTimeFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
        [Required]
        public List<string> DriverIds { get; set; }
    }
    public class SingleDriverTimeFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public string VIN { get; set; }
        [Required]
        public string DriverId { get; set; }
    }

    public class DriverTimeChartFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public string DriverId { get; set; }
    }

}

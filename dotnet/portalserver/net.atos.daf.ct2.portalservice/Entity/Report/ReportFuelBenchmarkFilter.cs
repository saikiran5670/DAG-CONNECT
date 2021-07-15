using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class ReportFuelBenchmarkFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }

        public int VehicleGroupId { get; set; }

    }
}

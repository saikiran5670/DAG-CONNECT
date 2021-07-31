using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Dashboard
{
    public class DashboardFilter
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
    }
    public class Alert24HoursFilter
    {
        [Required]
        public List<string> VINs { get; set; }
    }
}

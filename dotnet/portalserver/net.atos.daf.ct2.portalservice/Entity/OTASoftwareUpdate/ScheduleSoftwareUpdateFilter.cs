using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.OTASoftwareUpdate
{
    public class ScheduleSoftwareUpdateFilter
    {
        public string CampaignId { get; set; }
        public string Vin { get; set; }
        public string BaseLineId { get; set; }
        public string ScheduleDateTime { get; set; }
    }
}

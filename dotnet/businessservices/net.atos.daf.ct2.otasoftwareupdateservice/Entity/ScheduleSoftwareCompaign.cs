using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Entity
{
    public class ScheduleSoftwareCompaign
    {
        public string CompaignId { get; set; }
        public string Vin { get; set; }
        public long ScheduleDateTime { get; set; }
        public long CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public long TimeStampBoasch { get; set; }
        public string Status { get; set; }
        public string BaselineId { get; set; }
    }
}

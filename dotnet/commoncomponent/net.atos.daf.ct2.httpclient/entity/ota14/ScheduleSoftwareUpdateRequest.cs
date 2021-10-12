using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota14
{
    public class ScheduleSoftwareUpdateRequest
    {
        public string ScheduleDateTime { get; set; }
        public string ApprovalMessage { get; set; }
        public string BaseLineId { get; set; }
    }
}

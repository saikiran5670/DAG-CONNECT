using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota14
{
    public class ScheduleSoftwareUpdateRequest
    {
        public string SchedulingTime { get; set; }
        public string ApprovalMessage { get; set; }
        public string BaseLineId { get; set; }
        public string AccountEmailId { get; set; }
    }

    public class ScheduleSoftwareUpdateReq
    {
        [JsonProperty("schedulingTime")]
        public string SchedulingTime { get; set; }
        [JsonProperty("approvalMessage")]
        public string ApprovalMessage { get; set; }
        //public string BaseLineId { get; set; }
    }
}

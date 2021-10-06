using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota14
{
    public class ScheduleSoftwareUpdateRequest
    {
        public string CampaignId { get; set; }
        [JsonProperty("vins")]
        public List<string> Vins { get; set; }
        public string BaseLineId { get; set; }
        public long ScheduleDateTime { get; set; }
    }
}

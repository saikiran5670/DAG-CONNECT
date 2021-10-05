using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota22
{
    public class CampiagnSoftwareReleaseNoteRequest
    {
        public string CampaignId { get; set; }
        [JsonProperty("vins")]
        public List<string> Vins { get; set; }
        public string Language { get; set; }
        public string Retention { get; set; }
    }
}

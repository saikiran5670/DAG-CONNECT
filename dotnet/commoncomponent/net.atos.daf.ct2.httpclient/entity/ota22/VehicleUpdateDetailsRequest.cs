using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota22
{
    public class VehicleUpdateDetailsRequest
    {
        [JsonProperty("vin")]
        public string Vin { get; set; }
        [JsonProperty("retention")]
        public string Retention { get; set; }
    }
}

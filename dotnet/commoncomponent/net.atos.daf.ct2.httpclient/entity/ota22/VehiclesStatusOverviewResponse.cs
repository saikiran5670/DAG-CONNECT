using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota22
{
    public class VehiclesStatusOverview
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("vin")]
        public string Vin { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class VehiclesStatusOverviewResponse
    {
        public int HttpStatusCode { get; set; }
        public VehiclesStatusOverview VehiclesStatusOverview { get; set; }
        //public string Message { get; set; }
    }
}

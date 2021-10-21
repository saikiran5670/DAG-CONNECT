using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota22
{
    public class Campaign
    {
        [JsonProperty("campaignID")]
        public string CampaignID { get; set; }
        [JsonProperty("baselineAssignment")]
        public string BaselineAssignment { get; set; }
        [JsonProperty("campaignSubject")]
        public string CampaignSubject { get; set; }
        [JsonProperty("systems")]
        public List<string> Systems { get; set; }
        [JsonProperty("campaignType")]
        public string CampaignType { get; set; }
        [JsonProperty("campaignCategory")]
        public string CampaignCategory { get; set; }
        [JsonProperty("updateStatus")]
        public string UpdateStatus { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
    }

    public class VehicleUpdateDetails
    {
        [JsonProperty("vin")]
        public string Vin { get; set; }
        [JsonProperty("vehicleSoftwareStatus")]
        public string VehicleSoftwareStatus { get; set; }
        [JsonProperty("campaigns")]
        public List<Campaign> Campaigns { get; set; }
    }

    public class VehicleUpdateDetailsResponse
    {
        public int HttpStatusCode { get; set; }
        public VehicleUpdateDetails VehicleUpdateDetails { get; set; }
    }
}

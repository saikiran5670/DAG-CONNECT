using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota22
{
    public class Assignment
    {
        [JsonProperty("updateStatus")]
        public string UpdateStatus { get; set; }
        public string BaselineAssignmentId { get; set; }
    }

    public class Vins
    {
        [JsonProperty("vin")]
        public string Vin { get; set; }
        public List<Assignment> Assignments { get; set; }
    }

    public class CampiagnSoftwareReleaseNote
    {
        [JsonProperty("campaignID")]
        public string CampaignID { get; set; }
        //[JsonProperty("subject")]
        //public string Subject { get; set; }
        //[JsonProperty("affectedSystems")]
        //public List<string> AffectedSystems { get; set; }
        //[JsonProperty("campaignType")]
        //public string CampaignType { get; set; }
        //[JsonProperty("campaignCategory")]
        //public string CampaignCategory { get; set; }
        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
        //[JsonProperty("vins")]
        //public List<Vins> Vins { get; set; }
    }
    public class CampiagnSoftwareReleaseNoteResponse
    {
        public int HttpStatusCode { get; set; }
        public CampiagnSoftwareReleaseNote CampiagnSoftwareReleaseNote { get; set; }
    }
}

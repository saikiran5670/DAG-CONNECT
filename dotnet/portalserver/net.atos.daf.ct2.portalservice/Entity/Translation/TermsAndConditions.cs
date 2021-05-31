using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class TermsAndConditions
    {

    }
    public class AccountTermsCondition
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public int Account_Id { get; set; }
        public int Terms_And_Condition_Id { get; set; }
        public string version_no { get; set; }
    }

    /// <summary>
    /// This Model is passed as payload from front end
    /// </summary>
    public class TermsandConFileData
    {
        public string fileName { get; set; }
        [JsonIgnore]
        public string version_no { get; set; }
        [JsonIgnore]
        public string code { get; set; }
        public byte[] description { get; set; }


    }
    /// <summary>
    /// List of data from front end
    /// </summary>
    public class TermsandConFileDataList
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int created_by { get; set; }
        public List<TermsandConFileData> _data { get; set; }
    }
    /// <summary>
    /// Data returned from DB is saved here
    /// </summary>
    public class VersionAndCodeExits
    {
        public int id { get; set; }
        public string version_no { get; set; }
        public string code { get; set; }
    }

    /// <summary>
    /// Response to send back with TermsAndCondition record status.
    /// </summary>
    public class TermsAndConditionResponseList
    {
        public List<TermsAndConditionResponse> termsAndConditionDetails { get; set; }
    }
    public class TermsAndConditionResponse
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string action { get; set; }
    }
    public class VersionByID
    {
        public int orgId { get; set; }
        public int levelCode { get; set; }
        public int accountId { get; set; }
    }
}

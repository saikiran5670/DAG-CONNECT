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
        public string Version_no { get; set; }
    }

    /// <summary>
    /// This Model is passed as payload from front end
    /// </summary>
    public class TermsandConFileData
    {
        public string FileName { get; set; }
        [JsonIgnore]
        public string Version_no { get; set; }
        [JsonIgnore]
        public string Code { get; set; }
        public byte[] Description { get; set; }


    }
    /// <summary>
    /// List of data from front end
    /// </summary>
    public class TermsandConFileDataList
    {
        public string Start_date { get; set; }
        public string End_date { get; set; }
        public int Created_by { get; set; }
        public List<TermsandConFileData> Data { get; set; }
    }

    public class VersionByID
    {
        public int OrgId { get; set; }
        public int LevelCode { get; set; }
        public int AccountId { get; set; }
    }
}

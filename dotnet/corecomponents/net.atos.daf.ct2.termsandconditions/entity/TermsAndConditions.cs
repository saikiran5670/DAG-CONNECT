using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.termsandconditions.entity
{
    public class TermsAndConditions
    {
        public int Id { get; set; }
        public string version_no { get; set; }
        public string Code { get; set; }
        public byte[] Description { get; set; }
        public char State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Created_At { get; set; }
        public int created_by { get; set; }
        public DateTime Modified_at { get; set; }
        public int Modified_by { get; set; }
        public DateTime Accepted_Date { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
    }

    public class AccountTermsCondition
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public int Account_Id { get; set; }
        public int Terms_And_Condition_Id { get; set; }
        public DateTime Accepted_Date { get; set; }
        public string version_no { get; set; }
    }

    /// <summary>
    /// This Model is passed as payload from front end
    /// </summary>
    public class TermsandConFileData
    {
        public string fileName { get; set; }
        public string version_no { get; set; }
        public string code { get; set; }
        public byte[] description { get; set; }
    }
    /// <summary>
    /// List of data from front end
    /// </summary>
    public class TermsandConFileDataList
    {
        public long start_date { get; set; }
        public long end_date { get; set; }
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

    public class InactivateTandCRequestList
    {
        public int orgId { get; set; }
        public int accountId { get; set; }
        public List<InactivateTandCRequest> _ids { get; set; }
    }

    public class InactivateTandCRequest
    {
        public int id { get; set; }
    }

    public class InactivateTandCStatusResponceList
    {
        public List<InactivateTandCStatusResponce> _ids { get; set; }
    }

    public class InactivateTandCStatusResponce
    {
        public int id { get; set; }
        public string action { get; set; }
    }

    public class VersionByID
    {
        public int orgId { get; set; }
        public int levelCode { get; set; }
        public int accountId { get; set; }
    }
}

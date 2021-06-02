using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.termsandconditions.entity
{
    public class TermsAndConditions
    {
        public int Id { get; set; }
        public string Version_no { get; set; }
        public string Code { get; set; }
        public byte[] Description { get; set; }
        public char State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Created_At { get; set; }
        public int Created_by { get; set; }
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
        public string Version_no { get; set; }
    }

    /// <summary>
    /// This Model is passed as payload from front end
    /// </summary>
    public class TermsandConFileData
    {
        public string FileName { get; set; }
        public string Version_no { get; set; }
        public string Code { get; set; }
        public byte[] Description { get; set; }
    }
    /// <summary>
    /// List of data from front end
    /// </summary>
    public class TermsandConFileDataList
    {
        public long Start_date { get; set; }
        public long End_date { get; set; }
        public int Created_by { get; set; }
        public List<TermsandConFileData> Data { get; set; }
    }
    /// <summary>
    /// Data returned from DB is saved here
    /// </summary>
    public class VersionAndCodeExits
    {
        public int Id { get; set; }
        public string Version_no { get; set; }
        public string Code { get; set; }
    }

    /// <summary>
    /// Response to send back with TermsAndCondition record status.
    /// </summary>
    public class TermsAndConditionResponseList
    {
        public List<TermsAndConditionResponse> TermsAndConditionDetails { get; set; }
    }
    public class TermsAndConditionResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Action { get; set; }
    }

    public class InactivateTandCRequestList
    {
        public int OrgId { get; set; }
        public int AccountId { get; set; }
        public List<InactivateTandCRequest> Ids { get; set; }
    }

    public class InactivateTandCRequest
    {
        public int Id { get; set; }
    }

    public class InactivateTandCStatusResponceList
    {
        public List<InactivateTandCStatusResponce> Ids { get; set; }
    }

    public class InactivateTandCStatusResponce
    {
        public int Id { get; set; }
        public string Action { get; set; }
    }

    public class VersionByID
    {
        public int OrgId { get; set; }
        public int LevelCode { get; set; }
        public int AccountId { get; set; }
    }
}

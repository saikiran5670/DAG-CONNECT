using System;
using System.Collections.Generic;
using System.Text;

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
}

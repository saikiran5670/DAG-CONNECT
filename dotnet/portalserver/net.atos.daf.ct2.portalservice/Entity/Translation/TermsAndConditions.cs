using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}

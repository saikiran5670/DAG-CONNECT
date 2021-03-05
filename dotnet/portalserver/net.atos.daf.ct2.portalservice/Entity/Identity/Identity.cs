using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Identity
{
    public class Identity
    {
        public Account accountInfo { get; set; }
        public List<KeyValue> accountOrganization { get; set; }
        public List<AccountOrgRole> accountRole { get; set; }
    }
}

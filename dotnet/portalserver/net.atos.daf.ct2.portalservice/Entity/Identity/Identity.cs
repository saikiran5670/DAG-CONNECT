using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Identity
{
    public class Identity
    {
        public Account AccountInfo { get; set; }
        public List<KeyValue> AccountOrganization { get; set; }
        public List<AccountOrgRole> AccountRole { get; set; }
    }
}

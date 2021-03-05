using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Identity
{
    public class AccountOrgRole
    {
        public int id { get; set; }
        public string name { get; set; }
        public int organization_Id { get; set; }
    }
}

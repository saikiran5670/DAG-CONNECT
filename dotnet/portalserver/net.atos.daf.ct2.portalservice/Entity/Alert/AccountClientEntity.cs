using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AccountClientEntity
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public string HubClientId { get; set; }
    }
}

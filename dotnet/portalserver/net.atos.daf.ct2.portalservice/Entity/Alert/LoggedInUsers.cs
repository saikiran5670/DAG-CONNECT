using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class LoggedInUsers
    {
        public int OrganizationId { get; set; }
        public int AccountId { get; set; }
        public int HubClientId { get; set; }

    }
}

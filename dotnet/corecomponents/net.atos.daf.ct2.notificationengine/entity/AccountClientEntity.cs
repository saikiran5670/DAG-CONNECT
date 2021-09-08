using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class AccountClientEntity
    {
        public int AlertId { get; set; }
        public int OrganizationId { get; set; }
        public string HubClientId { get; set; }
    }

    public class AlertMessageEntity
    {
        public int AlertId { get; set; }
        public string Vin { get; set; }
    }
}

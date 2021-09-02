using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class AccountClientEntity
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public string HubClientId { get; set; }
    }

    public class AlertMessageAndAccountClientEntity
    {
        public List<AccountClientEntity> AccountClientEntity { get; set; }
        public TripAlert TripAlert { get; set; }
    }
}

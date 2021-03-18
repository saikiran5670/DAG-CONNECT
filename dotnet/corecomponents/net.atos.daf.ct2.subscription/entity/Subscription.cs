using System;

namespace net.atos.daf.ct2.subscription.entity
{
    public class Subscription
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime OrganizationDate { get; set; }
    }

}

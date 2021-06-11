using System;

namespace net.atos.daf.ct2.subscription.entity
{
    public class PackageSubscription
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int PackageId { get; set; }
        public char SubscriptionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public char Status { get; set; }
    }

}

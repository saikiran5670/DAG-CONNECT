using System.Collections.Generic;

namespace net.atos.daf.ct2.featureactivationservice.Entity
{
    public class SubsCriptionEntity
    {
        public ActiveSubscription SubscribeEvent { get; set; }
        public DeactiveUnSubscription UnsubscribeEvent { get; set; }
    }
    public class ActiveSubscription
    {
        public string OrganizationId { get; set; }//M
        public string packageId { get; set; }//M
        public List<string> VINs { get; set; }
        public string StartDateTime { get; set; }

    }

    public class DeactiveUnSubscription
    {
        public string OrganizationID { get; set; }//M
        public string PackageId { get; set; }//M
        public string OrderID { get; set; }
        public List<string> VINs { get; set; }
        public string EndDateTime { get; set; }

    }
}

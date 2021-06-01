using System.Collections.Generic;

namespace net.atos.daf.ct2.subscription.entity
{
    public class UnSubscription
    {
        public string OrganizationID { get; set; }//M
        public string OrderID { get; set; }//M
        public List<string> VINs { get; set; }
        public long EndDateTime { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.subscription.entity
{
    public class SubscriptionDetails
    {
        public string Subscription_Id { get; set; }
        public string OrgName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Package_Code { get; set; }
        public long Subscription_Start_Date { get; set; }
        public long Subscription_End_Date { get; set; }
        public string State { get; set; }
        public int Count { get; set; }
    }
    public class SubscriptionDetailsRequest
    {
        public int Organization_Id { get; set; }
        public string Type { get; set; }
        public StatusType State { get; set; }

    }
    public enum StatusType
    {
        None = 0,
        A = 1,
        D = 2
    }
}

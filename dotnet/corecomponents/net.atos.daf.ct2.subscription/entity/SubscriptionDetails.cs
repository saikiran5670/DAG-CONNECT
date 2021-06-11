using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.subscription.entity
{
    public class SubscriptionDetails
    {
        [Column("subscription_id")]
        public string SubscriptionId { get; set; }
        [Column("orgname")]
        public string OrgName { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("package_code")]
        public string PackageCode { get; set; }
        [Column("subscription_start_date")]
        public long SubscriptionStartDate { get; set; }
        [Column("subscription_end_date")]
        public long SubscriptionEndDate { get; set; }
        [Column("state")]
        public string State { get; set; }
        [Column("count")]
        public int Count { get; set; }
    }
    public class SubscriptionDetailsRequest
    {
        [Column("organization_id")]
        public int OrganizationId { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("state")]
        public StatusType State { get; set; }

    }
    public enum StatusType
    {
        None = 0,
        A = 1,
        D = 2
    }
}

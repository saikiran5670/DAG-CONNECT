using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace net.atos.daf.ct2.subscription.entity
{
    public class SubscriptionActivation
    {
        public string OrganizationId { get; set; }//M
        [Column("packageId")]
        public string PackageId { get; set; }//M
        public List<string> VINs { get; set; }
        public long StartDateTime { get; set; }

    }

    public class Package
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("packagecode")]
        public string PackageCode { get; set; }
    }

    public class SubscriptionIdType
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("type")]
        public string Type { get; set; }
        public char State { get; set; }
    }

    public class SubscriptionIdStatus
    {
        [Column("subscription_id")]
        public long SubscriptionId { get; set; }
        [Column("state")]
        public string State { get; set; }
    }
    public class UnSubscribeVin
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("state")]
        public string State { get; set; }
    }
    public class SubscriptionResponse
    {
        public SubscriptionResponse()
        {
            Response = new SubscriptionSubResponse();
        }
        public SubscriptionResponse(string code, object value)
        {
            Value = value;
            ErrorCode = code;
        }

        public SubscriptionSubResponse Response { get; set; }
        public string ErrorCode { get; set; }
        public object Value { get; set; }
    }

    public class SubscriptionSubResponse
    {
        public string OrderId { get; set; }//M
        public int NumberOfVehicles { get; set; }//M

    }
}

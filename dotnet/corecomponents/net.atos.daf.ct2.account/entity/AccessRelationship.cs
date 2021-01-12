using System;

namespace net.atos.daf.ct2.account
{
    public class AccessRelationship
    {
        public int Id { get; set; }
        public AccessType AccessType { get; set; }
        public int accountGroupId { get; set; }
        public int vehicleGroupId { get; set; }
        public DateTime ? startDate { get; set; }
        public DateTime ? endDate{ get; set; }
    }
}

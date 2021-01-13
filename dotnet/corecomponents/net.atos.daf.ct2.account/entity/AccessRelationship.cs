using System;

namespace net.atos.daf.ct2.account
{
    public class AccessRelationship
    {
        public int Id { get; set; }
        public AccessType AccessType { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
        public DateTime ? StartDate { get; set; }
        public DateTime ? EndDate{ get; set; }
    }
}

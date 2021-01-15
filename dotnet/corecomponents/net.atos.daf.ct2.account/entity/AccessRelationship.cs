using System;
using System.Collections.Generic;
using net.atos.daf.ct2.account.ENUM;

namespace net.atos.daf.ct2.account.entity
{
    public class AccessRelationship
    {
        public int Id { get; set; }
        public AccessRelationType AccessRelationType { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
        public DateTime ? StartDate { get; set; }
        public DateTime ? EndDate{ get; set; }
        public List<int> VehicleGroupIds { get; set; }
    }
}

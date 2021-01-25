using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class GroupFilterRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public bool GroupRef { get; set; }
        //public bool GroupRefCount { get; set; }
        public List<int> GroupIds { get; set; }
        public bool IsGroup { get; set; }
    }
}

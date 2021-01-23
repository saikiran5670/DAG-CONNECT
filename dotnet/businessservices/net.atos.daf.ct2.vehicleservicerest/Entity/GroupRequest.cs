using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class GroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public List<GroupRefRequest> GroupRef { get; set; }
    }
}

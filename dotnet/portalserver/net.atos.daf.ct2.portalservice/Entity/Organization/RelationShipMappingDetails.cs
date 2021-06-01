using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class RelationShipMappingDetails
    {
        public List<VehileGroupData> VehicleGroup { get; set; }
        public List<RelationshipData> RelationShipData { get; set; }
        public List<OrganizationData> OrganizationData { get; set; }
    }

    public class VehileGroupData
    {
        public int VehiclegroupID { get; set; }
        public string GroupName { get; set; }
    }

    public class RelationshipData
    {
        public int RelationId { get; set; }
        public string RelationName { get; set; }
    }

    public class OrganizationData
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }
}

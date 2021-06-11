using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class RelationshipMapping
    {
        public int RelationshipId { get; set; }
        public int VehicleId { get; set; }
        public string VIN { get; set; }
        public int VehicleGroupId { get; set; }
        public int OwnerOrgId { get; set; }
        public int CreatedOrgId { get; set; }
        public int TargetOrgId { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public bool AllowChain { get; set; }
        public bool IsFirstRelation { get; set; }
        public long CreatedAt { get; set; }

    }
    public class OrganizationRelationship
    {
        public int OrganizationId { get; set; }
        public int FeatureSetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Boolean Isactive { get; set; }
        public int Level { get; set; }
        public long Createdat { get; set; }
    }
}

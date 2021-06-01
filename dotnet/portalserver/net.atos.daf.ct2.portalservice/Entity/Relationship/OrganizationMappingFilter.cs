namespace net.atos.daf.ct2.portalservice.Entity.Relationship
{
    public class OrganizationMappingFilter
    {
        public int Id { get; set; }
        public int relationship_id { get; set; }
        public int vehicle_group_id { get; set; }
        public int created_org_id { get; set; }
        public int target_org_id { get; set; }

    }
}

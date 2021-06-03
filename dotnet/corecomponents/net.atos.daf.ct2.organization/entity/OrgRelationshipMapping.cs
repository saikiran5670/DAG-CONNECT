namespace net.atos.daf.ct2.organization.entity
{
    public class OrgRelationshipMapping
    {
        public int Id { get; set; }
        public int RelationId { get; set; }
        public int Type { get; set; }
        public int OrganizationId { get; set; }
        public int VehicleId { get; set; }
        public int VehicleGroupId { get; set; }
    }
}

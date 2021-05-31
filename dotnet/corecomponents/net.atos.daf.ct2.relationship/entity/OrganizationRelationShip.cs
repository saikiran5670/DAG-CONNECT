namespace net.atos.daf.ct2.relationship.entity
{
    public class OrganizationRelationShip
    {
        public int Id { get; set; }
        public int relationship_id { get; set; }
        public int vehicle_id { get; set; }
        public string VIN { get; set; }
        public int vehicle_group_id { get; set; }
        public int owner_org_id { get; set; }
        public int created_org_id { get; set; }
        public int target_org_id { get; set; }
        public long start_date { get; set; }
        public long end_date { get; set; }
        public bool allow_chain { get; set; }
        public bool isFirstRelation { get; set; }
        public long created_at { get; set; }
        public string VehicleGroupName { get; set; }
        public string OrganizationName { get; set; }
        public string RelationshipName { get; set; }

    }
}

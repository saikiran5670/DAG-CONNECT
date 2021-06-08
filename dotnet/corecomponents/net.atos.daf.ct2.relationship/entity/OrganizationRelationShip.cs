namespace net.atos.daf.ct2.relationship.entity
{
    public class OrganizationRelationShip
    {
        public int Id { get; set; }
        public int Relationship_id { get; set; }
        public int Vehicle_id { get; set; }
        public string VIN { get; set; }
        public int Vehicle_group_id { get; set; }
        public int Owner_org_id { get; set; }
        public int Created_org_id { get; set; }
        public int Target_org_id { get; set; }
        public long Start_date { get; set; }
        public long End_date { get; set; }
        public bool Allow_chain { get; set; }
        public bool IsFirstRelation { get; set; }
        public long Created_at { get; set; }
        public string VehicleGroupName { get; set; }
        public string OrganizationName { get; set; }
        public string RelationshipName { get; set; }

    }
}

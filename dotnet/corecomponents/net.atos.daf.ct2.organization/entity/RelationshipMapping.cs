using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class RelationshipMapping
    {
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

    }
    public class OrganizationRelationship
    {
        public int organization_id { get; set; }
        public int feature_set_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public Boolean is_active { get; set; }
        public int level { get; set; }
        public long created_at { get; set; }
    }
}

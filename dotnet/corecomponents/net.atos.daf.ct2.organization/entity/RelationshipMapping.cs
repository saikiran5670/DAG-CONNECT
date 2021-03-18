using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class RelationshipMapping
    {
        public int relationship_id { get; set; } 
        public int vehicle_id { get; set; } 
        public int VIN { get; set; } 
        public int vehicle_group_id { get; set; } 
        public int owner_org_id { get; set; } 
        public int created_org_id { get; set; } 
        public int target_org_id { get; set; } 
        public long start_date { get; set; } 
        public long end_date { get; set; } 
        public bool allow_chain { get; set; } 

        public bool isFirstRelation { get; set; } 



    }
}

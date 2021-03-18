using System;
using System.Collections.Generic;
using net.atos.daf.ct2.account.ENUM;

namespace net.atos.daf.ct2.account.entity
{
    public class AccessRelationship
    {
        public int Id { get; set; }
        public AccessRelationType AccessRelationType { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
        public DateTime ? StartDate { get; set; }
        public DateTime ? EndDate{ get; set; }
        public List<int> VehicleGroupIds { get; set; }
        public bool Exists { get; set; }

        public AccessRelationship() { }

        public AccessRelationship(AccessRelationType accessRelationType, int accountGroupId, int vehicleGroupId)
        {
            AccessRelationType = accessRelationType;
            VehicleGroupId = vehicleGroupId;
            AccountGroupId = accountGroupId;
        }
    }
    //public class VehicleAccessRelationship
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public AccessRelationType AccessType { get; set; }
    //    public int VehicleCount { get; set; }
    //    public bool IsGroup { get; set; }
    //    public List<RelationshipData> AccountsAccountGroups { get; set; }
    //}
    //public class VehicleAccessRelationshipEntity
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string access_type { get; set; }
    //    public int vehicle_count  { get; set; }
    //    public bool is_group { get; set; }
    //    public int account_id { get; set; }
    //    public string account_name { get; set; }
    //    public bool is_account_group { get; set; }

    //}
    public class AccountAccessRelationshipEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string access_type { get; set; }
        public int count { get; set; }
        public bool is_group { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public bool is_ag_vg_group { get; set; }

    }
    public class AccountVehicleEntity
    {
        public int id { get; set; }
        public string name { get; set; }        
        public int count { get; set; }
        public bool is_group { get; set; }
    }
    public class AccountVehicleAccessRelationship
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccessRelationType AccessType { get; set; }
        public bool IsGroup { get; set; }
        public int Count { get; set; }
        public List<RelationshipData> RelationshipData { get; set; }
    }
    public class RelationshipData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
    }
}

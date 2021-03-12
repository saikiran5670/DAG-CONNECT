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
    }
    public class VehicleAccessRelationship
    {
        public int Id { get; set; }
        public AccessRelationType AccessType { get; set; }        
        public bool IsGroup { get; set; }
        public List<RelationshipData> AccountsAccountGroups { get; set; }
    }
    public class AccountAccessRelationship
    {
        public int Id { get; set; }
        public AccessRelationType AccessType { get; set; }
        public bool IsGroup { get; set; }
        public List<RelationshipData> VehiclesVehicleGroups { get; set; }
    }
    public class RelationshipData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

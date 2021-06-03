using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Account
{


    public class AccessRelationshipResponseDetail
    {
        public List<VehicleAccount> Account { get; set; }
        public List<VehicleAccount> Vehicle { get; set; }
    }

    public class AccessRelationshipResponse
    {
        public List<AccessRelationshipDetail> Account { get; set; }
        public List<AccessRelationshipDetail> Vehicle { get; set; }
    }

    public class AccessRelationshipRequest
    {
        public int Id { get; set; }
        public string AccessType { get; set; }
        public bool IsGroup { get; set; }
        public List<RelationshipData> AssociatedData { get; set; }
        public int OrganizationId { get; set; }
    }
    public class AccessRelationshipDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccessType { get; set; }
        public bool IsGroup { get; set; }
        public int Count { get; set; }
        public List<RelationshipData> AssociatedData { get; set; }
    }

    public class RelationshipData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
    }
    public class VehicleAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public int Count { get; set; }
        public string VIN { get; set; }
        public string RegistrationNo { get; set; }
    }

}


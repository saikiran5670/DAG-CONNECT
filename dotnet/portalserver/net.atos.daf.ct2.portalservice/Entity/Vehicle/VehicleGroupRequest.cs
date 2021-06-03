using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class VehicleGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public string GroupType { get; set; }
        public string FunctionEnum { get; set; }
        public List<GroupRefRequest> Vehicles { get; set; }

    }
    public class GroupRefRequest
    {
        public int VehicleGroupId { get; set; }
        public int VehicleId { get; set; }
    }

    public class VehicleGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string ObjectType { get; set; }
        public string GroupType { get; set; }
        //public string Argument { get; set; }
        public string FunctionEnum { get; set; }
        public int OrganizationId { get; set; }
        public int? RefId { get; set; }
        public string Description { get; set; }
        public List<GroupRefRequest> GroupRef { get; set; }
        public int GroupRefCount { get; set; }
    }

    public class DynamicVehicleGroupRequest
    {
        public int GroupId { get; set; }
        public string GroupType { get; set; }
        public string FunctionEnum { get; set; }
        public int OrganizationId { get; set; }
        public int RelationShipId { get; set; }
    }

}

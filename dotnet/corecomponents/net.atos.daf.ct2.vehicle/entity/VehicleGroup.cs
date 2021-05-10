using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleGroup
    {
        public int Id { get; set; }
        public int Group_Id { get; set; }
        public string Name { get; set; }

        //  public int VehicleGroupID { get; set; }
        // public int OrganizationID { get; set; }       
        // public string Name { get; set; }
        // public string Description { get; set; }
        // public int ParentID { get; set; }     
        // public bool  IsActive { get; set; }
        // public DateTime CreatedDate { get; set; }
        // public int CreatedBy { get; set; }
        // public DateTime UpdatedDate { get; set; }
        // public int UpdatedBy { get; set; }
        // public bool IsDefaultGroup { get; set; }
        // public bool IsUserDefindGroup { get; set; }
        // public List<Vehicle> Vehicles { get; set; }
        // public string VehicleOrgIds { get; set; }
    }

    public class VehicleGroupDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupType { get; set; }
        public string GroupMethod { get; set; }
        public int RefId { get; set; }
    }
}
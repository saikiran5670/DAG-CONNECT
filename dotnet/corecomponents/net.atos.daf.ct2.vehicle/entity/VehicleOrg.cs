using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleOrg
    {
        public int VehicleOrgId { get; set; }
        public int OrganizationId { get; set; }
        public int VehicleId { get; set; }
        public string name { get; set; }
        public bool OptOutStatus { get; set; }
        public DateTime OptOutStatusChangedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}

using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleGroupVehicleMapping
    {
        public int VehicleGroupVehicleMappingId { get; set; }
        public int VehicleGroupId { get; set; }
        public int VehicleOrgId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}

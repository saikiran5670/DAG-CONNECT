using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertCategoryResponse
    {
        public List<EnumTranslation> EnumTranslation { get; set; } = new List<EnumTranslation>();
        public List<VehicleGroup> VehicleGroup { get; set; } = new List<VehicleGroup>();
    }
    public class EnumTranslation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Enum { get; set; }
        public string ParentEnum { get; set; }
        public string Key { get; set; }
    }
    public class VehicleGroup
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleId { get; set; }        
        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public bool SubcriptionStatus { get; set; }
    }
}

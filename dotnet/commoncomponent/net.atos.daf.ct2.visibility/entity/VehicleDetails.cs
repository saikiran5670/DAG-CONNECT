using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.visibility.entity
{
    public class VehicleDetails
    {
        public int VehicleGroupId { get; set; }
        public int AccountId { get; set; }
        public string ObjectType { get; set; }
        public string GroupType { get; set; }
        public string FunctionEnum { get; set; }
        public int OrganizationId { get; set; }
        public string AccessType { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public string RegistrationNo { get; set; }
    }
}

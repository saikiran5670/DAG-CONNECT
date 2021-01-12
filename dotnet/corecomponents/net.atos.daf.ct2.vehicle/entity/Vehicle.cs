using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class Vehicle
    {
          //public int VehicleID { get; set; }
        public int ID { get; set; }
        public int Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }       
        public string License_Plate_Number { get; set; }
        public DateTime ManufactureDate { get; set; }
        public string ChassisNo { get; set; } 
        public VehicleStatusType Status { get; set; }
        public DateTime Status_Changed_Date { get; set; }
        public DateTime Termination_Date { get; set; } 
        public bool  IsActive { get; set; }
       // public int Account_Id { get; set; }
       // public OptInOptOutType Type { get; set; } 
        public VehicleProperty vehicleProperty { get; set; }
        public List<VehicleOptInOptOut> vehicleOptInOptOut { get; set; }
        
        
        
        // public DateTime CreatedDate { get; set; }
        // public int CreatedBy { get; set; }
        // public DateTime UpdatedDate { get; set; }
        // public int UpdatedBy { get; set; }
    }
}

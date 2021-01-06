using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class Vehicle
    {
         public int VehicleID { get; set; }
        public string VIN { get; set; }       
        public string RegistrationNo { get; set; }
        public string ChassisNo { get; set; } 
        public DateTime TerminationDate { get; set; } 
        public bool  IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}

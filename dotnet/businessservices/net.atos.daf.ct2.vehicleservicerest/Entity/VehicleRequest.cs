using System;

namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class VehicleRequest
    {
        public int ID { get; set; }
        //public int ? Organization_Id { get; set; }
        public string Name { get; set; }
        //public string VIN { get; set; }       
        public string License_Plate_Number { get; set; }
        // public bool  IsActive { get; set; }
        // public string Vid { get; set; }
        // public string Tcu_Id { get; set; }
        // public string Tcu_Serial_Number { get; set; }
        // public string Tcu_Brand { get; set; }
        // public string Tcu_Version { get; set; }
        // public bool Is_Tcu_Register { get; set; }
        // public DateTime ? Reference_Date { get; set; }
    }
}

using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class Vehicle
    {
        //public int VehicleID { get; set; }
        public int ID { get; set; }
        public int? Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }
        public string License_Plate_Number { get; set; }
        //public DateTime ManufactureDate { get; set; }
        public string Description { get; set; }
        public VehicleCalculatedStatus Status { get; set; }
        public DateTime Status_Changed_Date { get; set; }
        public DateTime? Termination_Date { get; set; }
        //public bool  IsActive { get; set; }
        // public int Account_Id { get; set; }
        // public OptInOptOutType Type { get; set; } 
        //  public VehicleProperty vehicleProperty { get; set; }
        //  public List<VehicleOptInOptOut> vehicleOptInOptOut { get; set; }
        public bool IsVehicleGroup { get; set; }
        public int VehicleCount { get; set; }
        public string ModelId { get; set; }
        public string Vid { get; set; }
        public VehicleType Type { get; set; }
        public string Tcu_Id { get; set; }
        public string Tcu_Serial_Number { get; set; }
        public string Tcu_Brand { get; set; }
        public string Tcu_Version { get; set; }
        public bool Is_Tcu_Register { get; set; }
        public DateTime? Reference_Date { get; set; }
        public int? VehiclePropertiesId { get; set; }
        public long? CreatedAt { get; set; }
        public int Oem_id { get; set; }
        public int Oem_Organisation_id { get; set; }
        public VehicleStatusType Opt_In { get; set; }
        public bool Is_Ota { get; set; }

        // public DateTime CreatedDate { get; set; }
        // public int CreatedBy { get; set; }
        // public DateTime UpdatedDate { get; set; }
        public int Modified_By { get; set; }
        public string RelationShip { get; set; }
        public string AssociatedGroups { get; set; }
        public string Fuel { get; set; }
        public bool VehicleNameExists { get; set; }
        public bool VehicleLicensePlateNumberExists { get; set; }
        public bool IPPS { get; set; } = false;
    }
}

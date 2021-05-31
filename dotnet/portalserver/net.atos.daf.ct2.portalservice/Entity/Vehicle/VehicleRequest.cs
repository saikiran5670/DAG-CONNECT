using System;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class VehicleRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string License_Plate_Number { get; set; }
        public int Organization_Id { get; set; }
    }

    public class VehicleCreateRequest
    {
        public int ID { get; set; }
        public int? Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }
        public string License_Plate_Number { get; set; }
        public string Status { get; set; }

    }

    public class VehicleFilterRequest
    {
        public int VehicleId { get; set; }
        public int OrganizationId { get; set; }
        public string VehicleIdList { get; set; }
        public string VIN { get; set; }
        //public string Status { get; set; }
    }

    public class VehicleResponse
    {

        public int ID { get; set; }
        public int? Organization_Id { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }
        public string License_Plate_Number { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Status_Changed_Date { get; set; }
        public DateTime? Termination_Date { get; set; }
        public bool IsVehicleGroup { get; set; }
        public int VehicleCount { get; set; }
        public string ModelId { get; set; }
        public string Vid { get; set; }
        public string Tcu_Id { get; set; }
        public string Tcu_Serial_Number { get; set; }
        public string Tcu_Brand { get; set; }
        public string Tcu_Version { get; set; }
        public bool Is_Tcu_Register { get; set; }
        public DateTime? Reference_Date { get; set; }
        public int VehiclePropertiesId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleProperty
    {

         public int ID { get; set; }
        public int VehicleId { get; set; }
        public string VIN { get; set; }
        public string Classification_Make { get; set; }
        public string Classification_Series { get; set; }
        public VehicleType Classification_Type { get; set; }
        public string Classification_Model { get; set; }
        public string License_Plate_Number { get; set; }
        public int Tank_Nr { get; set; }
        public int Tank_Volume { get; set; }
        public string Chasis_Id { get; set; }
        public bool SideSkirts { get; set; }
        public bool SideCollars { get; set; }
        public int RearOverhang { get; set; }
        public string Engine_ID { get; set; }
        public string Engine_Type { get; set; }
        public int Engine_Power { get; set; }
        public string Engine_Coolant { get; set; }
        public string Engine_EmissionLevel { get; set; }
        public string GearBox_Id { get; set; }
        public string GearBox_Type { get; set; }
        public string DriverLine_AxleConfiguration { get; set; }
        public decimal DriverLine_Wheelbase { get; set; }
        public string DriverLine_Tire_Size { get; set; }
        public int DriverLine_FrontAxle_Position { get; set; }
        public int DriverLine_FrontAxle_Load { get; set; }
        public int DriverLine_RearAxle_Position { get; set; }
        public int DriverLine_RearAxle_Load { get; set; }
        public decimal DriverLine_RearAxle_Ratio { get; set; }
        public string DriverLine_Cabin_ID { get; set; }
        public string DriverLine_Cabin_Color_ID { get; set; }
        public string DriverLine_Cabin_Color_Value { get; set; }
        public int Dimensions_Size_Length { get; set; }
        public int Dimensions_Size_Width { get; set; }
        public int Dimensions_Size_Height { get; set; }
        public int Dimensions_Size_Weight { get; set; }
         public DateTime ManufactureDate { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public DateTime DeliveryDate { get; set; }
        // public string Party_Role_1 { get; set; }
        // public int Party_Role_ID_1 { get; set; }
        // public string Party_Role_Name_1 { get; set; }
        // public string Party_Role_2 { get; set; }
        // public int Party_Role_ID_2 { get; set; }
        // public string Party_Role_Name_2 { get; set; }
    }
}

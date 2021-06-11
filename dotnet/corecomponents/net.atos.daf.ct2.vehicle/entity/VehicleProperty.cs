using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleProperty
    {

        public int ID { get; set; }
        public string VIN { get; set; }
        public string Classification_Make { get; set; }
        public string Classification_Series_Id { get; set; }
        public string Classification_Series_VehicleRange { get; set; }
        public string Classification_Type_Id { get; set; }
        public string Classification_Model_Id { get; set; }
        public string Classification_ModelYear { get; set; }
        public string License_Plate_Number { get; set; }
        public string Chassis_Id { get; set; }
        public string Chassis_SideSkirts { get; set; }
        public string Chassis_SideCollars { get; set; }
        public string Chassis_RearOverhang { get; set; }
        public string Engine_ID { get; set; }
        public string Engine_Type { get; set; }
        public string Engine_Power { get; set; }
        public string Engine_Coolant { get; set; }
        public string Engine_EmissionLevel { get; set; }
        public string GearBox_Id { get; set; }
        public string GearBox_Type { get; set; }
        public string DriverLine_AxleConfiguration { get; set; }
        public string DriverLine_Wheelbase { get; set; }
        public string DriverLine_Tire_Size { get; set; }
        public string DriverLine_Cabin_ID { get; set; }
        public string DriverLine_Cabin_Type { get; set; }
        public string DriverLine_Cabin_RoofSpoiler { get; set; }
        public string DriverLine_ElectronicControlUnit_Type { get; set; }
        public string DriverLine_ElectronicControlUnit_Name { get; set; }
        public string Dimensions_Size_Length { get; set; }
        public string Dimensions_Size_Width { get; set; }
        public string Dimensions_Size_Height { get; set; }
        public string Dimensions_Size_Weight_Type { get; set; }
        public string Dimensions_Size_Weight_Value { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Org_Id { get; set; }
        public List<VehicleAxelInformation> VehicleAxelInformation { get; set; }
        //public List<VehicleAxelInformation> VehicleRearAxelInformation {get; set;}
        public List<VehicleFuelTankProperties> VehicleFuelTankProperties { get; set; }

        // public int Party_Role_ID_1 { get; set; }
        // public string Party_Role_Name_1 { get; set; }
        // public string Party_Role_2 { get; set; }
        // public int Party_Role_ID_2 { get; set; }
        // public string Party_Role_Name_2 { get; set; }
        public int VehicleId { get; set; }
        public string Fuel { get; set; }
    }

    public class VehicleAxelInformation
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Position { get; set; }
        public string Load { get; set; }
        public AxelType AxelType { get; set; }
        public string Springs { get; set; }
        public string Type { get; set; }
        public string Ratio { get; set; }
        public bool Is_Wheel_Tire_Size_Replaced { get; set; }
        public string Size { get; set; }
    }

    public class VehicleFuelTankProperties
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Chassis_Tank_Nr { get; set; }
        public string Chassis_Tank_Volume { get; set; }
    }

}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using System.Linq;

namespace net.atos.daf.ct2.vehicle.test
{
    [TestClass]
    public class vehiclerepositorytest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IVehicleRepository _vehicleRepository;

        public vehiclerepositorytest()
        {
              string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _vehicleRepository = new VehicleRepository(_dataAccess);
            
        }

        [TestMethod]
        public void CreateVehicle()
        {
            Vehicle Objvehicle = new Vehicle();

            Objvehicle.Organization_Id = 1;
            Objvehicle.Name = "Vehicle 4";
            Objvehicle.VIN = "V69856";
            Objvehicle.License_Plate_Number = "REG3264";
            Objvehicle.ManufactureDate = DateTime.Now;
            Objvehicle.ChassisNo = "123545";
            Objvehicle.Status_Changed_Date = DateTime.Now;
            //Objvehicle.Status=1;//VehicleStatusType.OptIn;
            Objvehicle.Termination_Date=DateTime.Now;
            var resultvehicle = _vehicleRepository.Create(Objvehicle).Result;
            Assert.IsNotNull(resultvehicle);
            Assert.IsTrue(resultvehicle.ID > 0);

        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetVehicle()
        {
            VehicleFilter ObjFilter=new VehicleFilter ();
            //ObjFilter.OrganizationId=1;
            //ObjFilter.VehicleId=5;
            //ObjFilter.VIN="H";
            ObjFilter.VehicleIdList="7,8";
            
            var resultvehicleList = _vehicleRepository.Get(ObjFilter).Result;
            Assert.IsNotNull(resultvehicleList);
            Assert.IsTrue(resultvehicleList.Count() > 0);

        }

         [TestMethod]
        public void UpdateVehicle()
        {
            Vehicle Objvehicle = new Vehicle();            
            Objvehicle.ID = 5;
            Objvehicle.Name = "Vehicle 2";
            Objvehicle.License_Plate_Number = "123654889";
            var resultUpdatevehicle = _vehicleRepository.Update(Objvehicle).Result;
            Assert.IsNotNull(resultUpdatevehicle);
            Assert.IsTrue(resultUpdatevehicle.ID > 0);

        }

        [TestMethod]
        public void UpdateStatus()
        {
            VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();            
            ObjvehicleOptInOptOut.RefId = 5;
            ObjvehicleOptInOptOut.AccountId = 4;
          //  ObjvehicleOptInOptOut.Status=2;//VehicleStatusType.OptOut;
            ObjvehicleOptInOptOut.Type=OptInOptOutType.VehicleLevel;
            ObjvehicleOptInOptOut.Date=DateTime.Now;
            var resultUpdateOptInOptOutvehicle = _vehicleRepository.UpdateStatus(ObjvehicleOptInOptOut).Result;
            Assert.IsNotNull(resultUpdateOptInOptOutvehicle);
            Assert.IsTrue(resultUpdateOptInOptOutvehicle.RefId > 0);

        }

        [TestMethod]
        public void CreateVehicleProperty()
        {
            VehicleProperty ObjVehicleProperty = new VehicleProperty();            
            ObjVehicleProperty.VehicleId = 5;
            ObjVehicleProperty.ManufactureDate = DateTime.Now;
            ObjVehicleProperty.RegistrationDateTime=DateTime.Now;
            ObjVehicleProperty.DeliveryDate=DateTime.Now;
            ObjVehicleProperty.Classification_Make="Make 1";
            ObjVehicleProperty.Classification_Model="Model 1";
            ObjVehicleProperty.Classification_Series="Series 1";
            ObjVehicleProperty.Classification_Type="Type 1";
            ObjVehicleProperty.Dimensions_Size_Length=1;
            ObjVehicleProperty.Dimensions_Size_Width=2;
            ObjVehicleProperty.Dimensions_Size_Height=3;
            ObjVehicleProperty.Dimensions_Size_Weight=4;
            ObjVehicleProperty.Engine_ID="1";
            ObjVehicleProperty.Engine_Type=EngineType.Trucks;
            ObjVehicleProperty.Engine_Power=500;
            ObjVehicleProperty.Engine_Coolant="";
            ObjVehicleProperty.Engine_EmissionLevel="";
            ObjVehicleProperty.Chasis_Id="";
            ObjVehicleProperty.SideSkirts=true;
            ObjVehicleProperty.SideCollars=true;
            ObjVehicleProperty.RearOverhang=122;
            ObjVehicleProperty.Tank_Nr=155;
            ObjVehicleProperty.Tank_Volume=155;
            ObjVehicleProperty.DriverLine_AxleConfiguration="";
            ObjVehicleProperty.DriverLine_Wheelbase=1;
            ObjVehicleProperty.DriverLine_Tire_Size="1.2";
            ObjVehicleProperty.DriverLine_Tire_Size="1.2";
            ObjVehicleProperty.DriverLine_FrontAxle_Position="1.2";
            ObjVehicleProperty.DriverLine_FrontAxle_Load=2;
            ObjVehicleProperty.DriverLine_RearAxle_Position=2;
            ObjVehicleProperty.DriverLine_RearAxle_Load=2;
            ObjVehicleProperty.DriverLine_RearAxle_Ratio=2;
            ObjVehicleProperty.GearBox_Id="2";
            ObjVehicleProperty.GearBox_Type=GearBoxType.GrearBox1;
            ObjVehicleProperty.DriverLine_Cabin_ID="2";
            ObjVehicleProperty.DriverLine_Cabin_Color_Value="2";

            var resultCreateProperty = _vehicleRepository.CreateProperty(ObjVehicleProperty).Result;
            Assert.IsNotNull(resultCreateProperty);
            Assert.IsTrue(resultCreateProperty.ID > 0);

        }

        [TestMethod]
        public void UpdateVehicleProperty()
        {
            VehicleProperty ObjVehicleProperty = new VehicleProperty();            
            ObjVehicleProperty.VehicleId = 5;
            ObjVehicleProperty.ManufactureDate = DateTime.Now;
            ObjVehicleProperty.RegistrationDateTime=DateTime.Now;
            ObjVehicleProperty.DeliveryDate=DateTime.Now;
            ObjVehicleProperty.Classification_Make="Make 1";
            ObjVehicleProperty.Classification_Model="Model 1";
            ObjVehicleProperty.Classification_Series="Series 1";
            ObjVehicleProperty.Classification_Type="Type 1";
            ObjVehicleProperty.Dimensions_Size_Length=1;
            ObjVehicleProperty.Dimensions_Size_Width=2;
            ObjVehicleProperty.Dimensions_Size_Height=3;
            ObjVehicleProperty.Dimensions_Size_Weight=4;
            ObjVehicleProperty.Engine_ID="1";
            ObjVehicleProperty.Engine_Type=EngineType.Heavy;
            ObjVehicleProperty.Engine_Power=500;
            ObjVehicleProperty.Engine_Coolant="";
            ObjVehicleProperty.Engine_EmissionLevel="";
            ObjVehicleProperty.Chasis_Id="";
            ObjVehicleProperty.SideSkirts=true;
            ObjVehicleProperty.SideCollars=true;
            ObjVehicleProperty.RearOverhang=122;
            ObjVehicleProperty.Tank_Nr=155;
            ObjVehicleProperty.Tank_Volume=155;
            ObjVehicleProperty.DriverLine_AxleConfiguration="";
            ObjVehicleProperty.DriverLine_Wheelbase=1;
            ObjVehicleProperty.DriverLine_Tire_Size="1.2";
            ObjVehicleProperty.DriverLine_Tire_Size="1.2";
            ObjVehicleProperty.DriverLine_FrontAxle_Position="1.2";
            ObjVehicleProperty.DriverLine_FrontAxle_Load=2;
            ObjVehicleProperty.DriverLine_RearAxle_Position=2;
            ObjVehicleProperty.DriverLine_RearAxle_Load=2;
            ObjVehicleProperty.DriverLine_RearAxle_Ratio=2;
            ObjVehicleProperty.GearBox_Id="2";
            ObjVehicleProperty.GearBox_Type=GearBoxType.GrearBox2;
            ObjVehicleProperty.DriverLine_Cabin_ID="2";
            ObjVehicleProperty.DriverLine_Cabin_Color_Value="2";

            var resultCreateProperty = _vehicleRepository.UpdateProperty(ObjVehicleProperty).Result;
            Assert.IsNotNull(resultCreateProperty);
            Assert.IsTrue(resultCreateProperty.ID > 0);

        }
    }
}

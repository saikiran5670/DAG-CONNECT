using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.vehicle.test
{
    [TestClass]
    public class Vehiclerepositorytest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleManager _vehiclemanager;
        private readonly IGroupRepository _groupRepository;
        private readonly IAuditTraillib _auditlog;

        public Vehiclerepositorytest()
        {
            string connectionString = "Server=10.193.124.168;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require; Trust Server Certificate=True;";
            string datamartconnectionString = "Server=10.193.124.165;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require; Trust Server Certificate=True;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            _vehicleRepository = new VehicleRepository(_dataAccess, _datamartDataacess);
            _groupRepository = new GroupRepository(_dataAccess);
            _vehiclemanager = new VehicleManager(_vehicleRepository);

        }
        //[TestCategory("Unit-Test-Case")]
        //[Description("Test for Create vehicle with organization")]
        //[TestMethod]
        //public void CreateVehicleWithOragnization()
        //{
        //    Vehicle Objvehicle = new Vehicle();

        //    Objvehicle.Organization_Id = 1;
        //    Objvehicle.Name = "Vehicle 34";
        //    Objvehicle.VIN = "V8770uuu";
        //    Objvehicle.License_Plate_Number = "REG3264";
        //   // Objvehicle.ManufactureDate = DateTime.Now;
        //   // Objvehicle.ChassisNo = "123545";
        //    Objvehicle.Status_Changed_Date = DateTime.Now;
        ////    Objvehicle.Status = VehicleStatusType.OptIn;
        //    Objvehicle.Termination_Date = DateTime.Now;
        //    Objvehicle.Vid = "F344334";
        //    Objvehicle.Type = VehicleType.SemiTrailer;
        //    //Objvehicle.ModelId = "Model";
        //    Objvehicle.Tcu_Id = "TId234234";
        //    Objvehicle.Tcu_Serial_Number = "S23432490892346";
        //    Objvehicle.Tcu_Brand = "Truck";
        //    Objvehicle.Tcu_Version = "Tv0.1";
        //    Objvehicle.Is_Tcu_Register = true;
        //    Objvehicle.Reference_Date = Convert.ToDateTime("2019-02-02T12:34:56");
        //    var resultvehicle = _vehicleRepository.Create(Objvehicle).Result;
        //    Assert.IsNotNull(resultvehicle);
        //    Assert.IsTrue(resultvehicle.ID > 0);

        //}

        // [TestCategory("Unit-Test-Case")]
        //[Description("Test for Create vehicle without organization")]
        //[TestMethod]
        //public void CreateVehicleWithoutOrganization()
        //{
        //    Vehicle Objvehicle = new Vehicle();

        //    Objvehicle.Organization_Id = null;
        //    Objvehicle.Name = "Vehicle 34";
        //    Objvehicle.VIN = "NJH54855";
        //    Objvehicle.License_Plate_Number = "REG3264";
        //  //  Objvehicle.ManufactureDate = DateTime.Now;
        //  //  Objvehicle.ChassisNo = "123545";
        //    Objvehicle.Status_Changed_Date = DateTime.Now;
        // //   Objvehicle.Status = VehicleStatusType.OptIn;
        //    Objvehicle.Termination_Date = DateTime.Now;
        //    Objvehicle.Vid = "F344334";
        //    Objvehicle.Type = VehicleType.SemiTrailer;
        //    Objvehicle.ModelId = "Model";
        //    Objvehicle.Tcu_Id = "TId234234";
        //    Objvehicle.Tcu_Serial_Number = "S23432490892346";
        //    Objvehicle.Tcu_Brand = "Truck";
        //    Objvehicle.Tcu_Version = "Tv0.1";
        //    Objvehicle.Is_Tcu_Register = true;
        //    Objvehicle.Reference_Date = Convert.ToDateTime("2019-02-02T12:34:56");
        //    var resultvehicle = _vehicleRepository.Create(Objvehicle).Result;
        //    Assert.IsNotNull(resultvehicle);
        //    Assert.IsTrue(resultvehicle.ID > 0);

        //}

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get vehicle details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetVehicle()
        {
            VehicleFilter ObjFilter = new VehicleFilter();
            //ObjFilter.OrganizationId=1;
            //ObjFilter.VehicleId=5;
            //ObjFilter.VIN="H";
            //ObjFilter.VehicleIdList="7,8";
            ObjFilter.Status = VehicleStatusType.OptIn;

            var resultvehicleList = _vehicleRepository.Get(ObjFilter).Result;
            Assert.IsNotNull(resultvehicleList);
            Assert.IsTrue(resultvehicleList.Count() > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update vehicle")]
        [TestMethod]
        public void UpdateVehicle()
        {
            //Vehicle Objvehicle = new Vehicle();
            //Objvehicle.ID = 5;
            //Objvehicle.Name = "Vehicle 5";
            //Objvehicle.License_Plate_Number = "LIC0325147878";
            //var resultUpdatevehicle = _vehicleRepository.Update(Objvehicle).Result;

            VehicleFilter ObjFilter = new VehicleFilter();
            ObjFilter.VIN = "KLRAE75PC0E200144";
            var resultvehicleList = _vehicleRepository.Get(ObjFilter).Result;
            var testVehicle = resultvehicleList.Where(x => x is Vehicle).First();
            var resultUpdatevehicle = _vehicleRepository.Update(testVehicle).Result;

            Assert.IsNotNull(resultUpdatevehicle);
            Assert.IsTrue(resultUpdatevehicle.ID > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update vehicle TCU details")]
        [TestMethod]
        public void UpdateVehicleTCUDetails()
        {
            Vehicle Objvehicle = new Vehicle();
            Objvehicle.VIN = "TLRAS47MS0E808080";
            Objvehicle.Vid = "VID0001";
            Objvehicle.Tcu_Id = "TCU001";
            Objvehicle.Tcu_Serial_Number = "TCUSR001";
            Objvehicle.Tcu_Brand = "123654889";
            Objvehicle.Tcu_Version = "T V0.1";
            Objvehicle.Is_Tcu_Register = false;
            Objvehicle.Reference_Date = Convert.ToDateTime("2019-02-02T12:34:56");
            var resultUpdateTCUvehicle = _vehicleRepository.Update(Objvehicle).Result;
            Assert.IsNotNull(resultUpdateTCUvehicle);
            Assert.IsTrue(resultUpdateTCUvehicle.VIN != null);

        }

        [TestMethod]
        public void UpdateStatus()
        {
            VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();
            ObjvehicleOptInOptOut.RefId = 28;
            ObjvehicleOptInOptOut.AccountId = 4;
            ObjvehicleOptInOptOut.Status = VehicleStatusType.OptOut;
            ObjvehicleOptInOptOut.Type = OptInOptOutType.VehicleLevel;
            ObjvehicleOptInOptOut.Date = DateTime.Now;
            var resultUpdateOptInOptOutvehicle = _vehicleRepository.UpdateStatus(ObjvehicleOptInOptOut).Result;
            Assert.IsNotNull(resultUpdateOptInOptOutvehicle);
            Assert.IsTrue(resultUpdateOptInOptOutvehicle.RefId > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get VehicleBySubscriptionSet ")]
        [TestMethod]
        public async Task UnT_vehicle_VehicleManager_GetVehicleBySubscriptionSet()
        {
            int subscriptionId = 1;
            var results = await _vehicleRepository.GetVehicleBySubscriptionId(subscriptionId);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get VehicleGroup By accountId ")]
        [TestMethod]
        public async Task UnT_vehicle_VehicleManager_GetVehicleGroupbyAccountIdTest()
        {
            int accountId = 125;
            int orgId = 1;
            var results = await _vehiclemanager.GetVehicleGroupbyAccountId(accountId, orgId);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for update vehicle")]
        [TestMethod]
        public async void UpdateVehicleConnection()
        {


            var ObjFilter = new List<VehicleConnect>() {
            new VehicleConnect(){ VehicleId =10,Opt_In='I',ModifiedBy=1 },
            new VehicleConnect(){ VehicleId =253,Opt_In='I',ModifiedBy=1},
            };

            var resultvehicleList = await _vehicleRepository.UpdateVehicleConnection(ObjFilter);


            //  Assert.IsNotNull(resultvehicleList.VehicleConnectedList.Count > 0);
            Assert.IsTrue(resultvehicleList.VehicleConnectedList.Count > 0);

        }

        #region Vehicle Mileage
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Vehicle Mileage Data ")]
        [TestMethod]
        public async Task UnT_vehicle_VehicleManager_GetVehicleMileage()
        {
            long lsince = 1619419546008;
            string sTimezone = "UTC";
            string targetdateformat = "MM/DD/YYYY";
            string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(lsince, sTimezone, targetdateformat);
            string since = converteddatetime;
            bool isnumeric = true;
            string contenttype = "text/csv";
            var results = await _vehiclemanager.GetVehicleMileage(since, isnumeric, contenttype, 1, 1);
            if (contenttype == "text/csv")
            {
                Assert.IsNotNull(results.VehiclesCSV);
                Assert.IsTrue(results.VehiclesCSV != null);
            }
            else
            {
                Assert.IsNotNull(results.Vehicles);
                Assert.IsTrue(results.Vehicles != null);
            }

        }
        #endregion

        //[TestCategory("Unit-Test-Case")]
        //[Description("Test for create vehicle group and vehicle details")]
        //[TestMethod]
        //public void CreateVehicleGroupWithVehicle()
        //{
        //    Group group = new Group();
        //    group.ObjectType = ObjectType.VehicleGroup;
        //    group.GroupType = GroupType.Group;
        //    group.Argument = "Truck UT 01";
        //    group.FunctionEnum = FunctionEnum.None;
        //    group.OrganizationId = 1;
        //    group.RefId = null;
        //    group.Name = "Vehicle Group Unit test 02";
        //    group.Description = "Vehicle Group";
        //    var groupResult = _groupRepository.Create(group).Result;
        //    if (groupResult.Id > 0)
        //    {
        //        // Add vehicles in it
        //        groupResult.GroupRef = new List<GroupRef>();
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 101 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 102 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 103 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 104 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 105 });
        //    }
        //    var result = _groupRepository.UpdateRef(groupResult).Result;
        //    Assert.IsTrue(result);
        //}

        //[TestCategory("Unit-Test-Case")]
        //[Description("Test for update vehicle group and vehicle details")]
        //[TestMethod]
        //public void UpdateVehicleGroupWithVehicle()
        //{
        //    Group group = new Group();
        //    group.Id = 24;
        //    group.ObjectType = ObjectType.VehicleGroup;
        //    group.GroupType = GroupType.Single;
        //    group.Argument = "Truck 10";
        //    group.FunctionEnum = FunctionEnum.All;
        //    group.OrganizationId = 1;
        //    group.RefId = null;
        //    group.Name = "Vehicle Group Unit test02";
        //    group.Description = "Vehicle Group";
        //    var groupResult = _groupRepository.Update(group).Result;
        //    if (groupResult.Id > 0)
        //    {
        //        // Add vehicles in it
        //        groupResult.GroupRef = new List<GroupRef>();
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 10 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 11 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 13 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 14 });
        //        groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 15 });
        //    }
        //    var result = _groupRepository.UpdateRef(groupResult).Result;
        //    Assert.IsTrue(result);
        //}

        //// [TestCategory("Unit-Test-Case")]
        //// [Description("Test for delete vehicle group")]
        //// [TestMethod]
        //// public void DeleteGroup()
        //// {
        ////     var result = _groupRepository.Delete(7).Result;
        ////     Assert.IsTrue(result == true);
        //// }

        //[TestCategory("Unit-Test-Case")]
        //[Description("Test for get organization group details")]
        //[TestMethod]
        //public void GetOrganizationVehicleGroupdetails()
        //{
        //    var result = _vehicleRepository.GetOrganizationVehicleGroupdetails(7).Result;
        //    Assert.IsTrue(result.Count() > 0);
        //}




        //[TestMethod]
        //public void CreateVehicleProperty()
        //{
        //    VehicleProperty ObjVehicleProperty = new VehicleProperty();            
        //    ObjVehicleProperty.ManufactureDate = DateTime.Now;

        //    ObjVehicleProperty.DeliveryDate = DateTime.Now;
        //    ObjVehicleProperty.Classification_Make = "Make 1";
        //    // ObjVehicleProperty.Classification_Model="Model 1";
        //    ObjVehicleProperty.Classification_Series_Id = "Series 1";
        //    //  ObjVehicleProperty.Classification_Type=VehicleType.TRAILER;
        //  //  ObjVehicleProperty.Dimensions_Size_Length = 1;
        //  //  ObjVehicleProperty.Dimensions_Size_Width = 2;
        //  //  ObjVehicleProperty.Dimensions_Size_Height = 3;
        //    //ObjVehicleProperty.Dimensions_Size_Weight = 4;
        //    ObjVehicleProperty.Engine_ID = "1";
        //    ObjVehicleProperty.Engine_Type = "Trucks";
        //  //  ObjVehicleProperty.Engine_Power = 500;
        //    ObjVehicleProperty.Engine_Coolant = "Coolant1";
        //    ObjVehicleProperty.Engine_EmissionLevel = "EURO_III_EEV";
        //    // ObjVehicleProperty.Chasis_Id = "234";
        //    // ObjVehicleProperty.SideSkirts = true;
        //    // ObjVehicleProperty.SideCollars = true;
        //    // ObjVehicleProperty.RearOverhang = 122;
        //    // ObjVehicleProperty.Tank_Nr = 155;
        //    // ObjVehicleProperty.Tank_Volume = 155;
        //    ObjVehicleProperty.DriverLine_AxleConfiguration = "4535";
        //  //  ObjVehicleProperty.DriverLine_Wheelbase = 1;
        //    ObjVehicleProperty.DriverLine_Tire_Size = "1.2";
        //    ObjVehicleProperty.DriverLine_Tire_Size = "1.2";
        //    // ObjVehicleProperty.DriverLine_FrontAxle_Position = 1;
        //    // ObjVehicleProperty.DriverLine_FrontAxle_Load = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Position = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Load = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Ratio = 2;
        //    ObjVehicleProperty.GearBox_Id = "2";
        //    ObjVehicleProperty.GearBox_Type = "GrearBox1";
        //    ObjVehicleProperty.DriverLine_Cabin_ID = "2";
        //    //ObjVehicleProperty.DriverLine_Cabin_Color_Value = "2";

        //    var resultCreateProperty = _vehicleRepository.UpdateProperty(ObjVehicleProperty).Result;
        //    Assert.IsNotNull(resultCreateProperty);
        //    Assert.IsTrue(resultCreateProperty.ID > 0);

        //}

        //[TestMethod]
        //public void UpdateVehicleProperty()
        //{
        //    VehicleProperty ObjVehicleProperty = new VehicleProperty();
        //    //ObjVehicleProperty.VehicleId = 5;
        //    ObjVehicleProperty.ManufactureDate = DateTime.Now;
        //    //ObjVehicleProperty.RegistrationDateTime = DateTime.Now;
        //    ObjVehicleProperty.DeliveryDate = DateTime.Now;
        //    ObjVehicleProperty.Classification_Make = "Make 1";
        //    //    ObjVehicleProperty.Classification_Model="Model 1";
        //    //ObjVehicleProperty.Classification_Series = "Series 1";
        //    //    ObjVehicleProperty.Classification_Type=VehicleType.TRAILER;
        // //   ObjVehicleProperty.Dimensions_Size_Length = 1;
        // //   ObjVehicleProperty.Dimensions_Size_Width = 2;
        //  //  ObjVehicleProperty.Dimensions_Size_Height = 3;
        //    //ObjVehicleProperty.Dimensions_Size_Weight = 4;
        //    ObjVehicleProperty.Engine_ID = "1";
        //    ObjVehicleProperty.Engine_Type = "Trucks";
        //   // ObjVehicleProperty.Engine_Power = 500;
        //    ObjVehicleProperty.Engine_Coolant = "Coolant1";
        //    ObjVehicleProperty.Engine_EmissionLevel = "EURO_III";
        //    // ObjVehicleProperty.Chasis_Id = "234";
        //    // ObjVehicleProperty.SideSkirts = true;
        //    // ObjVehicleProperty.SideCollars = true;
        //    // ObjVehicleProperty.RearOverhang = 122;
        //    // ObjVehicleProperty.Tank_Nr = 155;
        //    // ObjVehicleProperty.Tank_Volume = 155;
        //    ObjVehicleProperty.DriverLine_AxleConfiguration = "4535";
        //   // ObjVehicleProperty.DriverLine_Wheelbase = 1;
        //    ObjVehicleProperty.DriverLine_Tire_Size = "1.2";
        //    ObjVehicleProperty.DriverLine_Tire_Size = "1.2";
        //    // ObjVehicleProperty.DriverLine_FrontAxle_Position = 1;
        //    // ObjVehicleProperty.DriverLine_FrontAxle_Load = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Position = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Load = 2;
        //    // ObjVehicleProperty.DriverLine_RearAxle_Ratio = 2;
        //    ObjVehicleProperty.GearBox_Id = "2";
        //    ObjVehicleProperty.GearBox_Type = "GrearBox2";
        //    ObjVehicleProperty.DriverLine_Cabin_ID = "2";
        //    //ObjVehicleProperty.DriverLine_Cabin_Color_Value = "2";

        //    var resultCreateProperty = _vehicleRepository.UpdateProperty(ObjVehicleProperty).Result;
        //    Assert.IsNotNull(resultCreateProperty);
        //    Assert.IsTrue(resultCreateProperty.ID > 0);

        //}

        // [TestMethod]
        // public void UpdateVehicleforCRM()
        // {          
        //     string vin="V22";
        //     string tcuId="V22UpdatedManager";
        //     string tcuactivation="true";
        //     string referenceDateTime="04-04-2019";
        //     var resultUpdatevehicle = _vehicleRepository.update(vin,tcuId,tcuactivation,referenceDateTime).Result;
        //     Assert.IsNotNull(resultUpdatevehicle);
        //   //  Assert.IsTrue(resultUpdatevehicle.ID > 0);

        // }

        //  [TestMethod]
        // public void CreateVehicleforCRM()
        // {          
        //     int orgid=10;
        //     string vin="V2265555";
        //     string tcuId="tesdddddd";
        //     string tcuactivation="true";
        //     string referenceDateTime="1610877108";

        //     var resultUpdatevehicle = _vehicleRepository.create(orgid,vin,tcuId,tcuactivation,referenceDateTime).Result;
        //     Assert.IsNotNull(resultUpdatevehicle);
        //   //  Assert.IsTrue(resultUpdatevehicle.ID > 0);

        // }





    }
}

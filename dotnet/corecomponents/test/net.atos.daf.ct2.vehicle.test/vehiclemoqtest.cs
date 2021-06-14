using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.vehicle.test
{
    [TestClass]
    public class vehiclemoqtest
    {
        public class VehicleManagerMoqTest
    {
        Mock<IVehicleRepository> _iVehicleRepository;
        Mock<IAuditTraillib>  _iauditlog;

        VehicleManager _vehicleManager;
        public VehicleManagerMoqTest()
        {
            _iVehicleRepository = new Mock<IVehicleRepository>();
            _iauditlog = new Mock<IAuditTraillib>();
            _vehicleManager = new VehicleManager(_iVehicleRepository.Object, _iauditlog.Object);
        }  

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetTest()
        {            
            VehicleFilter vehiclefilter = new VehicleFilter();
            vehiclefilter.OrganizationId = 23;
           //vehiclefilter.OrganizationId=1;
                vehiclefilter.VehicleId=5;
            //vehiclefilter.VIN="H";
            //vehiclefilter.VehicleIdList="7,8";
            vehiclefilter.Status = VehicleStatusType.OptIn;
            var actual =new List<Vehicle>();
            _iVehicleRepository.Setup(s=>s.Get(It.IsAny<VehicleFilter>())).ReturnsAsync(actual);
            var result = await _vehicleManager.Get(vehiclefilter);
            Assert.AreEqual(result, actual);
        }     

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateTest()
        {            
            Vehicle vehicle = new Vehicle();
            vehicle.ID = 5;
            vehicle.Name = "Vehicle 5";
            vehicle.License_Plate_Number = "LIC0325147878";
            Vehicle actual = new Vehicle();
            _iVehicleRepository.Setup(s=>s.Update(It.IsAny<Vehicle>())).ReturnsAsync(actual);
            var result = await _vehicleManager.Update(vehicle);
            Assert.AreEqual(result, actual);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateStatus")]
        [TestMethod]
        public async Task UpdateStatusTest()
        {            
            VehicleOptInOptOut vehicleOptInOptOut = new VehicleOptInOptOut();
           vehicleOptInOptOut.RefId = 28;
            vehicleOptInOptOut.AccountId = 4;
            vehicleOptInOptOut.Status = VehicleStatusType.OptOut;
            vehicleOptInOptOut.Type = OptInOptOutType.VehicleLevel;
            vehicleOptInOptOut.Date = DateTime.Now;
            VehicleOptInOptOut actual = new VehicleOptInOptOut();
            _iVehicleRepository.Setup(s=>s.UpdateStatus(It.IsAny<VehicleOptInOptOut>())).ReturnsAsync(actual);
            var result = await _vehicleManager.UpdateStatus(vehicleOptInOptOut);
            Assert.AreEqual(result, actual);
        } 
        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVehicleBySubscriptionId")]
        [TestMethod]
        public async Task GetVehicleBySubscriptionIdTest()
        {            
           int subscriptionId =23;
            List<VehiclesBySubscriptionId> actual =new List<VehiclesBySubscriptionId>();
            _iVehicleRepository.Setup(s=>s.GetVehicleBySubscriptionId(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _vehicleManager.GetVehicleBySubscriptionId(subscriptionId);
            Assert.AreEqual(result, actual);
        }     

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVehicleGroupbyAccountId")]
        [TestMethod]
        public async Task GetVehicleGroupbyAccountIdTest()
        {            
           int accountid = 23;
            int orgnizationid = 56;
            var actual =new List<VehicleGroupList>();
            _iVehicleRepository.Setup(s=>s.GetVehicleGroupbyAccountId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _vehicleManager.GetVehicleGroupbyAccountId(accountid, orgnizationid);
            Assert.AreEqual(result, actual);
        }    

        // public async Task GetVehicleMileageTest()
        //  {            
          //string since = "rfgfg";
           //bool isnumeric = true;
           //string contentType = "dhdhdh";
           // int accountId = 23;
           // int orgid = 54;
          // long startDate = 1619419546008;
          // long endDate = 1819419546008;
          //  bool noFilter = true;

           // var actual =new List<DtoVehicleMileage>();
           // _iVehicleRepository.Setup(s=>s.GetVehicleMileage(It.IsAny<long>(),(It.IsAny<long>(), It.IsAny<bool>())).ReturnsAsync(actual));
           // var result = await _vehicleManager.GetVehicleMileage(startDate, endDate, noFilter );
          //  Assert.AreEqual(result, actual);
       // } // 


        
    }

    }
}

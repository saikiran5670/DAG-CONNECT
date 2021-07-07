using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.test
{
    public partial class ReportManagerTest
    {
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVehicleHealthStatus_Test()
        {
            var vehicleHealthStatusRequest = new VehicleHealthStatusRequest()
            {
                VIN = "XLR0998HGFFT76657",
                //TripId = "52a0f631-4077-42f9-b999-cb21c6309c71",
                Days = 90
            };
            var result = await _reportRepository.GetVehicleHealthStatus(vehicleHealthStatusRequest);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetFleetVehicleDetails_Test()
        {
            var fleetOverviewFilter = new FleetOverviewFilter();// { VINIds = { "XLRAS47MS0E808080" }, Days = 7 };
            List<string> objvin = new List<string>();
            objvin.Add("XLR0998HGFFT76657");
            fleetOverviewFilter.VINIds = objvin;
            List<string> objdriver = new List<string>();
            objdriver.Add("SK 2236526558846039");
            fleetOverviewFilter.DriverId = objdriver;
            List<string> objhealth = new List<string>();
            objhealth.Add("S");
            fleetOverviewFilter.HealthStatus = objhealth;
            //var fleetOverviewFilter = new FleetOverviewFilter() { GroupId = { " " }, VINIds = { "XLRAS47MS0E808080" }, AlertLevel = { " " }, AlertCategory = { " " }, HealthStatus = { " " }, OtherFilter = { " " }, DriverId = { "SK 0000000012340437" }, Days = 7 };
            //var fleetOverviewFilter = new FleetOverviewFilter();
            var result = await _reportRepository.GetFleetOverviewDetails(fleetOverviewFilter);
            Assert.IsNotNull(result);
        }
    }
}

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
            var vehicleHealthStatusRequest = new VehicleHealthStatusRequest() { VIN = "XLRAS47MS0E808080" };
            var result = await _reportRepository.GetVehicleHealthStatus(vehicleHealthStatusRequest);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetCurrentWarnning_Test()
        {
            var vehicleHealthStatusRequest = new VehicleHealthStatusRequest() { VIN = "XLRAS47MS0E808080" };
            var result = await _reportRepository.GetCurrentWarnning("XLRAS47MS0E808080");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetHistoryWarnning_Test()
        {
            var vehicleHealthStatusRequest = new VehicleHealthStatusRequest() { VIN = "XLRAS47MS0E808080" };
            var result = await _reportRepository.GetHistoryWarnning("XLRAS47MS0E808080");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetFleetVehicleDetails_Test()
        {
            //var fleetOverviewFilter = new FleetOverviewFilter() { GroupId = " ", AlertLevel = " ", AlertCategory = " ", HealthStatus = " ", OtherFilter = " ", DriverId = "SK 0000000012340437", Days = "7" };
            var fleetOverviewFilter = new FleetOverviewFilter();
            var result = await _reportRepository.GetFleetOverviewDetails(fleetOverviewFilter);
            Assert.IsNotNull(result);
        }
    }
}

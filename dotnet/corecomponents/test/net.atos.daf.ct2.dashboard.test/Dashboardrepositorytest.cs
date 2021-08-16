using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.dashboard;
using net.atos.daf.ct2.dashboard.repository;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.subscription.test
{
    [TestClass]
    public class Dashboardrepositorytest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDashBoardManager _iDashBoardManager;
        private readonly IDataMartDataAccess _iDataMartDataAccess;
        private readonly IConfiguration _config;
        private readonly DashBoardRepository _dashBoardRepository;
        public Dashboardrepositorytest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
            var connectionString = _config.GetConnectionString("connectionString");
            var dataMartConnectionString = _config.GetConnectionString("datamartconnectionString");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _iDataMartDataAccess = new PgSQLDataMartDataAccess(dataMartConnectionString);
            _dashBoardRepository = new DashBoardRepository(_dataAccess, _iDataMartDataAccess);
            _iDashBoardManager = new DashBoardManager(_dashBoardRepository);
        }
        /// <summary>
        /// Need to Return Data for requested vins b/w time span today 00:00 to current time
        /// </summary>
        /// <returns>Vehicle Data</returns>
        [TestCategory("Unit-Test-Case")]
        [Description("Test For Today Vehicle Live Vin Data")]
        [TestMethod]
        public async Task GetTodayLiveVinData()
        {
            TodayLiveVehicleRequest objTodayLiveVehicleRequest = new TodayLiveVehicleRequest();
            objTodayLiveVehicleRequest.VINs = new List<string>();
            objTodayLiveVehicleRequest.VINs.Add("M4A14532");
            var results = await _iDashBoardManager.GetTodayLiveVinData(objTodayLiveVehicleRequest);
            Assert.IsNotNull(results);
            Assert.IsTrue(results != null);
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task UnT_Report_GetAlertLast24Hours_Test()
        {
            var alertLast24Hours = new Alert24HoursFilter()
            {
                VINs = new List<string>() { "XLR0998HGFFT76657" },

            };
            var result = await _iDashBoardManager.GetLastAlert24Hours(alertLast24Hours);
            Assert.IsNotNull(result);
        }
    }
}

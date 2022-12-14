using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;

namespace net.atos.daf.ct2.notificationengine.test
{
    [TestClass]
    public class NotificationEngineManager
    {
        private readonly IConfiguration _config = null;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _datamartDataacess;
        private readonly INotificationIdentifierManager _inotificationIdentifierManager;
        private readonly NotificationIdentifierRepository _notificationIdentifierRepository;
        public NotificationEngineManager()
        {
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            string datamartconnectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-datamart.postgres.database.azure.com;Database=vehicledatamart;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-datamart;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _config = new ConfigurationBuilder().Build();
            _dataAccess = new PgSQLDataAccess(connectionString);
            _datamartDataacess = new PgSQLDataMartDataAccess(datamartconnectionString);
            _notificationIdentifierRepository = new NotificationIdentifierRepository(_dataAccess, _datamartDataacess);
            _inotificationIdentifierManager = new NotificationIdentifierManager(_notificationIdentifierRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Send alert trip data")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void SendAlertTripData()
        {
            TripAlert tripAlert = new TripAlert();
            tripAlert.Id = 1;
            tripAlert.Tripid = "a801403e-ae4c-42cf-bf2d-ae39009c69oi";
            tripAlert.Vin = "XLR0998HGFFT76657";
            tripAlert.CategoryType = "L";
            tripAlert.Type = "G";
            tripAlert.Alertid = 328;
            tripAlert.Latitude = 51.12768896;
            tripAlert.Longitude = 4.935644520;
            tripAlert.AlertGeneratedTime = 1626965785;
            tripAlert.ThresholdValue = 8766;
            tripAlert.ValueAtAlertTime = 8767;
            tripAlert.ThresholdValueUnitType = "M";


            var notificationDetails = _inotificationIdentifierManager.GetNotificationDetails(tripAlert).Result;
            Assert.IsNotNull(notificationDetails.Count > 0);
        }

    }
}

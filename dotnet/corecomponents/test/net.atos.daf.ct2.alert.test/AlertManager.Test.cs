using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.alert.repository;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.test
{
    [TestClass]
    public class AlertManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly AlertRepository _alertRepository;
        private readonly IAlertManager _ialertManager;
        public AlertManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _alertRepository = new AlertRepository(_dataAccess);
            _ialertManager = new AlertManager(_alertRepository);
        }
    }
}

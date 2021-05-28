using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.visibility;
using net.atos.daf.ct2.visibility.repository;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.Audittrail.test
{
    [TestClass]
    public class Visibilitytest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly IVisibilityRepository _visibilityRepository;
        private readonly IVisibilityManager _visibilityManager;
        public Visibilitytest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                      .Build();
            var connectionString = _config.GetConnectionString("DevAzure");
                        
            _dataAccess = new PgSQLDataAccess(connectionString);
            _visibilityRepository = new VisibilityRepository(_dataAccess);
            _visibilityManager = new VisibilityManager(_visibilityRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVehicleByAccountVisibility Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVehicleByAccountVisibility_Success()
        {

            var result = await _visibilityManager
                                    .GetVehicleByAccountVisibility(51,93);
            Assert.IsTrue(result.Count() >0);
        }
    }
}

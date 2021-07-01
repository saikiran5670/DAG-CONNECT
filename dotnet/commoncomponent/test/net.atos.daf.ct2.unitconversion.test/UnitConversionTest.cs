using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.unitconversion.ENUM;

namespace net.atos.daf.ct2.unitconversion.test
{
    [TestClass]
    public class UnitConversionTest
    {
        private readonly IUnitConversionManager _unitConversionManager;
        public UnitConversionTest()
        {
            _unitConversionManager = new UnitConversionManager();
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetDistance Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVehicleByAccountVisibility_Success()
        {

            var result = await _unitConversionManager
                                   .GetDistance(1234.45454
                                   , DistanceUnit.Meter
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 1234454.54);
        }
    }
}

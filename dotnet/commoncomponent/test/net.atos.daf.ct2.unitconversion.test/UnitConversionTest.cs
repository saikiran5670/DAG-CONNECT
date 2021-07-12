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
        public async Task GetDistance_Success()
        {

            var result = await _unitConversionManager
                                   .GetDistance(123.58
                                   , DistanceUnit.Meter
                                   , UnitToConvert.Metric);
            Assert.IsTrue(result == 0.12);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVolume Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVolume_Success()
        {

            var result = await _unitConversionManager
                                   .GetVolume(123123.58
                                   , VolumeUnit.MilliLiter
                                   , UnitToConvert.Metric);
            Assert.IsTrue(result == 0.12);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetTimeSpan Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetTimeSpan_Success()
        {
            var result = await _unitConversionManager
                                   .GetTimeSpan(3540
                                   , TimeUnit.Seconds
                                   , UnitToConvert.Metric);
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetSpeed Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetSpeed_Success()
        {
            var result = await _unitConversionManager
                                   .GetSpeed(123.58
                                   , SpeedUnit.MeterPerMilliSec
                                   , UnitToConvert.Metric);
            Assert.IsTrue(result == 444888);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetWeight Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetWeight_Success()
        {
            var result = await _unitConversionManager
                                   .GetWeight(123.58
                                   , WeightUnit.KiloGram
                                   , UnitToConvert.Metric);
            Assert.IsTrue(result == 123.58);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVolumePerDistance Sucess case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVolumePerDistance_Success()
        {
            var result = await _unitConversionManager
                                   .GetVolumePerDistance(123.58
                                   , VolumePerDistanceUnit.MilliLiterPerMeter
                                   , UnitToConvert.Metric);
            Assert.IsTrue(result == 123.58);
        }



        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetDistance Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetDistance_Imperial_Success()
        {

            var result = await _unitConversionManager
                                   .GetDistance(123.58
                                   , DistanceUnit.Meter
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 0.08);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVolume Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVolume_Imperial_Success()
        {

            var result = await _unitConversionManager
                                   .GetVolume(123123.58
                                   , VolumeUnit.MilliLiter
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 27.08);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetTimeSpan Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetTimeSpan_Imperial_Success()
        {
            var result = await _unitConversionManager
                                   .GetTimeSpan(3540
                                   , TimeUnit.Seconds
                                   , UnitToConvert.Imperial);
            Assert.IsNotNull(result);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetSpeed Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetSpeed_Imperial_Success()
        {
            var result = await _unitConversionManager
                                   .GetSpeed(123.58
                                   , SpeedUnit.MeterPerMilliSec
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 276441.05);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetWeight Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetWeight_Imperial_Success()
        {
            var result = await _unitConversionManager
                                   .GetWeight(123.58
                                   , WeightUnit.KiloGram
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 272.49);
        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test for GetVolumePerDistance Imperial Success case")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public async Task GetVolumePerDistance_Imperial_Success()
        {
            var result = await _unitConversionManager
                                   .GetVolumePerDistance(123.58
                                   , VolumePerDistanceUnit.MilliLiterPerMeter
                                   , UnitToConvert.Imperial);
            Assert.IsTrue(result == 52.54);
        }
    }
}

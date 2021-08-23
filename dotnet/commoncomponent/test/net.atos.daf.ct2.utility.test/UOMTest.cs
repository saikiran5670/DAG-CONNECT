using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.utility.test
{
    [TestClass]
    public class UOMTest
    {
        [TestCategory("Unit-Test-Case-01")]
        [Description("Test for converted threshold value into hours")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetExpectedValueHoursTest()
        {
            string expectedUnitType = "H";
            double expectedValue = UOMHandling.GetConvertedThresholdValue(7200, expectedUnitType);
            Assert.IsTrue(expectedValue == 2);
            Assert.IsNotNull(expectedValue);
        }

        [TestCategory("Unit-Test-Case-02")]
        [Description("Test for converted threshold value into KM")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetExpectedValueKMTest()
        {
            string expectedUnitType = "K";
            double expectedValue = UOMHandling.GetConvertedThresholdValue(100, expectedUnitType);
            Assert.IsTrue(expectedValue == Convert.ToDouble(0.1));
            Assert.IsNotNull(expectedValue);
        }

        [TestCategory("Unit-Test-Case-03")]
        [Description("Test for percentage value")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetExpectedValuePercentageTest()
        {
            string expectedUnitType = "P";
            double expectedValue = UOMHandling.GetConvertedThresholdValue(10, expectedUnitType);
            Assert.IsTrue(expectedValue == 10);
            Assert.IsNotNull(expectedValue);
        }

        [TestCategory("Unit-Test-Case-04")]
        [Description("Test for percentage value")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetExpectedValueFeet()
        {
            string expectedUnitType = "F";
            double expectedValue = UOMHandling.GetConvertedThresholdValue(12500, expectedUnitType);
            Assert.IsTrue(expectedValue == Convert.ToDouble(41012.500));
            Assert.IsNotNull(expectedValue);
        }
    }
}

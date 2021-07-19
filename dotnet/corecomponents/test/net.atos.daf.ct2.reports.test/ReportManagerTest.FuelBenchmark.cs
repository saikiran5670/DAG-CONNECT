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
        public async Task UnT_Report_GetFuelBenchmarkDetails_Test()
        {
            var fuelBenchmarkFilterRequest = new FuelBenchmarkFilter()
            {
                VINs = new List<string>() { "XLR0998HGFFT76657" },
                StartDateTime = 1623325980000,
                EndDateTime = 1623330444000
            };
            var result = await _reportManager.GetFuelBenchmarkDetails(fuelBenchmarkFilterRequest);
            Assert.IsNotNull(result);
        }
    }
}

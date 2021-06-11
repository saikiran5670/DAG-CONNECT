using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.utilities;
namespace net.atos.daf.ct2.utility.test
{
    [TestClass]
    public class UTCHandlingTest
    {

        // [TestMethod]
        // public void GetConvertedDateTimeTest()
        // {           
        //     DateTime utcDate=Convert.ToDateTime("2020/10/12 15:24");
        //     string sTimezone="New Zealand Standard Time";
        //    // DateTime expectedValue = "";
        //     UTCHandling objUtcHandling=new UTCHandling();
        //     DateTime dt =objUtcHandling.GetConvertedDateTime(utcDate,sTimezone);
        //     Assert.IsNotNull(dt); 
        // }

        [TestMethod]
        public void GetConvertedDateTimeTest()
        {
            long utctime = 1608706686000;
            string sTimezone = "New Zealand Standard Time";
            string targetdateformat = "MM/DD/YYYY";
            string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(utctime, sTimezone, targetdateformat);
            Assert.IsNotNull(converteddatetime);
        }

        [TestMethod]
        public void GetUTCFromDateTimestringTest()
        {
            string sourcedatetime = "12/23/2020";
            long utctime = UTCHandling.GetUTCFromDateTime(sourcedatetime);
            Assert.IsNotNull(utctime);
        }

        [TestMethod]
        public void GetConvertedDateTimeobjectTest()
        {
            DateTime sourcedatetime = Convert.ToDateTime("12/23/2020");
            long utctime = UTCHandling.GetUTCFromDateTime(sourcedatetime);
            Assert.IsNotNull(utctime);
        }
    }

}

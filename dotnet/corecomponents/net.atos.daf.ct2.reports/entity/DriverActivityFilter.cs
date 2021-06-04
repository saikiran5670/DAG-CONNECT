using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class DriverActivityFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VIN { get; set; }
        public List<string> DriverId { get; set; }
    }
    public class Driver
    {
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VIN { get; set; }
        public long ActivityDateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class DriversActivities
    {
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string VIN { get; set; }
        public long ActivityDate { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public int Code { get; set; }
        public long RestTime { get; set; }
        public long AvailableTime { get; set; }
        public long WorkTime { get; set; }
        public long DriveTime { get; set; }
        public long ServiceTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota14
{
    public class ScheduleStatusOverview
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public long BoashTimestamp { get; set; }
        public string SecurityResponseFileId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public DateTime LastChangedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ScheduleDateTime { get; set; }
    }
    public class ScheduleSoftwareUpdateResponse
    {
        public int HttpStatusCode { get; set; }
        //public ScheduleStatusOverview ScheduleStatusOverview { get; set; }
        public long BoashTimesStamp { get; internal set; }
    }
}

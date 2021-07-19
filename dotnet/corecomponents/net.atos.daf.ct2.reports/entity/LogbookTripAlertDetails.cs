using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class LogbookTripAlertDetails
    {
        public string Id { get; set; }
        public string Vin { get; set; }
        public string TripId { get; set; }
        public long AlertGeneratedTime { get; set; }
        public string AlertCategoryType { get; set; }
        public string AlertType { get; set; }
        public string AlertLevel { get; set; }

    }



}

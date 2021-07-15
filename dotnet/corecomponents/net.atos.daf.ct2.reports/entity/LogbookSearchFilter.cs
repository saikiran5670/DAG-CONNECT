using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class LogbookSearchFilter
    {
        public string Vin { get; set; }
        public string TripId { get; set; }
        public string AlertId { get; set; }
        public long AlertGeneratedTime { get; set; }
        public string AlertCategoryType { get; set; }
        public string AlertType { get; set; }
        public string AlertName { get; set; }
        public string AlertLevel { get; set; }
        public int Days { get; set; }
        public string LanguageCode { get; set; }
    }


    public class LogbookSearchParameter
    {

        public List<string> GroupId { get; set; }
        public List<string> VINIds { get; set; }
        public List<string> AlertLevel { get; set; }
        public List<string> AlertCategory { get; set; }
        public List<string> AlertType { get; set; }


    }
}

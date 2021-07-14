﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetOverviewAlert
    {
        public int Id { get; set; }
        public string AlertName { get; set; }
        public string AlertType { get; set; }
        public string AlertLocation { get; set; }
        public string AlertTime { get; set; }
        public string AlertLevel { get; set; }
        public string CategoryType { get; set; }
        public string AlertLatitude { get; set; }
        public string AlertLongitude { get; set; }
    }
}

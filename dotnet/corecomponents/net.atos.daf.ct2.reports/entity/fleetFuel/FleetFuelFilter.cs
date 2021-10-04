﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetFuelFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
        public string LanguageCode { get; set; } = "EN-GB";

    }
    public class FleetFuelFilterDriver
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public string VIN { get; set; }
        public string DriverId { get; set; }
        public string LanguageCode { get; set; } = "EN-GB";
    }
}

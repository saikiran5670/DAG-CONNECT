using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity.fleetFuel
{
    public class FleetFuelFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VIN { get; set; }

    }
}

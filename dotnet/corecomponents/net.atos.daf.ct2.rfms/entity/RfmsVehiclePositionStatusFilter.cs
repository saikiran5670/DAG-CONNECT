using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.rfms.entity
{
    public class RfmsVehiclePositionStatusFilter
    {
        public string StartTime { get; set; }

        public string StopTime { get; set; }

        public string Vin { get; set; }

        public Boolean LatestOnly { get; set; }

        public string TriggerFilter { get; set; }

        public string LastVin { get; set; }
        public string Type { get; set; }

    }
}

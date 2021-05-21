using System;

namespace net.atos.daf.ct2.rfms.entity
{
    public class RfmsVehiclePositionRequest
    {
        public string RequestId{ get; set; }
        // enum

        public string StartTime { get; set; }

        public string StopTime { get; set; }

        public string Vin { get; set; }       

        public Boolean LatestOnly { get; set; }

        public string TriggerFilter { get; set; }

    }
}

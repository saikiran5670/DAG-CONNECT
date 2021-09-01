using System;

namespace net.atos.daf.ct2.rfms.entity
{
    public class RfmsVehicleStatusRequest : RfmsBaseRequest
    {

        public RfmsVehiclePositionStatusFilter RfmsVehicleStatusFilter { get; set; }

        public string ContentFilter { get; set; }
    }
}

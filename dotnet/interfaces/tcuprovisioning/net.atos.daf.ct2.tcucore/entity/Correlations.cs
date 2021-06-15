using System;

namespace net.atos.daf.ct2.tcucore
{
    public class Correlations
    {
        public string DeviceId { get; }
        public string VehicleId { get; }
        public Correlations(string deviceId, string vehicleId)
        {
            this.DeviceId = deviceId;
            this.VehicleId = vehicleId;
        }


    }
}

using System;

namespace TCUReceive
{
    public class Correlations
    {
        public Correlations(string deviceId, string vehicleId)
        {
            DeviceId = deviceId;
            VehicleId = vehicleId;
        }

        public string DeviceId { get;  }

        public string VehicleId { get;}
    }
}

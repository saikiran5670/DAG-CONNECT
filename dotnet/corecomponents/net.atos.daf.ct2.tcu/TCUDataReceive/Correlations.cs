using System;

namespace TCUReceive
{
    public class Correlations
    {
        private String deviceId;
        private String vehicleId;

        public Correlations(string deviceId, string vehicleId)
        {
            this.deviceId = deviceId;
            this.vehicleId = vehicleId;
        }

        public string DeviceId { get => deviceId; set => deviceId = value; }

        public string VehicleId { get => vehicleId; set => vehicleId = value; }
    }
}

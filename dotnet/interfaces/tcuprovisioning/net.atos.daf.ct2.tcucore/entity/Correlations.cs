using System;

namespace net.atos.daf.ct2.tcucore
{
    public class Correlations
    {
        private String deviceId;
        private String vehicleId;

        public Correlations(string _deviceId, string _vehicleId)
        {
            this.deviceId = _deviceId;
            this.vehicleId = _vehicleId;
        }

        public string DeviceId
        {
            get => deviceId;
            set => deviceId = value;
        }

        public string VehicleId
        {
            get => vehicleId;
            set => vehicleId = value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    class VehicleChangeEvent
    {
        private String vin;
        private String deviceID;

        public VehicleChangeEvent(string vin, string deviceID)
        {
            this.vin = vin;
            this.deviceID = deviceID;
        }

        public string VIN { get => vin; set => vin = value; }
        public string DeviceID { get => deviceID; set => deviceID = value; }
    }
}

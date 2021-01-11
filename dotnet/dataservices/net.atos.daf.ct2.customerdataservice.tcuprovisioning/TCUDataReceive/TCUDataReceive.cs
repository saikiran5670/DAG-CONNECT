using System;
using System.Collections.Generic;
using System.Text;

namespace TCUReceive
{
    class TCUDataReceive
    {
        private String vin;
        private String deviceIdentifier;
        private String deviceSerialNumber;
        private Correlations correlations;

        public TCUDataReceive(string vin, string deviceIdentifier, string deviceSerialNumber, Correlations correlations)
        {
            this.vin = vin;
            this.deviceIdentifier = deviceIdentifier;
            this.deviceSerialNumber = deviceSerialNumber;
            this.correlations = correlations;
        }

        public string Vin { get => vin; set => vin = value; }

        public string DeviceIdentifier { get => deviceIdentifier; set => deviceIdentifier = value; }

        public string DeviceSerialNumber { get => deviceSerialNumber; set => deviceSerialNumber = value; }

        public Correlations Correlations { get => correlations; set => correlations = value; }
    }
}

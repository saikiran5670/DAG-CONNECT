using System;

namespace TCUReceive
{
    public class TCUDataReceive
    {
        private String vin;
        private String deviceIdentifier;
        private String deviceSerialNumber;
        private Correlations correlations;
        private DateTime referenceDate;

        public TCUDataReceive(string vin, string deviceIdentifier, string deviceSerialNumber, Correlations correlations, DateTime referenceDate)
        {
            this.vin = vin;
            this.deviceIdentifier = deviceIdentifier;
            this.deviceSerialNumber = deviceSerialNumber;
            this.correlations = correlations;
            this.referenceDate = referenceDate;
        }

        public string Vin { get => vin; set => vin = value; }

        public string DeviceIdentifier { get => deviceIdentifier; set => deviceIdentifier = value; }

        public string DeviceSerialNumber { get => deviceSerialNumber; set => deviceSerialNumber = value; }

        public Correlations Correlations { get => correlations; set => correlations = value; }
        public DateTime ReferenceDate { get => referenceDate; set => referenceDate = value; }
    }
}

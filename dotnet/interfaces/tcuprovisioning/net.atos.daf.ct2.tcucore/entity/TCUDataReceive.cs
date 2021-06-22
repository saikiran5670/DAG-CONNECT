using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCUDataReceive
    {
        public TCUDataReceive(string vin
            , string deviceIdentifier
            , string deviceSerialNumber
            , Correlations correlations
            , DateTime referenceDate)
        {
            this.Vin = vin;
            this.DeviceIdentifier = deviceIdentifier;
            this.DeviceSerialNumber = deviceSerialNumber;
            this.Correlations = correlations;
            this.ReferenceDate = referenceDate;
        }

        public string Vin { get; }

        public string DeviceIdentifier { get; }

        public string DeviceSerialNumber { get; }

        public Correlations Correlations { get; }

        public DateTime ReferenceDate { get; }
    }
}

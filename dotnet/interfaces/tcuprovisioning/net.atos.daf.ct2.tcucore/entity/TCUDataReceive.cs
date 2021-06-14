using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCUDataReceive
    {
        public TCUDataReceive(string _vin
            , string _deviceIdentifier
            , string _deviceSerialNumber
            , Correlations _correlations
            , DateTime _referenceDate)
        {
            this.Vin = _vin;
            this.DeviceIdentifier = _deviceIdentifier;
            this.DeviceSerialNumber = _deviceSerialNumber;
            this.Correlations = _correlations;
            this.ReferenceDate = _referenceDate;
        }

        public string Vin { get; }

        public string DeviceIdentifier { get; }

        public string DeviceSerialNumber { get; }

        public Correlations Correlations { get; }

        public DateTime ReferenceDate { get; }
    }
}

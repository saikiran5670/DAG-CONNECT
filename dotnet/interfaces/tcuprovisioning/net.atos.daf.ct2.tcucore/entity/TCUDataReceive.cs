using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCUDataReceive
    {
        private String vin;
        private String deviceIdentifier;
        private String deviceSerialNumber;
        private Correlations correlations;
        private DateTime referenceDate;

        public TCUDataReceive(string _vin
            , string _deviceIdentifier
            , string _deviceSerialNumber
            , Correlations _correlations
            , DateTime _referenceDate)
        {
            this.vin = _vin;
            this.deviceIdentifier = _deviceIdentifier;
            this.deviceSerialNumber = _deviceSerialNumber;
            this.correlations = _correlations;
            this.referenceDate = _referenceDate;
        }

        public string Vin
        {
            get => vin;
            set => vin = value;
        }

        public string DeviceIdentifier
        {
            get => deviceIdentifier;
            set => deviceIdentifier = value;
        }

        public string DeviceSerialNumber
        {
            get => deviceSerialNumber;
            set => deviceSerialNumber = value;
        }

        public Correlations Correlations
        {
            get => correlations;
            set => correlations = value;
        }

        public DateTime ReferenceDate
        {
            get => referenceDate;
            set => referenceDate = value;
        }
    }
}

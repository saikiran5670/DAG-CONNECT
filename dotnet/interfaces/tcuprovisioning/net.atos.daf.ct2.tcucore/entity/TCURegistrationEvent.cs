using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvent
    {
        private String vin;
        private TCU tcu;
        private DateTime referenceDate;

        public TCURegistrationEvent(string _vin, TCU _tcu, DateTime _referenceDate)
        {
            this.vin = _vin;
            this.tcu = _tcu;
            this.referenceDate = _referenceDate;
        }

        public string VIN
        {
            get => vin;
            set => vin = value;
        }

        public TCU TCU
        {
            get => tcu;
            set => tcu = value;
        }

        public DateTime ReferenceDate
        {
            get => referenceDate;
            set => referenceDate = value;
        }
    }
}

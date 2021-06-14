using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvent
    {
        public TCURegistrationEvent(string _vin, TCU _tcu, DateTime _referenceDate)
        {
            VIN = _vin;
            TCU = _tcu;
            ReferenceDate = _referenceDate;
        }

        public string VIN { get; }

        public TCU TCU { get; }

        public DateTime ReferenceDate { get; }
    }
}

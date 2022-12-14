using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvent
    {
        public TCURegistrationEvent(string vin, TCU tcu, DateTime referenceDate)
        {
            VIN = vin;
            TCU = tcu;
            ReferenceDate = referenceDate;
        }

        public string VIN { get; }

        public TCU TCU { get; }

        public DateTime ReferenceDate { get; }
    }
}

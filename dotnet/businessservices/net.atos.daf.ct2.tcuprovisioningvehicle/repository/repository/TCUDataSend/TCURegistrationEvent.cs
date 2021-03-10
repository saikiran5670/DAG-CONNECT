using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    public class TCURegistrationEvent
    {
        private String vin;
        private TCU tcu;
        private String tcuRegistration;
        private DateTime referenceDate;

        public TCURegistrationEvent(string VIN, TCU TCU, String TCURegistration, DateTime referenceDate)
        {
            this.vin = VIN;
            this.tcu = TCU;
            this.tcuRegistration = TCURegistration;
            this.referenceDate = referenceDate;
        }

        public string VIN { get => vin; set => vin = value; }

        public TCU TCU { get => tcu; set => tcu = value; }

        public String TCURegistration { get => tcuRegistration; set => tcuRegistration = value; }

        public DateTime ReferenceDate { get => referenceDate; set => referenceDate = value; }

       

    }
}

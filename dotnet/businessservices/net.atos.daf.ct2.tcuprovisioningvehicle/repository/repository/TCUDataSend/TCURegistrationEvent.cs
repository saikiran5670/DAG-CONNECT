using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    public class TCURegistrationEvent
    {
        private String vin;
        private TCU tcu;
        private String tCURegistration;
        private DateTime referenceDate;

        public TCURegistrationEvent(string vin, TCU tcu, string tCURegistration, DateTime referenceDate)
        {
            this.vin = vin;
            this.tcu = tcu;
            this.tCURegistration = tCURegistration;
            this.referenceDate = referenceDate;
        }

        public string VIN { get => vin; set => vin = value; }

        public TCU TCU { get => tcu; set => tcu = value; }

        public DateTime ReferenceDate { get => referenceDate; set => referenceDate = value; }
        public string TCURegistration { get => tCURegistration; set => tCURegistration = value; }
    }
}

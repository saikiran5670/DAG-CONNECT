using System;

namespace TCUSend
{
    public class TCURegistrationEvent
    {
        private String vin;
        private TCU tcu;
        private DateTime referenceDate;

        public TCURegistrationEvent(string vin, TCU tcu, DateTime referenceDate)
        {
            this.vin = vin;
            this.tcu = tcu;
            this.referenceDate = referenceDate;
        }

        public string VIN { get => vin; set => vin = value; }

        public TCU TCU { get => tcu; set => tcu = value; }

        public DateTime ReferenceDate { get => referenceDate; set => referenceDate = value; }
    }
}

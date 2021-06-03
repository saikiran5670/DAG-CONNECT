using System;

namespace TCUReceive
{
    public class TCUDataReceive
    {
        public TCUDataReceive(string vin, string deviceIdentifier, string deviceSerialNumber, Correlations correlations, DateTime referenceDate)
        {
            Vin = vin;
            DeviceIdentifier = deviceIdentifier;
            DeviceSerialNumber = deviceSerialNumber;
            Correlations = correlations;
            ReferenceDate = referenceDate;
        }

        public string Vin { get;  }

        public string DeviceIdentifier { get;  }

        public string DeviceSerialNumber { get;  }

        public Correlations Correlations { get;  }
        public DateTime ReferenceDate { get;  }
    }
}

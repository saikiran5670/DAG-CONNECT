using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.kafkacdc.entity
{
    public class VehicleCdc
    {
        public string Vin { get; set; }
        public string Vid { get; set; }
        public string Status { get; set; }
        public string FuelType { get; set; }
        public string FuelTypeCoefficient { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class FleetFuelRankingPdf
    {
        public int Ranking { get; set; }
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public double Consumption { get; set; }
    }
}

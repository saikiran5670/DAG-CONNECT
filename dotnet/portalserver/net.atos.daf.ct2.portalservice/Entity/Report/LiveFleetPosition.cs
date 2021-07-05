using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class LiveFleetPosition
    {
        public double GpsAltitude { get; set; }
        public double GpsHeading { get; set; }
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public int Id { get; set; }
        public string TripId { get; set; }
        public double Fuelconsumtion { get; set; }
        public double Co2emission { get; set; }

    }
}

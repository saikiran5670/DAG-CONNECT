using System;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleOptInOptOut
    {
        public int RefId { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
    }
}

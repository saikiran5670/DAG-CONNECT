using System;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.vehicleservicerest.Entity
{
    public class VehicleOptInOptOutRequest
    {
        public int VehicleId { get; set; }
        public int AccountId { get; set; }
        public VehicleStatusType Status { get; set; }
        public DateTime Date { get; set; }
    }
}

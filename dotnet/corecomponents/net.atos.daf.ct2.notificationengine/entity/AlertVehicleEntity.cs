using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class AlertVehicleEntity
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public string VehicleRegNo { get; set; }
        public string VehicleName { get; set; }
        public int AlertCreatedAccountId { get; set; }
    }
}

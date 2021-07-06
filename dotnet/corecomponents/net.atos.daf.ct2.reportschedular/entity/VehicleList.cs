using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class VehicleList
    {
        public string VIN { get; set; }

        public string VehicleName { get; set; }

        public string RegistrationNo { get; set; }

        public int Id { get; set; }

    }
    public class VehicleListByGroup : VehicleList
    {
        public int VehicleGroupId { get; set; }

        public string VehicleGroupName { get; set; }
    }
}

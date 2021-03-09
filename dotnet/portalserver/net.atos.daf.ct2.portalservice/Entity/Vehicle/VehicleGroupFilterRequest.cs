﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class VehicleGroupFilterRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public bool Vehicles { get; set; }
        public bool VehiclesGroup { get; set; }
        public List<int> GroupIds { get; set; }


    }
}

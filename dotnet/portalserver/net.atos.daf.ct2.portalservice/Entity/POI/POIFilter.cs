﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class POIFilter
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}

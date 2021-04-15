﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Relationship
{
    public class RelationshipFilter
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int FeaturesetId { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }
    }
}

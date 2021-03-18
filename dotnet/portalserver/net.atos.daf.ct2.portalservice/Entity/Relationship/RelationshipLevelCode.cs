﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Relationship
{
    public class RelationshipLevelCode
    {
        public List<Level> Levels { get; set; }
        public List<Code> Codes { get; set; }
    }
    public class Code
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

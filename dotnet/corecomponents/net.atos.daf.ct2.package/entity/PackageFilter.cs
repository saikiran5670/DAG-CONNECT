﻿using net.atos.daf.ct2.package.ENUM;

namespace net.atos.daf.ct2.package.entity
{
    public class PackageFilter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int FeatureSetId { get; set; }       
        public string  Status { get; set; }       
    }
}

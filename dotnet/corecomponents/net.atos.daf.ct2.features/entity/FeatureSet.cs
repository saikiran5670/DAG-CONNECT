using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.features.entity
{
    public class FeatureSet
    {
        public int FeatureSetID { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public bool Is_Active { get; set; } 
        public bool is_custom_feature_set { get; set; }
        public List<Feature> Features { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
namespace net.atos.daf.ct2.features.entity
{   
     public class Features
    {
        public int FeatureId { get; set; }
        public string FeatureName{ get; set; }
        public List<Feature> FeatureList { get; set; }
    }
}

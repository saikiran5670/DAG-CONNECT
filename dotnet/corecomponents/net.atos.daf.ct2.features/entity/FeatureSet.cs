using System.Collections.Generic;

namespace net.atos.daf.ct2.features.entity
{
    public class FeatureSet
    {
        public int FeatureSetID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public char State { get; set; }
        public bool Is_custom_feature_set { get; set; }
        public List<Feature> Features { get; set; }
        public long Created_at { get; set; }
        public int Created_by { get; set; }
        public long Modified_at { get; set; }
        public int Modified_by { get; set; }
        public StatusType Status { get; set; }


    }
}

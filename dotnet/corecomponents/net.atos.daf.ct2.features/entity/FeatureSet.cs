using System.Collections.Generic;

namespace net.atos.daf.ct2.features.entity
{
    public class FeatureSet
    {
        public int FeatureSetID { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public char State { get; set; }
        public bool is_custom_feature_set { get; set; }
        public List<Feature> Features { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public StatusType status { get; set; }


    }
}

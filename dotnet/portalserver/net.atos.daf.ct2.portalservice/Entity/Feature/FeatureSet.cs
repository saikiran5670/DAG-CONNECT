using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class FeatureSet
    {
        public int FeatureSetID { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public bool Is_Active { get; set; }
        public bool is_custom_feature_set { get; set; }
        public List<Features> Features { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public StatusType status { get; set; }


    }
}

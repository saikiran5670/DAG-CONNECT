using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.account.entity
{
    public class MenuFeatureDto
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureType { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string ParentMenuName { get; set; }
        public string MenuKey { get; set; }
        public string MenuSeqNo { get; set; }
    }
}

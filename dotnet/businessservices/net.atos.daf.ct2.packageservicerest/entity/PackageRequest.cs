using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.ENUM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.packageservicerest.entity
{
    public class PackageRequest
    {
        public int Id { get; set; }
        public string code { get; set; }
        public FeatureSet feature_set { get; set; }
        public string name { get; set; }
        public PackageType type { get; set; }
        public string short_description { get; set; }
        public PackageDefault is_default { get; set; }     
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_active { get; set; }
    }
}

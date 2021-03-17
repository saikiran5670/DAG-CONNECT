using net.atos.daf.ct2.package.ENUM;
using System.Collections.Generic;

namespace net.atos.daf.ct2.package.entity
{
    public class Package
    {
        public int Id{get;set;}
        public string Code{get;set;}
        public int FeatureSetID { get; set; }
        public List<string> Features { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }     
        public bool Status { get; set; }
    }  
    public class PackageMaster
    {
        public List<Package> packages { get; set; }     

    }
}

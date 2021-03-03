using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.ENUM;
using System;
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
        public PackageType Pack_Type { get; set; }
        public string ShortDescription { get; set; }
        public PackageDefault Default { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PackageStatus Status { get; set; }

    }
    public class FeatureSetNamedStructure  { 
    
    
    
    }

    //public class PackageMaster
    //{
    //    public List<Package> packages { get; set; }
    //   // public FeatureSet FeatureSet { get; set; }

    //}
}

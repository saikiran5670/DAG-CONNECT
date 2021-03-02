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
        public PackageDefault Default { get; set; } //need to clarify         
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PackageStatus Status { get; set; }

    }
    public class FeatureSetNamedStructure  { 
    
    
    
    }

    //public class PackageSet
    //{

    //    public int Id { get; set; }
    //    public string Code { get; set; }
    //    //public int FeatureSetID { get; set; }
    //    public FeatureSet FeatureSet { get; set; }
    //    public string Name { get; set; }
    //    public PackageType Pack_Type { get; set; }
    //    public string ShortDescription { get; set; }
    //    public PackageDefault Default { get; set; } //need to clarify         
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public bool Is_Active { get; set; }

    //    //public string code { get; set; }       
    //    //public List<string> featureSet { get; set; }
    //    //public bool is_active { get; set; }
    //    //public string name { get; set; }
    //    //public string type { get; set; }
    //    //public string short_description { get; set; }
    //    //public DateTime start_date { get; set; }
    //    //public DateTime end_date { get; set; }
    //    //public string is_default { get; set; }
    //    //public bool? is_Active { get; set; }
    //}

    public class PackageMaster
    {
        public List<Package> packages { get; set; }
        public FeatureSet FeatureSet { get; set; }

    }
}

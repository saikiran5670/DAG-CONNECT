using System;

namespace net.atos.daf.ct2.visibility.entity
{
    public class FeatureType : DateTimeStamp
    {
        public int RoleFeatureTypeId {get;set;}
        public string FeatureTypeDescription { get; set; }
    }
}

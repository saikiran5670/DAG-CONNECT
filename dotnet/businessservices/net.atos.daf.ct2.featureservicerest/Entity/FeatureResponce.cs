using System;

namespace net.atos.daf.ct2.featureservicerest.Entity
{
    public class FeatureResponce
    {
        public int Id { get; set; }
        public string FeatureName { get; set; }    
        public string Description { get; set; }
        public int   RoleId { get; set; }
        public int OrganizationId { get; set; }
        public int CreatedBy { get; set; }
    }
}

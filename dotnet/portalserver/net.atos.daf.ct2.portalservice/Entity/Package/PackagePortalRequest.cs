
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Package
{
    public class PackagePortalRequest
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public int FeatureSetID { get; set; }
        [Required]
        public List<string> Features { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }        
        public string Status { get; set; }
    }

    public class PackageImportRequest
    {
        [Required]
        public List<PackagePortalRequest> packages { get; set; }
    }
}

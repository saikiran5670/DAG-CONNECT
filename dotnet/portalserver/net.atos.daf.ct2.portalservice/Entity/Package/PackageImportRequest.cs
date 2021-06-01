using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.Entity.Package
{
    public class ImportPackage
    {

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        [Required]
        public int FeatureSetID { get; set; }
        [Required]
        public List<string> Features { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(1)]
        public string Type { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [StringLength(1)]
        public string State { get; set; }
        [Required]
        [StringLength(1)]
        public string Status { get; set; }
        public long CreatedAt { get; set; }
    }
    public class PackageImportRequest
    {
        [Required]
        public List<ImportPackage> packagesToImport { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.organization.entity
{
    public class OrgRelationship
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int FeaturesetId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }
        [StringLength(120, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Description { get; set; }
        public List<string> Features { get; set; }
        public bool IsActive { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class RelationshipPortalRequest
    {
        public int Id { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        public int FeaturesetId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string Name { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }
        [StringLength(100, MinimumLength = 0, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string Description { get; set; }
        [Required]
        public List<int> FeatureIds { get; set; }
        [StringLength(1)]
        public string State { get; set; }
    }
}

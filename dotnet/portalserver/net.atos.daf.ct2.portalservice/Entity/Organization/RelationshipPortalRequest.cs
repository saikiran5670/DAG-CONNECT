using System;
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
        public string Name { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }
        [StringLength(120, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Description { get; set; }
        [Required]
        public List<string> Features { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}

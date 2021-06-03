using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.organizationservice.entity
{
    public class OrgRelationshipRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }
        [StringLength(120)]
        public string Description { get; set; }
        public List<string> Features { get; set; }
        public string FeatureSetId { get; set; }

    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class DTCIconUpdateRequest
    {
        public string Name { get; set; }
        [Required]
        public byte[] Icon { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }

    }

    public class DTCWarningIconUpdateRequest
    {
        [Required]
        public List<DTCIconUpdateRequest> dtcWarningUpdateIcon { get; set; }
    }
}

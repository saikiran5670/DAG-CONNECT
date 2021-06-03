using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.organization.entity
{
    public class KeyHandOver
    {
        [Required]
        public KeyHandOverEvent KeyHandOverEvent { get; set; }
    }
    public class KeyHandOverEvent
    {
        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string TCUID { get; set; }
        [Required]
        public EndCustomer EndCustomer { get; set; }
        [Required]
        [StringLength(3, MinimumLength = 2)]
        public string TCUActivation { get; set; }
        [Required]
        public string ReferenceDateTime { get; set; }
    }
}


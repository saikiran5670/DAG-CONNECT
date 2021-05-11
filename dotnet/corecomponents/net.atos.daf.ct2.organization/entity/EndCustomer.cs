using System;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.organization.entity
{
    public class EndCustomer
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ID { get; set; } 
        public string Name { get; set; } 
        public Address Address { get; set; } 
    }
}

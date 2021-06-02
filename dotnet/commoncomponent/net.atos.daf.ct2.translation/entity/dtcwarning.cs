using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.translation.entity
{
    public class DTCwarning
    {
        public int Id { get; set; }
        [Required]
        [StringLength(8)]
        public string Code { get; set; }
        [Required]
        [StringLength(1)]
        public string Type { get; set; }
        [StringLength(1)]
        public string VehType { get; set; }
        [Required]
        public int WarningClass { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Advice { get; set; }
        public int IconId { get; set; }
        public long ExpiresAt { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public long ModifyAt { get; set; }
        public int ModifyBy { get; set; }
        public string WarningType { get; set; }
        public string Message { get; set; }

    }
    //public class DTCWarningImportRequest
    //{
    //    [Required]
    //    public List<DTCwarning> dtcWarningToImport { get; set; }
    //}
}

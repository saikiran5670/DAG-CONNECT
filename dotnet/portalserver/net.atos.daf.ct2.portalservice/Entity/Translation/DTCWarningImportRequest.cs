using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
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
        public string Veh_type { get; set; }
        [Required]
        public int Warning_class { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Advice { get; set; }
        public int Icon_id { get; set; }
        public long Expires_at { get; set; }
        public long Created_at { get; set; }
        public int Created_by { get; set; }
        public long Modify_at { get; set; }
        public int Modify_by { get; set; }
    }

    public class DTCWarningImportRequest
    {
        [Required]
        public List<DTCwarning> DtcWarningToImport { get; set; }
    }

    public enum WarningType
    {
        DTC = 'D',
        DM = 'M'

    }

}




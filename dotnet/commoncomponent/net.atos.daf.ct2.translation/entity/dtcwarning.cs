using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.translation.entity
{
    public class DTCwarning
    {
        public int id { get; set; }
        [Required]
        [StringLength(8)]
        public string code { get; set; }
        [Required]
        [StringLength(1)]
        public string type { get; set; }
        [StringLength(1)]
        public string veh_type { get; set; }
        [Required]
        public int warning_class { get; set; }
        [Required]
        public int number { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string advice { get; set; }
        public int icon_id { get; set; }
        public long expires_at { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modify_at { get; set; }
        public int modify_by { get; set; }
        public string Warning_type { get; set; }
        public string message { get; set; }

    }
    public class DTCWarningImportRequest
    {
        [Required]
        public List<DTCwarning> dtcWarningToImport { get; set; }
    }
}

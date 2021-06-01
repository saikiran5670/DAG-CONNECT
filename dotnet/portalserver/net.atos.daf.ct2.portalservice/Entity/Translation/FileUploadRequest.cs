using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class FileUploadRequest
    {
        [Required(ErrorMessage = "File name is required.")]
        public string file_name { get; set; }
        public string description { get; set; }
        [Required]
        public int file_size { get; set; }
        public int failure_count { get; set; }
        //public int created_by { get; set; }   
        [Required]
        public List<FileData> file { get; set; }
        public int added_count { get; set; }
        public int updated_count { get; set; }

    }

    public class FileData
    {
        public string code { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}

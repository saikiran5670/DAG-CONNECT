using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class FileUploadRequest
    {
        [Required(ErrorMessage = "File name is required.")]
        public string File_name { get; set; }
        public string Description { get; set; }
        [Required]
        public int File_size { get; set; }
        public int Failure_count { get; set; }
        //public int created_by { get; set; }   
        [Required]
        public List<FileData> File { get; set; }
        public int Added_count { get; set; }
        public int Updated_count { get; set; }

    }

    public class FileData
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

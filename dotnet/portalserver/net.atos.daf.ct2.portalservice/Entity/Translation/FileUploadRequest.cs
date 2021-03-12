using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class FileUploadRequest
    {
        public string file_name { get; set; }
        public string description { get; set; }
        public int file_size { get; set; }
        public int failure_count { get; set; }
        //public int created_by { get; set; }
        public List<FileData> file { get; set; }
        public int added_count { get; set; }
        public string updated_count { get; set; }

    }

    public class FileData
    {
        public string code { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}

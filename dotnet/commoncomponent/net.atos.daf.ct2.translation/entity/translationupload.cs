using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.translation.entity
{
    public class Translationupload
    {
        public int id { get; set; }
        public string file_name { get; set; }
        public string description { get; set; }
        public int file_size { get; set; }
        public int failure_count { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public byte[] file { get; set; }
        public int added_count { get; set; }
        public string updated_count { get; set; }
        public List<Translations> translations { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.translation.entity
{
    public class Icon
    {
        public int id { get; set; }
        public byte icon { get; set; }
        public char type { get; set; }
        public int warning_class { get; set; }
        public int number { get; set; }
        public string name { get; set; }
        public char color_name { get; set; }
        public char state { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modify_at { get; set; }
        public int modify_by { get; set; }
        
    }
}

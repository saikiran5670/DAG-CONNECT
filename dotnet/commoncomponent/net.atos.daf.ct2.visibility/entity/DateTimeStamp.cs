using System;

namespace net.atos.daf.ct2.visibility.entity
{
   public class DateTimeStamp
    {
        public int createdby { get; set; }
        public int modifiedby{get; set;}
        public DateTime createddate { get; set; }
        public DateTime modifieddate { get; set; }
        public bool isactive { get; set; }
    }
}

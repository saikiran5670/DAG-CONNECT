using System;

namespace net.atos.daf.ct2.features.entity
{
    public class DateTimeStamp
    {
        public int Createdby { get; set; }
        public int Updatedby { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime Updateddate { get; set; }
        public bool Is_active { get; set; }
    }
}

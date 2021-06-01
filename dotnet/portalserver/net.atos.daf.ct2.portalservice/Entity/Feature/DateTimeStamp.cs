using System;

namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class DateTimeStamp
    {
        public int Createdby { get; set; }
        public int Updatedby { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime Updateddate { get; set; }
        public bool is_active { get; set; }
    }
}

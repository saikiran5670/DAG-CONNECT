using System;

namespace net.atos.daf.ct2.driver.entity
{
    public class DateTimeStamp
    {
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Isactive { get; set; }
    }
}

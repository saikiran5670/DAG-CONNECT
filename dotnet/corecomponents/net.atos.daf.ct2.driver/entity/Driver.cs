using System;

namespace net.atos.daf.ct2.driver.entity
{
    public class Driver
    {
        public int Id { get; set; }
        public int Organization_id { get; set; }        
        public string Driver_id_ext { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long DateOfBith { get; set; }
        public string Status { get; set; }
        public Boolean IsActive { get; set; }
    }
}

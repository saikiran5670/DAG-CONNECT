using System;

namespace net.atos.daf.ct2.organization.entity
{ 
    public class KeyHandOver  {
        public KeyHandOverEvent KeyHandOverEvent { get; set; } 
    }
    public class KeyHandOverEvent   {
        public string VIN { get; set; } 
        public string TCUID { get; set; } 
        public EndCustomer EndCustomer { get; set; } 
        public string TCUActivation { get; set; } 
        public string ReferenceDateTime { get; set; } 
    }
}


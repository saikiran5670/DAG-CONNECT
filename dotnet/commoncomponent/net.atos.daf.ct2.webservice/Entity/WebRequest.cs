using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.webservice.Entity
{
    public class WebRequest
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string State { get; set; }
    }
}

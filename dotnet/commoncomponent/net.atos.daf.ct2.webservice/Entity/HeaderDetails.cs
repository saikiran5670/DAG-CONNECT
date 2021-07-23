using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.webservice.entity
{
    public class HeaderDetails
    {
        public string ContentType { get; set; }
        public string BaseUrl { get; set; }
        public string Header { get; set; }
        public string MethodType { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AuthType { get; set; }
    }
}

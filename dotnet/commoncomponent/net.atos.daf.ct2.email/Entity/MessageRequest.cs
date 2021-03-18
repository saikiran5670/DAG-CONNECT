using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.email.Entity
{
    public class MessageRequest
    {
         public Dictionary<string, string> ToAddressList { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ContentMimeType { get; set; }
        public EmailConfiguration Configuration { get; set; }
    }
} 

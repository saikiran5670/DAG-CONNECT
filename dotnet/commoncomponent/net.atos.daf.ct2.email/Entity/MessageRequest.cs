using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.email.Entity
{
    public static class MimeType
    {
        public const string Text = "text/plain";
        public const string Html = "text/html";
    }

    public class MessageRequest
    {
         public Dictionary<string, string> ToAddressList { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ContentMimeType { get; set; }
        public EmailConfiguration Configuration { get; set; }
    }
} 

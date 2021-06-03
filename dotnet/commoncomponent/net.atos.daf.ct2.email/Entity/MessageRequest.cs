using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.email.Entity
{
    public class MessageRequest
    {
        public MessageRequest()
        {
            Subject = " ";
        }
        public Dictionary<string, string> ToAddressList { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ContentMimeType { get; set; }
        public EmailConfiguration Configuration { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public Guid? TokenSecret { get; set; }
        public int RemainingDaysToExpire { get; set; }
    }
}

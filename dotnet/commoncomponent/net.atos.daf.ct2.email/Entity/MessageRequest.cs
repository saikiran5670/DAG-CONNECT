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
        public List<ReportTokens> ReportTokens { get; set; } = new List<ReportTokens>();
        public int MailRetryCount { get; set; } = 1;
        public string LanguageCode { get; set; }
        public bool IsBcc { get; set; }
        public string Description { get; set; }
    }

    public class ReportTokens
    {
        public string Token { get; set; }
        public string ReportName { get; set; }
    }
}

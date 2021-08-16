using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.sms.entity
{
    public class SMSConfiguration
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
        public string MessagingServiceSid { get; set; }
    }
}

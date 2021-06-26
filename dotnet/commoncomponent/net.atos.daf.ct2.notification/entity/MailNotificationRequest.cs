using System;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;

namespace net.atos.daf.ct2.notification.entity
{
    public class MailNotificationRequest
    {
        public MessageRequest MessageRequest { get; set; }
        public EmailEventType EventType { get; set; }
        public Guid? TokenSecret { get; set; } = null;
        public EmailContentType ContentType { get; set; } = EmailContentType.Html;
    }
}

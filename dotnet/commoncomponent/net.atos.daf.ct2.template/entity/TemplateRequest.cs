using net.atos.daf.ct2.email.Enum;

namespace net.atos.daf.ct2.template.entity
{
    public class TemplateRequest
    {
        public EmailEventType EventType { get; set; }
        public EmailContentType ContentType { get; set; } = EmailContentType.Html;
        public string LanguageCode { get; set; } = "EN-GB";
    }
}

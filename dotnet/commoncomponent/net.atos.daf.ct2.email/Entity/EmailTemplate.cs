using System.Collections.Generic;
using net.atos.daf.ct2.email.Enum;

namespace net.atos.daf.ct2.email.entity
{
    public class EmailTemplate
    {
        public int TemplateId { get; set; }
        public char ContentType { get; set; }
        public EmailEventType EventType { get; set; }
        public string Description { get; set; }
        public IEnumerable<EmailTemplateTranslationLabel> TemplateLabels { get; set; }
    }

    public class EmailTemplateTranslationLabel
    {
        public string LabelKey { get; set; }
        public string TranslatedValue { get; set; }
    }
}

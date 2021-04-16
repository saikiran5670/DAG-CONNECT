using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.email.entity
{
    public class EmailTemplate
    {
        public int TemplateId { get; set; }
        //public char Type { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<EmailTemplateTranslationLabel> TemplateLabels { get; set; }
    }

    public class EmailTemplateTranslationLabel
    {
        public string LabelKey { get; set; }
        public string TranslatedValue { get; set; }
    }
}

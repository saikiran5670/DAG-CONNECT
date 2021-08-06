using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.template.entity;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public class ReportTemplateSingleto
    {
        private static ReportTemplateSingleto _instance;
        private List<ReportTemplate> _reportTemplate;
        private static readonly Object _root = new object();

        private ReportTemplateSingleto()
        {
        }

        public static ReportTemplateSingleto GetInstance()
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _instance = new ReportTemplateSingleto();
                    _instance._reportTemplate = new List<ReportTemplate>();
                }
            }
            return _instance;
        }

        public string GetReportTemplate(ITemplateManager templateManager, int reportId, EmailEventType evenType, EmailContentType contentType, string languageCode)
        {
            if (_reportTemplate.Any(w => w.ReportId == reportId && w.LanguageCode == languageCode && w.EventType == evenType))
            {
                return _reportTemplate.Where(w => w.ReportId == reportId && w.LanguageCode == languageCode).FirstOrDefault()?.ReportTranslatedContent;
            }
            else
            {
                lock (_root)
                {
                    var templateContent = templateManager
                                            .GetMultiLingualTemplate(
                                            new TemplateRequest
                                            {
                                                ContentType = contentType,
                                                EventType = evenType,
                                                LanguageCode = languageCode
                                            }).Result;
                    _instance._reportTemplate.Add(
                        new ReportTemplate
                        {
                            ReportId = reportId,
                            ReportTranslatedContent = templateContent,
                            LanguageCode = languageCode,
                            EventType = evenType
                        }
                        );
                    return templateContent;
                }
            }
        }

    }
}

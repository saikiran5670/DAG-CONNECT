using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.template.entity;
using net.atos.daf.ct2.translation.repository;

namespace net.atos.daf.ct2.template
{
    public class TemplateManager : ITemplateManager
    {
        private readonly ITranslationRepository _translationRepository;

        public TemplateManager(ITranslationRepository translationRepository)
        {

            _translationRepository = translationRepository;
        }


        public async Task<string> GetMultiLingualTemplate(TemplateRequest templateRequest)
        {

            try
            {
                return GetContent(await _translationRepository.GetEmailTemplateTranslations(templateRequest.EventType,
                                                                                           templateRequest.ContentType,
                                                                                           templateRequest.LanguageCode));

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GetContent(EmailTemplate reportTemplate)
        {
            var replacedContent = reportTemplate.Description;
            Regex regex = new Regex(@"\[(.*?)\]");

            var dictLabels = reportTemplate.TemplateLabels.ToDictionary(x => x.LabelKey, x => x.TranslatedValue);

            foreach (Match match in regex.Matches(reportTemplate.Description))
            {
                if (dictLabels.ContainsKey(match.Groups[1].Value))
                    replacedContent = replacedContent.Replace(match.Value, dictLabels[match.Groups[1].Value]);
            }
            return replacedContent;
        }
    }
}

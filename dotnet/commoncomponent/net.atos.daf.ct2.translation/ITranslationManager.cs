using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.translation.entity;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation
{
    public interface ITranslationManager
    {
        Task<IEnumerable<Langauge>> GetAllLanguageCode();
        Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode, string key);
        Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key);
        //    Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId);
        Task<IEnumerable<Translations>> GetTranslationsByMenu(int MenuId, MenuType type, string langaguecode);
        Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode);
        Task<TranslationDataStatus> InsertTranslationFileDetails(Translationupload translationupload);
        Task<IEnumerable<Translationupload>> GetFileUploadDetails(int FileID);
        Task<List<DTCwarning>> ImportDTCWarningData(List<DTCwarning> dtcwarningList);
        Task<IEnumerable<DTCwarning>> GetDTCWarningData(string LanguageCode);
        Task<List<DTCwarning>> UpdateDTCWarningData(List<DTCwarning> dtcwarningList);
        // Task<int> DeleteDTCWarningData(int id);
        Task<EmailTemplate> GetEmailTemplateTranslations(EmailEventType eventType, EmailContentType contentType, string languageCode);
    }
}

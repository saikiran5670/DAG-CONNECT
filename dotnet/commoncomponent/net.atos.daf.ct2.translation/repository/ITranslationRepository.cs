using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.translation.entity;

namespace net.atos.daf.ct2.translation.repository
{
    public interface ITranslationRepository
    {
        Task<IEnumerable<Langauge>> GetAllLanguageCode();
        Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode, string key);
        Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key);
        //   Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId);
        Task<IEnumerable<Translations>> GetTranslationsByMenu(int MenuId, string type, string langaguecode);
        Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode);
        Task<Translationupload> InsertTranslationFileDetails(Translationupload translationupload);
        Task<IEnumerable<Translationupload>> GetFileUploadDetails(int FileID);
        Task<translationStatus> InsertTranslationFileData(Translations translationdata, List<Translations> TranslationsList);
        Task<List<Translations>> GetAllTranslations();
        Task<List<DTCwarning>> ImportDTCWarningData(List<DTCwarning> dtcwarningList);
        Task<IEnumerable<DTCwarning>> GetDTCWarningData(string LanguageCode);
        Task<List<DTCwarning>> UpdateDTCWarningData(List<DTCwarning> dtcwarningList);
        //Task<int> DeleteDTCWarningData(int id);

        Task<EmailTemplate> GetEmailTemplateTranslations(EmailEventType eventType, EmailContentType contentType, string languageCode);
    }
}

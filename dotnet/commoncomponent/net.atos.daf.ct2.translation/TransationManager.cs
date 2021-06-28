using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.repository;
using static net.atos.daf.ct2.translation.Enum.Translationenum;

namespace net.atos.daf.ct2.translation
{
    public class TranslationManager : ITranslationManager
    {
        private readonly ITranslationRepository _translationRepository; // = new TranslationRepository();
        // private static readonly log4net.ILog log =
        // log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public TranslationManager(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public async Task<IEnumerable<Langauge>> GetAllLanguageCode()
        {
            try
            {
                var result = await _translationRepository.GetAllLanguageCode();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langagueCode, string key)
        {
            try
            {
                var result = await _translationRepository.GetKeyTranslationByLanguageCode(langagueCode, key);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key)
        {
            try
            {
                var result = await _translationRepository.GetLangagugeTranslationByKey(key);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int menuId, MenuType type, string langaguecode)
        {
            try
            {
                var result = await _translationRepository.GetTranslationsByMenu(menuId, ((char)type).ToString(), langaguecode);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string dropdownname, string langagugecode)
        {
            try
            {
                var result = await _translationRepository.GetTranslationsForDropDowns(dropdownname, langagugecode);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TranslationDataStatus> InsertTranslationFileDetails(Translationupload translationupload)
        {
            try
            {

                TranslationDataStatus tdataStatus = new TranslationDataStatus();
                tdataStatus.AddCount = 0;
                tdataStatus.UpdateCount = 0;
                tdataStatus.FailedCount = 0;
                var translationsList = await _translationRepository.GetAllTranslations();
                foreach (var item in translationupload.Translationss)
                {
                    var translationtResult = await InsertTranslationFileData(item, translationsList);
                    if (translationtResult == TranslationStatus.Added)
                        tdataStatus.AddCount = tdataStatus.AddCount + 1;
                    else if (translationtResult == TranslationStatus.Updated)
                        tdataStatus.UpdateCount = tdataStatus.UpdateCount + 1;
                    else if (translationtResult == TranslationStatus.Failed)
                        tdataStatus.FailedCount = tdataStatus.FailedCount + 1;
                }
                translationupload.AddedCount = tdataStatus.AddCount;
                translationupload.UpdatedCount = tdataStatus.UpdateCount;
                translationupload.FailureCount = tdataStatus.FailedCount;
                var result = await _translationRepository.InsertTranslationFileDetails(translationupload);
                return tdataStatus;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<TranslationStatus> InsertTranslationFileData(Translations translationupload, List<Translations> translationsList)
        {
            try
            {

                var result = await _translationRepository.InsertTranslationFileData(translationupload, translationsList);
                //Translationupload v = new Translationupload();
                return result;
                //return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translationupload>> GetFileUploadDetails(int fileID)
        {
            try
            {
                var result = await _translationRepository.GetFileUploadDetails(fileID);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DTCwarning>> ImportDTCWarningData(List<DTCwarning> dtcwarningList)
        {
            try
            {
                var result = await _translationRepository.ImportDTCWarningData(dtcwarningList);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTCwarning>> GetDTCWarningData(string languageCode)
        {
            try
            {
                var result = await _translationRepository.GetDTCWarningData(languageCode);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmailTemplate> GetEmailTemplateTranslations(EmailEventType eventType, EmailContentType contentType, string languageCode)
        {
            return await _translationRepository.GetEmailTemplateTranslations(eventType, contentType, languageCode);
        }

        public async Task<List<DTCwarning>> UpdateDTCWarningData(List<DTCwarning> dtcwarningList)
        {
            var result = await _translationRepository.UpdateDTCWarningData(dtcwarningList);
            return result;
        }

    }
}

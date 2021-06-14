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
        public TranslationManager(ITranslationRepository _repository)
        {
            _translationRepository = _repository;
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

        public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode, string key)
        {
            try
            {
                var result = await _translationRepository.GetKeyTranslationByLanguageCode(langaguecode, key);
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


        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int MenuId, MenuType type, string langaguecode)
        {
            try
            {
                var result = await _translationRepository.GetTranslationsByMenu(MenuId, ((char)type).ToString(), langaguecode);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode)
        {
            try
            {
                var result = await _translationRepository.GetTranslationsForDropDowns(Dropdownname, langagugecode);
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

                TranslationDataStatus TdataStatus = new TranslationDataStatus();
                TdataStatus.AddCount = 0;
                TdataStatus.UpdateCount = 0;
                TdataStatus.FailedCount = 0;
                var TranslationsList = await _translationRepository.GetAllTranslations();
                foreach (var item in translationupload.Translationss)
                {
                    var TranslationtResult = await InsertTranslationFileData(item, TranslationsList);
                    if (TranslationtResult == TranslationStatus.Added)
                        TdataStatus.AddCount = TdataStatus.AddCount + 1;
                    else if (TranslationtResult == TranslationStatus.Updated)
                        TdataStatus.UpdateCount = TdataStatus.UpdateCount + 1;
                    else if (TranslationtResult == TranslationStatus.Failed)
                        TdataStatus.FailedCount = TdataStatus.FailedCount + 1;
                }
                translationupload.AddedCount = TdataStatus.AddCount;
                translationupload.UpdatedCount = TdataStatus.UpdateCount;
                translationupload.FailureCount = TdataStatus.FailedCount;
                var result = await _translationRepository.InsertTranslationFileDetails(translationupload);
                return TdataStatus;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<TranslationStatus> InsertTranslationFileData(Translations translationupload, List<Translations> TranslationsList)
        {
            try
            {

                var result = await _translationRepository.InsertTranslationFileData(translationupload, TranslationsList);
                //Translationupload v = new Translationupload();
                return result;
                //return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Translationupload>> GetFileUploadDetails(int FileID)
        {
            try
            {
                var result = await _translationRepository.GetFileUploadDetails(FileID);
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

        public async Task<IEnumerable<DTCwarning>> GetDTCWarningData(string LanguageCode)
        {
            try
            {
                var result = await _translationRepository.GetDTCWarningData(LanguageCode);
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

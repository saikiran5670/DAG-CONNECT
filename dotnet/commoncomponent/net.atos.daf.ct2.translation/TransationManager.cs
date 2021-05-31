using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.repository;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation
{
    public class TranslationManager : ITranslationManager
    {
        private readonly ITranslationRepository Translationrepository; // = new TranslationRepository();
        // private static readonly log4net.ILog log =
        // log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public TranslationManager(ITranslationRepository _repository)
        {
            Translationrepository = _repository;
        }

        public async Task<IEnumerable<Langauge>> GetAllLanguageCode()
        {
            try
            {
                var result = await Translationrepository.GetAllLanguageCode();
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
                var result = await Translationrepository.GetKeyTranslationByLanguageCode(langaguecode, key);
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
                var result = await Translationrepository.GetLangagugeTranslationByKey(key);
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
                var result = await Translationrepository.GetTranslationsByMenu(MenuId, ((char)type).ToString(), langaguecode);
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
                var result = await Translationrepository.GetTranslationsForDropDowns(Dropdownname, langagugecode);
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
                var TranslationsList = await Translationrepository.GetAllTranslations();
                foreach (var item in translationupload.translations)
                {
                    var TranslationtResult = await InsertTranslationFileData(item, TranslationsList);
                    if (TranslationtResult == translationStatus.Added)
                        TdataStatus.AddCount = TdataStatus.AddCount + 1;
                    else if (TranslationtResult == translationStatus.Updated)
                        TdataStatus.UpdateCount = TdataStatus.UpdateCount + 1;
                    else if (TranslationtResult == translationStatus.Failed)
                        TdataStatus.FailedCount = TdataStatus.FailedCount + 1;
                }
                translationupload.added_count = TdataStatus.AddCount;
                translationupload.updated_count = TdataStatus.UpdateCount;
                translationupload.failure_count = TdataStatus.FailedCount;
                var result = await Translationrepository.InsertTranslationFileDetails(translationupload);
                return TdataStatus;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<translationStatus> InsertTranslationFileData(Translations translationupload, List<Translations> TranslationsList)
        {
            try
            {

                var result = await Translationrepository.InsertTranslationFileData(translationupload, TranslationsList);
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
                var result = await Translationrepository.GetFileUploadDetails(FileID);
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
                var result = await Translationrepository.ImportDTCWarningData(dtcwarningList);
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
                var result = await Translationrepository.GetDTCWarningData(LanguageCode);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmailTemplate> GetEmailTemplateTranslations(EmailEventType eventType, EmailContentType contentType, string languageCode)
        {
            return await Translationrepository.GetEmailTemplateTranslations(eventType, contentType, languageCode);
        }

        public async Task<List<DTCwarning>> UpdateDTCWarningData(List<DTCwarning> dtcwarningList)
        {
            var result = await Translationrepository.UpdateDTCWarningData(dtcwarningList);
            return result;
        }

        //public async Task<int> DeleteDTCWarningData(int id)
        //{
        //    var result = await Translationrepository.DeleteDTCWarningData(id);
        //    return result;
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode, string key)
        {
            try
            {
                var result = await Translationrepository.GetKeyTranslationByLanguageCode(langaguecode, key);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key)
        {
            try
            {
                var result = await Translationrepository.GetLangagugeTranslationByKey(key);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int MenuId, MenuType type, string langaguecode)
        {
            try
            {
                var result = await Translationrepository.GetTranslationsByMenu(MenuId, ((char)type).ToString(), langaguecode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode)
        {
            try
            {
                var result = await Translationrepository.GetTranslationsForDropDowns(Dropdownname, langagugecode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Translationupload> InsertTranslationFileDetails(Translationupload translationupload)
        {
            try
            {
                var result = await Translationrepository.InsertTranslationFileDetails(translationupload);
                TranslationDataStatus TdataStatus = new TranslationDataStatus();
                TdataStatus.AddCount = 0;
                TdataStatus.UpdateCount = 0;
                TdataStatus.FailedCount = 0;
                foreach (var item in translationupload.translations)
                {
                    var TranslationtResult = await InsertTranslationFileData(item);
                    if (TranslationtResult == translationStatus.Added)
                        TdataStatus.AddCount = TdataStatus.AddCount + 1;
                    else if (TranslationtResult == translationStatus.Updated)
                        TdataStatus.UpdateCount = TdataStatus.UpdateCount + 1;
                    else
                        TdataStatus.FailedCount = TdataStatus.FailedCount + 1;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<translationStatus> InsertTranslationFileData(Translations translationupload)
        {
            try
            {
                var TranslationsList = await Translationrepository.GetAllTranslations();
                var result = await Translationrepository.InsertTranslationFileData(translationupload, TranslationsList);
                //Translationupload v = new Translationupload();
                return result;
                //return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Translationupload>> GetFileUploadDetails(int FileID)
        {
            try
            {
                var result = await Translationrepository.GetFileUploadDetails(FileID);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

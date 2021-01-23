using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.repository;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation
{
    public class TranslationManager:ITranslationManager
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
            catch(Exception ex)
            {
                    throw ex;
            }
        }

         public async Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode,string key)
        {
            try
            {
                var result = await Translationrepository.GetKeyTranslationByLanguageCode(langaguecode,key);
                return result;
            }
            catch(Exception ex)
            {
                    throw ex;
            }
        }

        public async Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key, TranslationType Type)
        {
            try
            {
                var result = await Translationrepository.GetKeyTranslationByLanguageCode(key,((char)Type).ToString());
                return result;
            }
            catch(Exception ex)
            {
                    throw ex;
            }
        }


        public async Task<IEnumerable<Translations>> GetTranslationsByMenu(int  MenuId, MenuType type)
        {
            try
            {
                var result = await Translationrepository.GetTranslationsByMenu(MenuId,((char)type).ToString());
                return result;
            }
            catch(Exception ex)
            {
                    throw ex;
            }
        }

    }
}

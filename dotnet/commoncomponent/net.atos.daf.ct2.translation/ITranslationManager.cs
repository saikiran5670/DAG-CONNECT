using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.translation.entity;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation
{
    public interface ITranslationManager
    {
        
          Task<IEnumerable<Langauge>> GetAllLanguageCode();
          Task<IEnumerable<translations>> GetKeyTranslationByLanguageCode(string langaguecode,string key);
           Task<IEnumerable<translations>> GetLangagugeTranslationByKey(string key, TranslationType Type);
        //    Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId);
          Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId, MenuType type);
    }
}

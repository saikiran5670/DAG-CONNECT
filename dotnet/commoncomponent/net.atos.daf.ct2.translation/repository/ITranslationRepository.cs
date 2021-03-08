using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using net.atos.daf.ct2.translation.entity;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation.repository
{
    public interface ITranslationRepository
    {
          Task<IEnumerable<Langauge>> GetAllLanguageCode();
          Task<IEnumerable<Translations>> GetKeyTranslationByLanguageCode(string langaguecode,string key);
           Task<IEnumerable<Translations>> GetLangagugeTranslationByKey(string key);
        //   Task<IEnumerable<translations>> GetTranslationsByMenu(int  MenuId);
          Task<IEnumerable<Translations>> GetTranslationsByMenu(int  MenuId, string type,string langaguecode);

          Task<IEnumerable<Translations>> GetTranslationsForDropDowns(string Dropdownname, string langagugecode);
          
    }
}

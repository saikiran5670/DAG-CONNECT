using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using net.atos.daf.ct2.translation.entity;

namespace net.atos.daf.ct2.translation.repository
{
    public interface ITranslationRepository
    {
          Task<IEnumerable<Langauge>> GetAllLanguageCode();
    }
}

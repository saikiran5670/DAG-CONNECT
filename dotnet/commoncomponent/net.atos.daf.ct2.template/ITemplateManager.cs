using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.template.entity;

namespace net.atos.daf.ct2.template
{
    public interface ITemplateManager
    {
        Task<string> GetMultiLingualTemplate(TemplateRequest templateRequest);
    }
}

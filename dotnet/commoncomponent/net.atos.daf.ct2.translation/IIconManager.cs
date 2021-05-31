using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.translation.entity;

namespace net.atos.daf.ct2.translation
{
    public interface IIconManager
    {
        Task<bool> UpdateIcons(List<Icon> iconlist);
        Task<List<Icon>> GetIcons(int icon_id);
    }
}

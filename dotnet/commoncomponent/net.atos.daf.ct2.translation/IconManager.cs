using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.repository;

namespace net.atos.daf.ct2.translation
{
    public class IconManager : IIconManager
    {
        private readonly IIconRepository Iconrepository;

        public IconManager(IIconRepository _repository)
        {
            Iconrepository = _repository;
        }
        public async Task<bool> UpdateIcons(List<Icon> iconlist)
        {
            try
            {
                bool result = await Iconrepository.UpdateIcons(iconlist);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Icon>> GetIcons(int icon_id)
        {
            try
            {
                List<Icon> icon_list = await Iconrepository.GetIcons(icon_id);
                return icon_list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

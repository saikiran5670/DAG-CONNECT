using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.translation.repository;

namespace net.atos.daf.ct2.translation
{
    public class IconManager : IIconManager
    {
        private readonly IIconRepository _iconRepository;

        public IconManager(IIconRepository _repository)
        {
            _iconRepository = _repository;
        }
        public async Task<string> UpdateIcons(List<Icon> iconlist)
        {
            try
            {
                string result = "";
                string is_Result = "";
                int count = 0;
                int count1 = 0;
                foreach (var icon in iconlist)
                {
                    result = await _iconRepository.UpdateIcons(icon);

                    if (result != "" )
                    {
                        if (count > 0 || count1 > 0)
                        {
                            is_Result += " , ";
                        }
                        is_Result += result;
                        count++;
                    }
                    else
                    {
                        if (count1 > 0 || count > 0)
                        {
                            is_Result += " , ";
                        }
                        is_Result += " File Name not exist : " + icon.Name;
                        count1++;
                    }
                
                }
                return is_Result;
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
                List<Icon> icon_list = await _iconRepository.GetIcons(icon_id);
                return icon_list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

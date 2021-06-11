using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace net.atos.daf.ct2.accountservice.Entity
{
    public class AccountMenuModel
    {
        public List<MainMenu> Menus { get; set; }

        public AccountMenuModel()
        {
            Menus = new List<MainMenu>();
        }
    }

    public class MainMenu
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string TranslatedMenuName { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public int FeatureId { get; set; }
        public RepeatedField<SubMenu> SubMenus { get; set; }
    }

    public class SubMenu
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string TranslatedMenuName { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public int FeatureId { get; set; }
    }
}

using System.Collections.Generic;
using net.atos.daf.ct2.translation.entity;

namespace net.atos.daf.ct2.translationservicerest.Entity
{
    public class PreferenceResponce
    {
        public List<Translations> language { get; set; }
        public List<Translations> unit { get; set; }
        public List<Translations> timezone { get; set; }
        public List<Translations> currency { get; set; }
        public List<Translations> landingpagedisplay { get; set; }
        public List<Translations> dateformat { get; set; }
        public List<Translations> timeformat { get; set; }
        public List<Translations> vehicledisplay { get; set; }

    }



}

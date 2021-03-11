using System;
using static net.atos.daf.ct2.translation.Enum.translationenum;

namespace net.atos.daf.ct2.translation.entity
{
    public class Translations
    {

        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string  Value { get; set; }
        public string Filter { get; set; }
        public int MenuId { get; set; }
        public long created_at { get; set; }
        public long modified_at { get; set; }



    }

    public enum translationStatus
    { 
        Failed = 0,
        Updated = 1,
        Added = 2
        
    }
}

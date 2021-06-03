using System;

namespace net.atos.daf.ct2.translation.entity
{
    [Serializable]
    public class Translations
    {

        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public long? Created_at { get; set; }
        public long? Modified_at { get; set; }
        public string Filter { get; set; }
        public int MenuId { get; set; }



    }

    public enum TranslationStatus
    {
        Failed = 0,
        Updated = 1,
        Added = 2,
        Ignored = 3

    }
}

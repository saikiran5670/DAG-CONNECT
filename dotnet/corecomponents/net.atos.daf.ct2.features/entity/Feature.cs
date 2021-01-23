 using System;
 using System.Text.Json.Serialization;

namespace net.atos.daf.ct2.features.entity
{
    public class Feature : DateTimeStamp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string  Description { get; set; }
        public char  Type { get; set; }

        [JsonPropertyName("IsFeatureActive")]
        public bool Is_Active { get; set; }
        public int Data_attribute_Set_id { get; set; }

    }
}
  
 using System;
 using System.Text.Json.Serialization;
using System.Collections.Generic;

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
        public int RoleId { get; set; }
        public int Organization_Id { get; set; }
        public string  Key { get; set; }
        public DataAttributeSet DataAttributeSets { get; set; }
        public StatusType status { get; set; }
        public int Level { get; set; }

    }
}
  
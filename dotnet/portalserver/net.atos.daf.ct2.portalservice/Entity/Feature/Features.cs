using System.Text.Json.Serialization;

namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class Features
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public char Type { get; set; }

        [JsonPropertyName("IsFeatureActive")]
        public bool Is_Active { get; set; }
        public DataAttributeSet DataattributeSet { get; set; }
        public string Key { get; set; }
        public int[] DataAttributeIds { get; set; }
        public int Level { get; set; }
        public string FeatureState { get; set; }
    }
}

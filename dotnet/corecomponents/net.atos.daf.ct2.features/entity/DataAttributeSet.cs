using System.Collections.Generic;

namespace net.atos.daf.ct2.features.entity
{
    public class DataAttributeSet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public char State { get; set; }
        public string Description { get; set; }
        //public string Is_exlusive { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public List<DataAttribute> DataAttributes { get; set; }
        public bool Is_exlusive { get; set; }
        public StatusType status { get; set; }
    }
}

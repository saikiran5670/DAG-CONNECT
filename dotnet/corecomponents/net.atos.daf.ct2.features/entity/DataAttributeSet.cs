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
        public long Created_at { get; set; }
        public int Created_by { get; set; }
        public long Modified_at { get; set; }
        public int Modified_by { get; set; }
        public List<DataAttribute> DataAttributes { get; set; }
        public bool Is_exlusive { get; set; }
        public StatusType Status { get; set; }
    }
}

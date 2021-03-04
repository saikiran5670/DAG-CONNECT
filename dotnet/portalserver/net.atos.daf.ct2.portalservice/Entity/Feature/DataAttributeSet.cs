using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class DataAttributeSet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public string Description { get; set; }
        //public string Is_exlusive { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public List<DataAttribute> DataAttributes { get; set; }
        public DataAttributeSetType Is_exlusive { get; set; }
        public StatusType status { get; set; }
    }

    public enum DataAttributeSetType
    {
        Inclusive = 'F',
        Exclusive = 'T'
    }
    public enum StatusType
    {
        Active = 'A',
        InActive = 'I'
    }
}

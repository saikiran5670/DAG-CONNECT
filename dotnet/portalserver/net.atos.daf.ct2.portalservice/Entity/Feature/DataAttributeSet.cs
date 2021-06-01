namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class DataAttributeSet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public bool is_Exclusive { get; set; }

        public string Description { get; set; }
        //public string Is_exlusive { get; set; }
        //public DataAttributeSetType Is_exlusive { get; set; }
        public StatusType status { get; set; }
    }

    public enum DataAttributeSetType
    {
        Inclusive = 'F',
        Exclusive = 'T'
    }
    public enum StatusType
    {
        Active = 0,
        Inactive = 1
    }
}

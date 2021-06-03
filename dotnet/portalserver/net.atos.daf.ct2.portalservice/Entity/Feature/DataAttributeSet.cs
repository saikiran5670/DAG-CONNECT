namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class DataAttributeSet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool Is_Exclusive { get; set; }
        public string Description { get; set; }
        public StatusType Status { get; set; }
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

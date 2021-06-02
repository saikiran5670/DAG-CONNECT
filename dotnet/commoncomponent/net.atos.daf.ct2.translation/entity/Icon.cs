namespace net.atos.daf.ct2.translation.entity
{
    public class Icon
    {
        public int Id { get; set; }
        public byte[] Iconn { get; set; }
        public string Type { get; set; }
        public int WarningClass { get; set; }
        public int WarningNumber { get; set; }
        public string Name { get; set; }
        public string ColorName { get; set; }
        public string State { get; set; }
        public long? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public long? ModifiedAt { get; set; }
        public int? ModifiedBy { get; set; }

    }
}

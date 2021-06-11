namespace net.atos.daf.ct2.poigeofence.entity
{
    public class Icon
    {
        public int Id { get; set; }
        public byte[] IconData { get; set; }
        public string Type { get; set; }
        public int Warning_class { get; set; }
        public int Warning_number { get; set; }
        public string Name { get; set; }
        public string Color_name { get; set; }
        public string State { get; set; }
        public long? Created_at { get; set; }
        public int? Created_by { get; set; }
        public long? Modified_at { get; set; }
        public int? Modified_by { get; set; }

    }
}

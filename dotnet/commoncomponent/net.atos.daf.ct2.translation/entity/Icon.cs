namespace net.atos.daf.ct2.translation.entity
{
    public class Icon
    {
        public int id { get; set; }
        public byte[] icon { get; set; }
        public string type { get; set; }
        public int warning_class { get; set; }
        public int warning_number { get; set; }
        public string name { get; set; }
        public string color_name { get; set; }
        public string state { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? modified_at { get; set; }
        public int? modified_by { get; set; }

    }
}

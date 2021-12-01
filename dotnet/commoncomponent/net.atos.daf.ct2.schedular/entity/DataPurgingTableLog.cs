namespace net.atos.daf.ct2.schedular.entity
{
    public class DataPurgingTableLog
    {

        public int Id { get; set; }

        public long PurgingStartTime { get; set; }
        public long PurgingEndTime { get; set; }
        public int NoOfDeletedRecords { get; set; }
        public long CreatedAt { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }

        public int Duration { get; set; }
        public string State { get; set; } //single char
    }
}

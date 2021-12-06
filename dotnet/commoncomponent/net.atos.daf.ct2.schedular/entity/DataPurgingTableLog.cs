namespace net.atos.daf.ct2.schedular.entity
{
    class DataPurgingTableLog
    {
        public int Id { get; set; }
        public int DataPurgingServiceLogId { get; set; }
        public int DataPurgingTableId { get; set; }
        public long PurgingStartTime { get; set; }
        public long PurgingEndTime { get; set; }
        public int NoOfDeletedRecords { get; set; }
        public long CreatedAt { get; set; }
    }
}

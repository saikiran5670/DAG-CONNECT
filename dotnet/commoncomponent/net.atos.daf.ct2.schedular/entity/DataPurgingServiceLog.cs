namespace net.atos.daf.ct2.schedular.entity
{
    class DataPurgingServiceLog
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public long ExecutionStartTime { get; set; }
        public long ExecutionEndTime { get; set; }
        public long CreatedAt { get; set; }
    }
}

namespace net.atos.daf.ct2.portalservice.Entity.Audit
{
    public class AuditTrailEnum
    {
        public enum Event_type
        {
            BULK = 'B',
            LOGIN = 'L',
            CREATE = 'C',
            UPDATE = 'U',
            DELETE = 'D',
            GET = 'G'
        }

        public enum Event_status
        {
            SUCCESS = 'S',
            FAILED = 'F',
            PENDING = 'P',
            ABORTED = 'A'
        }
    }
}

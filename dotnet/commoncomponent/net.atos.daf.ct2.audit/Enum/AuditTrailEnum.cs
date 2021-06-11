namespace net.atos.daf.ct2.audit.Enum
{
    public class AuditTrailEnum
    {
        public enum Event_type
        {
            LOGIN = 'L',
            CREATE = 'C',
            UPDATE = 'U',
            DELETE = 'D',
            GET = 'G',
            Mail = 'M'
        }

        public enum Event_status
        {
            SUCCESS = 'S',
            PARTIAL = 'P',
            FAILED = 'F',
            PENDING = 'P',
            ABORTED = 'A'
        }
    }
}

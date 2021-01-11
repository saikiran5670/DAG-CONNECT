using System;

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
            Get = 'G'
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

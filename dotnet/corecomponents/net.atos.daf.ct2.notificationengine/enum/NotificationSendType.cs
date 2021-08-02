using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine
{
    public enum NotificationSendType
    {
        Successful = 'S',
        Failed = 'F',
        Queue = 'Q',
        Pending = 'P'
    }
    public enum SMSStatus
    {
        queued = 'Q',
        sending = 'P',
        sent = 'S',
        failed = 'F',
        delivered = 'D',
        undelivered = 'U',
        receiving = 'R',
        received = 'C',
        accepted = 'A',
        scheduled = 'H',
        read = 'E',
        partiallyDelivered = 'T',
        canceled = 'N'
    }
}

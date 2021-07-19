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
}

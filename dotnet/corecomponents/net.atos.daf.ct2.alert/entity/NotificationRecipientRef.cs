using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationRecipientRef
    {
        public int Id { get; set; }
        public int AlertId { get; set; }
        public int NotificationId { get; set; }
        public int RecipientId { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
}

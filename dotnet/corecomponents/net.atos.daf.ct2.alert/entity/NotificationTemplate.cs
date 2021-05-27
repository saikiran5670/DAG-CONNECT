using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationTemplate
    {
        public int Id { get; set; }
        public string AlertCategoryType { get; set; }
        public string AlertType { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }

    }
}

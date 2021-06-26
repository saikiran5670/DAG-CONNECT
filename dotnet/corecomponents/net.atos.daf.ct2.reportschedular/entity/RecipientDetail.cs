using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class RecipientDetail
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public int? Organization_Id { get; set; }
        public int? PreferenceId { get; set; }

    }
}

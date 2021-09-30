using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class TripAlertOtaConfigParam
    {
        public int Id { get; set; }
        public int Trip_Alert_Id { get; set; }
        public string Vin { get; set; }
        public string Compaign { get; set; }
        public string Baseline { get; set; }
        public string Status_Code { get; set; }
        public string Status { get; set; }
        public string Comapign_Id { get; set; }
        public string Subject { get; set; }
        public long Time_stamp { get; set; }
    }
}

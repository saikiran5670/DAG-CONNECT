using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class TripAlertOtaConfigParam
    {
        public int Id { get; set; }
        public int TripAlertId { get; set; }
        public string Vin { get; set; }
        public string Campaign { get; set; }
        public string Baseline { get; set; }
        public int StatusCode { get; set; }
        //public enum StatusCode
        //{
        //    UPDATE_FAILURE = 8,
        //    WAITING_FOR_MANAGER_APPROVAL = 10
        //}
        public string Status { get; set; }
        public string CampaignId { get; set; }
        public string Subject { get; set; }
        public long TimeStamp { get; set; }
    }
}

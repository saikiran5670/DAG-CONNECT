using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class NotificationHistory
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string TripId { get; set; }
        public int VehicleId { get; set; }
        public int AlertId { get; set; }
        public int NotificationId { get; set; }
        public int RecipientId { get; set; }
        public string NotificationModeType { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string WsUrl { get; set; }
        public long NotificationSendDate { get; set; }
        public string Status { get; set; }
        public string EmailSub { get; set; }
        public string EmailText { get; set; }
    }
}

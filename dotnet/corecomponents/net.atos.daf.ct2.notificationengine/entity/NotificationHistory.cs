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
        public string WsAuthType { get; set; }
        public string WsText { get; set; }
        public string WsLogin { get; set; }
        public string WsPassword { get; set; }
        public string AlertCategoryKey { get; set; }
        public string AlertCategoryEnum { get; set; }
        public string AlertTypeKey { get; set; }
        public string AlertTypeEnum { get; set; }
        public string UrgencyTypeKey { get; set; }
        public string UrgencyTypeEnum { get; set; }
        public double ThresholdValue { get; set; }
        public string ThresholdValueUnitType { get; set; }
        public double ValueAtAlertTime { get; set; }
        public string SMS { get; set; }
    }
}

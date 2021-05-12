using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationRecipient
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string RecipientLabel { get; set; }
        public int AccountGroupId { get; set; }
        public string NotificationModeType { get; set; }
        public string PhoneNo { get; set; }
        public string Sms { get; set; }
        public string EmailId { get; set; }
        public string EmailSub { get; set; }
        public string EmailText { get; set; }
        public string WsUrl { get; set; }
        public string WsType { get; set; }
        public string WsText { get; set; }
        public string WsLogin { get; set; }
        public string WsPassword { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
    }
}

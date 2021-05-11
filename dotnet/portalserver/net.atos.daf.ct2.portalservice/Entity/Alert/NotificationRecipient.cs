using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class NotificationRecipient
    {
        //public int Id { get; set; }
        //public int NotificationId { get; set; }
        [StringLength(50, MinimumLength = 0,ErrorMessage = "Notification recipient Label should be between 1 and 50 characters")]
        public string RecipientLabel { get; set; }
        public int AccountGroupId { get; set; }
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Notification mode type should be 1 character")]
        public string NotificationModeType { get; set; }
        [StringLength(100, MinimumLength = 0,ErrorMessage = "Notification PhoneNo should be between 1 and 100 characters")]
        public string PhoneNo { get; set; }
        public string Sms { get; set; }
        [StringLength(250, MinimumLength = 0,ErrorMessage = "Notification EmailId should be between 1 and 250 characters")]
        public string EmailId { get; set; }
        [StringLength(250, MinimumLength = 0,ErrorMessage = "Notification email subject should be between 1 and 250 characters")]
        public string EmailSub { get; set; }
        public string EmailText { get; set; }
        [StringLength(250, MinimumLength = 0,ErrorMessage = "Notification web service url should be between 1 and 250 characters")]
        public string WsUrl { get; set; }
        [StringLength(1, MinimumLength = 0,ErrorMessage = "Notification web service type should be 1 character")]
        public string WsType { get; set; }
        public string WsText { get; set; }
        [StringLength(50, MinimumLength = 0,ErrorMessage = "Notification web service login should be between 1 and 50 characters")]
        public string WsLogin { get; set; }
        [StringLength(50, MinimumLength = 0,ErrorMessage = "Notification web service password should be between 1 and 50 characters")]
        public string WsPassword { get; set; }
        //public string State { get; set; }
        //public long CreatedAt { get; set; }
        //public long ModifiedAt { get; set; }
    }
    public class NotificationRecipientEdit: NotificationRecipient
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        //public string State { get; set; }
        //public long ModifiedAt { get; set; }
    }
}

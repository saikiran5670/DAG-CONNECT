using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notification.entity
{
    public class NotificationAccount
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public int? Organization_Id { get; set; }
        public string FullName
        {
            get
            {
                return $"{Salutation} {FirstName} {LastName}";
            }
        }
    }
}

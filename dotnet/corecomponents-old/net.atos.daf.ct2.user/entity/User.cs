using System;

namespace net.atos.daf.ct2.user.entity
{
    public class User:TableLog
    {
        public int UserID { get; set; }
        public int OrganizationId { get; set; }
        public string UserName   { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime dob { get; set; }
        public string Salutation { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int LoginAttempt { get; set; }
        public int IsLocked{ get; set; }
        public int ParentUserId { get; set; }
        public int UserTypeid { get; set; }
        public int Updatedby { get; set; }
        public int CreateBy { get; set; }
    }
}

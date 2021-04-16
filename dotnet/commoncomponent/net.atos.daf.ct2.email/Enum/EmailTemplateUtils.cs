using System;
using System.ComponentModel;

namespace net.atos.daf.ct2.email.Enum
{
    public static class MimeType
    {
        public const string Text = "text/plain";
        public const string Html = "text/html";
    }

    public enum EmailContentType
    {
        Html = 'H',
        Text = 'T',
        Csv = 'C'
    }

    public enum EmailEventType
    {
        //[Subject("Welcome to DAF Connect!")]
        CreateAccount = 1,

        //[Subject("You have got yourself a new password")]
        ChangeResetPasswordSuccess = 2,

        //[Subject("Reset your DAF Connect password")]
        ResetPassword = 3,

        PasswordExpiryNotification = 4
    }

    //public class EmailTemplateIdAttribute : Attribute
    //{
    //    public int Id { get; private set; }

    //    public EmailTemplateIdAttribute(int id)
    //    {
    //        this.Id = id;
    //    }
    //}
}

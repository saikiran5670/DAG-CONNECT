using System;
using System.ComponentModel;

namespace net.atos.daf.ct2.email.Enum
{
    public static class MimeType
    {
        public const string Text = "text/plain";
        public const string Html = "text/html";
    }

    public enum EmailTemplateType
    {
        [MimeType(MimeType.Text)]
        [Subject("Welcome to DAF Connect!")]
        CreateAccount = 1,

        [MimeType(MimeType.Text)]
        [Subject("You have got yourself a new password")]
        ChangeResetPasswordSuccess = 2,

        [MimeType(MimeType.Text)]
        [Subject("Reset your DAF Connect password")]
        ResetPassword = 3
    }

    public class MimeTypeAttribute : Attribute
    {
        public string Name { get; private set; }

        public MimeTypeAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class SubjectAttribute : Attribute
    {
        public string Title { get; private set; }

        public SubjectAttribute(string title)
        {
            this.Title = title;
        }
    }
}

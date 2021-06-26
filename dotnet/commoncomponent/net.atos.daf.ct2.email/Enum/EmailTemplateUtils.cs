namespace net.atos.daf.ct2.email.Enum
{
    public enum EmailContentType
    {
        Html = 'H',
        Text = 'T',
        Csv = 'C'
    }

    public enum EmailEventType
    {
        CreateAccount = 1,
        ChangeResetPasswordSuccess = 2,
        ResetPassword = 3,
        PasswordExpiryNotification = 4,
        SendReport = 5
    }
}

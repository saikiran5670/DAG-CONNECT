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
        ScheduledReportEmail = 5,
        //Below EventTypes are for Report Creation
        TripReport = 6
        //End
    }
}

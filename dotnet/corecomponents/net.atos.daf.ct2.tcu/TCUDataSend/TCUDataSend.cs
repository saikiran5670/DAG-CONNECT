namespace TCUSend
{
    public class TCUDataSend
    {
        public TCUDataSend(TCURegistrationEvents tCURegistrationEvents)
        {
            TCURegistrationEvents = tCURegistrationEvents;
        }

        public TCURegistrationEvents TCURegistrationEvents { get;  }
    }
}

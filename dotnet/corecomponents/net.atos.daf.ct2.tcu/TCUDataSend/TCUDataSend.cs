namespace TCUSend
{
    public class TCUDataSend
    {

        private TCURegistrationEvents tCURegistrationEvents;

        public TCUDataSend(TCURegistrationEvents tCURegistrationEvents)
        {
            this.tCURegistrationEvents = tCURegistrationEvents;
        }

        public TCURegistrationEvents TCURegistrationEvents { get => tCURegistrationEvents; set => tCURegistrationEvents = value; }
    }
}

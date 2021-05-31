namespace net.atos.daf.ct2.tcucore
{
    public class TCUDataSend
    {
        private TCURegistrationEvents tCURegistrationEvents;

        public TCUDataSend(TCURegistrationEvents _tCURegistrationEvents)
        {
            this.tCURegistrationEvents = _tCURegistrationEvents;
        }

        public TCURegistrationEvents TCURegistrationEvents
        {
            get => tCURegistrationEvents;
            set => tCURegistrationEvents = value;
        }
    }
}

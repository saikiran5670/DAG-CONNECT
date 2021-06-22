namespace net.atos.daf.ct2.tcucore
{
    public class TCUDataSend
    {
        private TCURegistrationEvents _tCURegistrationEvents;

        public TCUDataSend(TCURegistrationEvents tCURegistrationEvents)
        {
            this._tCURegistrationEvents = tCURegistrationEvents;
        }

        public TCURegistrationEvents TCURegistrationEvents
        {
            get => _tCURegistrationEvents;
            set => _tCURegistrationEvents = value;
        }
    }
}

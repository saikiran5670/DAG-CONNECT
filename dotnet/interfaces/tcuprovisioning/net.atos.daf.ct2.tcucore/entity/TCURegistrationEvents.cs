using System.Collections.Generic;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvents
    {
        private List<TCURegistrationEvent> tCURegistrationEvent;

        public TCURegistrationEvents(List<TCURegistrationEvent> _tcuRegistrationEvent)
        {
            this.tCURegistrationEvent = _tcuRegistrationEvent;
        }

        public List<TCURegistrationEvent> TCURegistrationEvent
        {
            get => tCURegistrationEvent;
            set => tCURegistrationEvent = value;
        }
    }
}

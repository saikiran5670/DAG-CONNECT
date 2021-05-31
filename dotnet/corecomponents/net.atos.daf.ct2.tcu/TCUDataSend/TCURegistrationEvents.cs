using System.Collections.Generic;

namespace TCUSend
{
    public class TCURegistrationEvents
    {
        private List<TCURegistrationEvent> tCURegistrationEvent;

        public TCURegistrationEvents(List<TCURegistrationEvent> tcuRegistrationEvent)
        {
            this.tCURegistrationEvent = tcuRegistrationEvent;
        }

        public List<TCURegistrationEvent> TCURegistrationEvent { get => tCURegistrationEvent; set => tCURegistrationEvent = value; }
    }
}

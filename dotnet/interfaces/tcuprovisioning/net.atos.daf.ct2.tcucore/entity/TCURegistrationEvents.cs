using System.Collections.Generic;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvents
    {
        public TCURegistrationEvents(List<TCURegistrationEvent> tcuRegistrationEvent)
        {
            TCURegistrationEvent = tcuRegistrationEvent;
        }

        public List<TCURegistrationEvent> TCURegistrationEvent { get; }
    }
}

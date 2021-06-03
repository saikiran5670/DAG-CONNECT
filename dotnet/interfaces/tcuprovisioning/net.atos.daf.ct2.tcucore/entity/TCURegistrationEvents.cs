using System.Collections.Generic;

namespace net.atos.daf.ct2.tcucore
{
    public class TCURegistrationEvents
    {
        public TCURegistrationEvents(List<TCURegistrationEvent> _tcuRegistrationEvent)
        {
            TCURegistrationEvent = _tcuRegistrationEvent;
        }

        public List<TCURegistrationEvent> TCURegistrationEvent { get; }
    }
}

using System.Collections.Generic;

namespace TCUSend
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

using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    public class TCURegistrationEvents
    {
        private List<TCURegistrationEvent> tcuRegistrationEvent;

        public TCURegistrationEvents(List<TCURegistrationEvent> tcuRegistrationEvent)
        {
            this.tcuRegistrationEvent = tcuRegistrationEvent;
        }

        public List<TCURegistrationEvent> TcuRegistrationEvent { get => tcuRegistrationEvent; set => tcuRegistrationEvent = value; }
    }
}

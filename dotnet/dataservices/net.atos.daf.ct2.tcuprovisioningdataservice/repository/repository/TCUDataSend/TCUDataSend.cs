using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    public class TCUDataSend
    {

        private TCURegistrationEvents tcuRegistrationEvents;

        public TCUDataSend(TCURegistrationEvents tcuRegistrationEvents)
        {
            this.tcuRegistrationEvents = tcuRegistrationEvents;
        }

        public TCURegistrationEvents TcuRegistrationEvents { get => tcuRegistrationEvents; set => tcuRegistrationEvents = value; }




    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.rfms.entity
{
    public class RfmsBaseRequest
    {
        public string RequestId { get; set; }
        public int ThresholdValue { get; set; }
        public int AccountId { get; set; }
        public int OrgId { get; set; }
    }
}

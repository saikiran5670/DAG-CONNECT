using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.sms.entity;

namespace net.atos.daf.ct2.sms
{
    public interface ISMSManager
    {
        Task<string> SendSMS(SMS smsdetail);
    }
}

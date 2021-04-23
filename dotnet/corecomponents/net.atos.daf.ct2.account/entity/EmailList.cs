using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.account.entity
{
    public class EmailList
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public bool IsSend { get; set; }
    }
}

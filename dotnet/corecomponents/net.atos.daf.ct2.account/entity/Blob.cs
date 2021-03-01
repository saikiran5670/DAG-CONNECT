using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountBlob
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public ImageType Type { get; set; }
        public byte[] Image { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Identity
{
    public class Account
    {
        public int id { get; set; }
        public string emailId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int organization_Id { get; set; }
        public string salutation { get; set; }
        public int? preferenceId { get; set; }
        public int? blobId { get; set; }
    }
}

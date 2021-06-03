using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.account.entity
{
    public class AccountRole
    {

        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public List<int> RoleIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;

namespace net.atos.daf.ct2.organization
{
    public interface IOrganizationManager
    {
         Task<Organization> Create(Organization organization);
         Task<Organization> Update(Organization group);
         Task<bool> Delete(int organizationId);
         Task<IEnumerable<Organization>> Get(int organizationId);       
    }
}

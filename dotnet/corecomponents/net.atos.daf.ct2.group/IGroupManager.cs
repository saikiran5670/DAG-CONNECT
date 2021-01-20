using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group
{
    public interface IGroupManager
    {
            Task<Group> Create(Group group);
            Task<Group> Update(Group group);
            Task<bool> Delete(long groupid);
            Task<IEnumerable<Group>> Get(GroupFilter groupFilter);
            Task<bool> UpdateRef(Group group);
            Task<List<GroupRef>> GetRef(int groupid);
            Task<bool> AddRefToGroups(Group group);
        
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.group
{
    public interface IGroupRepository
    {
        Task<Group> Create(Group group);
        Task<Group> Update(Group group);
        Task<VehicleGroupDelete> Delete(long groupId, ObjectType objectType);
        Task<bool> CanDelete(long groupid, ObjectType objectType);
        Task<List<Group>> Get(GroupFilter groupFilter);
        Task<bool> AddRefToGroups(List<GroupRef> groupRef);
        Task<bool> UpdateRef(Group group);
        Task<List<GroupRef>> GetRef(int groupid);
        Task<bool> RemoveRef(int groupid);
        Task<bool> RemoveRefByRefId(int refId);
        Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter);
        Task<IEnumerable<int>> GetGroupVehicleCount(int groupId, int organizationId);
        Task<IEnumerable<int>> GetDynamicVehicleCount(int organizationId, FunctionEnum functionEnum);
        Task<List<GroupRef>> GetGroupRef(int groupid, int organizationId);
    }
}

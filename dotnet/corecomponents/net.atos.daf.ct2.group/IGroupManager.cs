using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.group
{
    public interface IGroupManager
    {
        Task<Group> Create(Group group);
        Task<Group> Update(Group group);
        Task<VehicleGroupDelete> Delete(long groupId, ObjectType objectType);
        Task<bool> CanDelete(long groupId, ObjectType objectType);
        Task<IEnumerable<Group>> Get(GroupFilter groupFilter);
        Task<bool> UpdateRef(Group group);
        Task<List<GroupRef>> GetRef(int groupid);
        Task<bool> AddRefToGroups(List<GroupRef> groupRef);
        Task<bool> RemoveRef(int groupid);
        Task<bool> RemoveRefByRefId(int refId);
        Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter);
        Task<int> GetVehiclesCount(int[] groupIds, int organizationId);
        Task<List<GroupRef>> GetGroupRef(int groupid, int organizationId);


    }
}

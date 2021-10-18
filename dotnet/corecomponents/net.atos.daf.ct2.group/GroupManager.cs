using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group
{
    public class GroupManager : IGroupManager
    {
        readonly IGroupRepository _groupRepository;
        readonly IAuditTraillib _auditlog;
        public GroupManager(IGroupRepository groupRepository, IAuditTraillib auditlog)
        {
            this._groupRepository = groupRepository;
            this._auditlog = auditlog;
        }
        public async Task<Group> Create(Group group)
        {
            return await _groupRepository.Create(group);
        }
        public async Task<Group> Update(Group group)
        {
            return await _groupRepository.Update(group);
        }
        public async Task<bool> CanDelete(long groupId, ObjectType objectType)
        {
            return await _groupRepository.CanDelete(groupId, objectType);
        }
        public async Task<VehicleGroupDelete> Delete(long groupId, ObjectType objectType)
        {
            return await _groupRepository.Delete(groupId, objectType);
        }
        public async Task<IEnumerable<Group>> Get(GroupFilter groupFilter)
        {
            return await _groupRepository.Get(groupFilter);
        }
        public async Task<bool> UpdateRef(Group group)
        {
            return await _groupRepository.UpdateRef(group);
        }
        public async Task<List<GroupRef>> GetRef(int groupid)
        {
            return await _groupRepository.GetRef(groupid);
        }
        public async Task<bool> AddRefToGroups(List<GroupRef> groupRef)
        {
            return await _groupRepository.AddRefToGroups(groupRef);
        }
        public async Task<bool> RemoveRef(int groupid)
        {
            return await _groupRepository.RemoveRef(groupid);
        }
        public async Task<bool> RemoveRefByRefId(int refId)
        {
            return await _groupRepository.RemoveRefByRefId(refId);
        }

        public async Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter)
        {
            return await _groupRepository.GetVehicleGroupWithVehCount(groupFilter);
        }

        public async Task<int> GetVehiclesCount(int[] groupIds, int organizationId)
        {
            List<int> vehicleIds = new List<int>();
            var groupFilter = new GroupFilter();
            groupFilter.GroupRefCount = false;
            groupFilter.GroupRef = false;
            groupFilter.ObjectType = ObjectType.VehicleGroup;
            groupFilter.GroupType = GroupType.None;
            groupFilter.FunctionEnum = FunctionEnum.None;

            foreach (var groupId in groupIds)
            {
                groupFilter.Id = groupId;
                var group = await _groupRepository.Get(groupFilter);
                var groupType = group.FirstOrDefault()?.GroupType;
                var functionEnum = group.FirstOrDefault()?.FunctionEnum ?? FunctionEnum.None;
                switch (group.FirstOrDefault()?.GroupType)
                {
                    case GroupType.Group:
                        vehicleIds.AddRange(await _groupRepository.GetGroupVehicleCount(groupId, organizationId));
                        break;
                    case GroupType.Dynamic:
                        vehicleIds.AddRange(await _groupRepository.GetDynamicVehicleCount(organizationId, functionEnum));
                        break;
                    default:
                        break;
                }
            }
            return vehicleIds.Distinct().Count();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group
{
    public class GroupManager : IGroupManager
    {
        IGroupRepository gropRepository;
        IAuditTraillib auditlog;

        public GroupManager(IGroupRepository _gropRepository, IAuditTraillib _auditlog)
        {
            gropRepository = _gropRepository;
            auditlog = _auditlog;
        }

        public async Task<Group> Create(Group group)
        {
            return await gropRepository.Create(group);
        }
        public async Task<Group> Update(Group group)
        {
            return await gropRepository.Update(group);
        }
        public async Task<bool> Delete(long groupid, ObjectType objectType)
        {
            return await gropRepository.Delete(groupid, objectType);
        }
        public async Task<IEnumerable<Group>> Get(GroupFilter groupFilter)
        {
            // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.GET,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.Get,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            //await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.Get,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            return await gropRepository.Get(groupFilter);
        }
        public async Task<bool> UpdateRef(Group group)
        {
            return await gropRepository.UpdateRef(group);
        }
        public async Task<List<GroupRef>> GetRef(int groupid)
        {
            return await gropRepository.GetRef(groupid);
        }
        public async Task<bool> AddRefToGroups(List<GroupRef> groupRef)
        {
            return await gropRepository.AddRefToGroups(groupRef);
        }
        public async Task<bool> RemoveRef(int groupid)
        {
            return await gropRepository.RemoveRef(groupid);
        }
        public async Task<bool> RemoveRefByRefId(int refId)
        {
            return await gropRepository.RemoveRefByRefId(refId);
        }

        public async Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter)
        {
            return await gropRepository.GetVehicleGroupWithVehCount(groupFilter);
        }

    }
}

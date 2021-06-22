using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group
{
    public class GroupManager : IGroupManager
    {
        readonly IGroupRepository _gropRepository;
        readonly IAuditTraillib _auditlog;
        public GroupManager(IGroupRepository gropRepository, IAuditTraillib auditlog)
        {
            this._gropRepository = gropRepository;
            this._auditlog = auditlog;
        }
        public async Task<Group> Create(Group group)
        {
            return await _gropRepository.Create(group);
        }
        public async Task<Group> Update(Group group)
        {
            return await _gropRepository.Update(group);
        }
        public async Task<bool> Delete(long groupid, ObjectType objectType)
        {
            return await _gropRepository.Delete(groupid, objectType);
        }
        public async Task<IEnumerable<Group>> Get(GroupFilter groupFilter)
        {
            // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.GET,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.Get,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            //await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Group Component","Group Service",AuditTrailEnum.Event_type.Get,AuditTrailEnum.Event_status.SUCCESS,"Test",1,2,null);
            return await _gropRepository.Get(groupFilter);
        }
        public async Task<bool> UpdateRef(Group group)
        {
            return await _gropRepository.UpdateRef(group);
        }
        public async Task<List<GroupRef>> GetRef(int groupid)
        {
            return await _gropRepository.GetRef(groupid);
        }
        public async Task<bool> AddRefToGroups(List<GroupRef> groupRef)
        {
            return await _gropRepository.AddRefToGroups(groupRef);
        }
        public async Task<bool> RemoveRef(int groupid)
        {
            return await _gropRepository.RemoveRef(groupid);
        }
        public async Task<bool> RemoveRefByRefId(int refId)
        {
            return await _gropRepository.RemoveRefByRefId(refId);
        }

        public async Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter)
        {
            return await _gropRepository.GetVehicleGroupWithVehCount(groupFilter);
        }

    }
}

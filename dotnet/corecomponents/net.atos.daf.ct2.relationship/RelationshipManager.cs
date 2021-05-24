using net.atos.daf.ct2.relationship.entity;
using net.atos.daf.ct2.relationship.repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.relationship
{
    public class RelationshipManager : IRelationshipManager
    {

       private readonly IRelationshipRepository _relationshipRepository;
        
        public RelationshipManager(IRelationshipRepository relationshipRepository )
        {
            _relationshipRepository = relationshipRepository;
            
        }


        public async Task<Relationship> CreateRelationship(Relationship orgRelationship)
        {
            return await _relationshipRepository.CreateRelationship(orgRelationship);
        }

        public async Task<Relationship> UpdateRelationship(Relationship orgRelationship)
        {
            return await _relationshipRepository.UpdateRelationship(orgRelationship);
        }

        public async Task<bool> DeleteRelationship(int orgRelationshipId)
        {
            return await _relationshipRepository.DeleteRelationship(orgRelationshipId);
        }

        public async Task<List<Relationship>> GetRelationship(RelationshipFilter orgRelationship)
        {
            return await _relationshipRepository.GetRelationship(orgRelationship);
        }
        public async Task<RelationshipLevelCode> GetRelationshipLevelCode()
        {
            return await _relationshipRepository.GetRelationshipLevelCode();
        }

        public async Task<int> CreateRelationShipMapping(OrganizationRelationShip relationshipMapping)
        {
            return await _relationshipRepository.CreateRelationShipMapping(relationshipMapping);
        }

        public async Task<int> EndRelationShipMapping(int OrgRelationId)
        {
            return await _relationshipRepository.EndRelationShipMapping(OrgRelationId);
        }

        public async Task<IEnumerable<OrganizationRelationShip>> GetOrgRelationships(int OrganizationID)
        {
            return await _relationshipRepository.GetOrgRelationships(OrganizationID);
        }

        public async Task<int> AllowChaining(int OrgRelationId, bool AllowChaining)
        {
            return await _relationshipRepository.AllowChaining(OrgRelationId, AllowChaining);
        }

        public async Task<List<OrganizationRelationShip>> GetRelationshipMapping(OrganizationRelationShip orgMapfilter)
        {
            return await _relationshipRepository.GetRelationshipMapping(orgMapfilter);
        }
    }
}

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

        public async Task<List<Relationship>> GetRelationship(Relationship orgRelationship)
        {
            return await _relationshipRepository.GetRelationship(orgRelationship);
        }
    }
}

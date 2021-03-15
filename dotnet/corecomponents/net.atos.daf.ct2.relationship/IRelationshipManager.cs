using net.atos.daf.ct2.relationship.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.relationship
{
    internal interface IRelationshipManager
    {
        Task<Relationship> CreateRelationship(Relationship relationship);
        Task<Relationship> UpdateRelationship(Relationship relationship);
        Task<bool> DeleteRelationship(int relationshipId);
        Task<List<Relationship>> GetRelationship(Relationship filter);
    }
}
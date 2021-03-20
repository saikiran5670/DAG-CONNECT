﻿using net.atos.daf.ct2.relationship.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.relationship
{
    public interface IRelationshipManager
    {
        Task<Relationship> CreateRelationship(Relationship relationship);
        Task<Relationship> UpdateRelationship(Relationship relationship);
        Task<bool> DeleteRelationship(int relationshipId);
        Task<List<Relationship>> GetRelationship(Relationship filter);
        Task<RelationshipLevelCode> GetRelationshipLevelCode();
        Task<int> CreateRelationShipMapping(OrganizationRelationShip relationshipMapping);
        Task<int> EndRelationShipMapping(int OrgRelationId);
        Task<int> AllowChaining(int OrgRelationId, bool AllowChaining);
        Task<List<OrganizationRelationShip>> GetRelationshipMapping(OrganizationRelationShip filter);
    }
}
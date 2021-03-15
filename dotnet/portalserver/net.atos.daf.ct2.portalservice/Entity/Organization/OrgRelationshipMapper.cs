﻿using net.atos.daf.ct2.organizationservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class OrgRelationshipMapper
    {
        internal RelationshipCreateRequest ToOrgRelationshipRequest(RelationshipPortalRequest request)
        {
            var orgRelationshipRequest = new RelationshipCreateRequest()
            {
                Id = request.Id,
                Code = request.Code,
                Description = request.Description,
                Featuresetid = request.FeaturesetId,
                IsActive = request.IsActive,
                Level = request.Level,
                Name = request.Name,
                OrganizationId = request.OrganizationId
            };
            return orgRelationshipRequest;
        }

        internal object ToGetOrgRelationshipRequest(RelationshipPortalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

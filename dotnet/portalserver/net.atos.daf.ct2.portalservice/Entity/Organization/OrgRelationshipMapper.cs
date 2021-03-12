using net.atos.daf.ct2.organizationservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class OrgRelationshipMapper
    {
        internal OrgRelationshipCreateRequest ToOrgRelationshipRequest(OrgRelationshipPortalRequest request)
        {
            var orgRelationshipRequest = new OrgRelationshipCreateRequest()
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

        internal object ToGetOrgRelationshipRequest(OrgRelationshipPortalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

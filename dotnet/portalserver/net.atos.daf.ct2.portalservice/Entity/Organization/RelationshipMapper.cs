using System;
using net.atos.daf.ct2.organizationservice;
namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
    public class RelationshipMapper
    {
        internal RelationshipCreateRequest ToRelationshipRequest(RelationshipPortalRequest request)
        {
            var orgRelationshipRequest = new RelationshipCreateRequest()
            {
                Id = request.Id,
                Code = request.Code,
                Description = request.Description,
                Featuresetid = request.FeaturesetId,
                State = request.State,
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

        //public dynamic MaprelationData(List<RelationshipPortalRequest> request, VehicleBusinessService.VehicleGroupDetailsResponse VehicleResponce, List<OrgGetResponse> Organizations)
        //{

        //}
    }
}

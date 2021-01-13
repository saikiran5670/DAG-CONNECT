using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization.repository;
using net.atos.daf.ct2.organization;

namespace net.atos.daf.ct2.organizationservice.Services
{
    public class OrganizationManagementService : OrganizationService.OrganizationServiceBase
    {
          private readonly ILogger<GreeterService> _logger;
        private readonly IOrganizationManager organizationtmanager;
        public OrganizationManagementService(ILogger<GreeterService> logger, IOrganizationManager _organizationmanager)
        {
            _logger = logger;
            organizationtmanager = _organizationmanager;
        }

        public override Task<OrganizationResponse> Create(OrganizationRequest request, ServerCallContext context)
        {
            try
            {
                Organization organization = new Organization();
                organization.OrganizationId = request.OrganizationId;
                organization.Type = request.Type;
                organization.Name = request.Name;
                organization.AddressType = request.AddressType;
                organization.AddressStreet = request.AddressStreet;                
                organization.AddressStreetNumber = request.AddressStreetNumber;
                organization.PostalCode = request.PostalCode;
                organization.City = request.City;
                organization.CountryCode = request.CountryCode;
                organization.ReferencedDate = request.ReferencedDate;
                organization.OptOutStatus = request.OptOutStatus;
                organization.OptOutStatusChangedDate = request.OptOutStatusChangedDate;
                organization.IsActive = request.IsActive;
                
                organization = organizationtmanager.Create(organization).Result;

                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Organization Created " + organization.OrganizationId
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using Preference = net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.vehicle;
using AccountComponent = net.atos.daf.ct2.account;
using Grpc.Core;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.organizationservice.entity;
using System.Linq;
namespace net.atos.daf.ct2.organizationservice
{
    public class OrganizationManagementService : OrganizationService.OrganizationServiceBase
    {

        private readonly ILogger _logger;
        private readonly IAuditTraillib _AuditTrail;
        private readonly IAuditTraillib auditlog;
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;
        private readonly EntityMapper _mapper;


        public OrganizationManagementService(ILogger<OrganizationManagementService> logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager, IPreferenceManager _preferencemanager, IVehicleManager _vehicleManager, IAuditTraillib _auditlog)
        {
            _logger = logger;
            _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager = _preferencemanager;
            vehicleManager = _vehicleManager;
            auditlog = _auditlog;
            _mapper = new EntityMapper();
        }


        //Org Relationship

        public override async Task<OrgRelationshipCreateResponse> CreateOrgRelationship(OrgRelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var orgRelationship = new OrgRelationship();
                var response = new OrgRelationshipCreateResponse();
                orgRelationship.OrganizationId = request.OrganizationId;
                orgRelationship.Code = request.Code;
                orgRelationship.Name = request.Name;
                orgRelationship.Level = request.Level;
                orgRelationship.FeaturesetId = request.Featuresetid;
                orgRelationship.Description = request.Description;
                orgRelationship.IsActive = request.IsActive;

                orgRelationship = await organizationtmanager.CreateOrgRelationship(orgRelationship);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Relationship Component", "Organization Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Create", 1, 2, orgRelationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Created";
                request.Id = orgRelationship.Id;
                response.OrgRelation = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Orgganization Relationship Service: Create : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new OrgRelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Relationship Creation failed due to - " + ex.Message,
                    OrgRelation = null
                });
            }
        }

        public override async Task<OrgRelationshipCreateResponse> UpdateOrgRelationship(OrgRelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var orgRelationship = new OrgRelationship();
                var response = new OrgRelationshipCreateResponse();
                orgRelationship.Id = request.Id;
                orgRelationship.OrganizationId = request.OrganizationId;
                orgRelationship.Code = request.Code;
                orgRelationship.Name = request.Name;
                orgRelationship.Level = request.Level;
                orgRelationship.FeaturesetId = request.Featuresetid;
                orgRelationship.Description = request.Description;
                orgRelationship.IsActive = request.IsActive;

                orgRelationship = await organizationtmanager.UpdateOrgRelationship(orgRelationship);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Relationship Component", "Organization Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Updated", 1, 2, orgRelationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Org relatioship Updated Successfully";
                request.Id = orgRelationship.Id;
                response.OrgRelation = request;

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Orgganization Relationship Service: Update : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new OrgRelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Relationship Updation failed due to - " + ex.Message,
                    OrgRelation = null
                });
            }
        }

        public async override Task<OrgRelationshipGetResponse> GetOrgRelationship(OrgRelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var response = new OrgRelationshipGetResponse();
                var orgRelationshipFilter = new OrgRelationship();
                orgRelationshipFilter.Id = request.Id;
                orgRelationshipFilter.OrganizationId = request.OrganizationId;
                orgRelationshipFilter.Code = request.Code;
                orgRelationshipFilter.FeaturesetId = request.Featuresetid;
                orgRelationshipFilter.Level = request.Level;
                orgRelationshipFilter.Name = request.Name;
                orgRelationshipFilter.Description = request.Description;
                orgRelationshipFilter.IsActive = request.IsActive;
                var orgRelationships = organizationtmanager.GetOrgRelationship(orgRelationshipFilter).Result;
                response.OrgRelationshipList.AddRange(orgRelationships
                                     .Select(x => new OrgRelationshipGetRequest()
                                     {
                                         Id = x.Id,
                                         OrganizationId = x.OrganizationId,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         Featuresetid = x.FeaturesetId,
                                         Level = x.Level,
                                         IsActive = x.IsActive
                                     }).ToList());
                _logger.LogInformation("Get org relationship details.");
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in  org relationship service:get  org relationship  details with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new OrgRelationshipGetResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }


        public async override Task<OrgRelationshipDeleteResponse> DeleteOrgRelationship(OrgRelationshipDeleteRequest request, ServerCallContext context)
        {
            try
            {
                var result = organizationtmanager.DeleteOrgRelationship(request.Id).Result;
                var response = new OrgRelationshipDeleteResponse();
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "org relationship deleted.";
                }
                else
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Org relationship Not Found.";
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in org relationship service:delete org relationship  with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new OrgRelationshipDeleteResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Org relationship Deletion Faile due to - " + ex.Message,

                });
            }
        }


        //Organization

        public override async Task<OrganizationCreateData> Create(OrgCreateRequest request, ServerCallContext context)
        {
            try
            {
                Organization organization = new Organization();
                OrganizationCreateData response = new OrganizationCreateData();
                organization.OrganizationId = request.OrgId;
                organization.Type = request.Type;
                organization.Name = request.Name;
                organization.AddressType = request.AddressType;
                organization.AddressStreet = request.Street;
                organization.AddressStreetNumber = request.StreetNumber;
                organization.City = request.City;
                organization.CountryCode = request.CountryCode;
                organization.reference_date = Convert.ToDateTime(request.ReferenceDate);
                organization = await organizationtmanager.Create(organization);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Organization Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Organization Create", 1, 2, organization.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Created";
                request.Id = organization.Id;
                response.Organization = _mapper.TOOrgUpdateResponse(request);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Orgganization Service: Create : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new OrganizationCreateData
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Creation failed due to - " + ex.Message,
                    Organization = null
                });
            }
        }

        public override async Task<OrganizationUpdateData> Update(OrgUpdateRequest request, ServerCallContext context)
        {
            try
            {
                Organization organization = new Organization();
                OrganizationUpdateData response = new OrganizationUpdateData();
                organization.Id = request.Id;
                organization.OrganizationId = request.OrgId;
                organization.Type = request.Type;
                organization.Name = request.Name;
                organization.AddressType = request.AddressType;
                organization.AddressStreet = request.Street;
                organization.AddressStreetNumber = request.StreetNumber;
                organization.City = request.City;
                organization.CountryCode = request.CountryCode;
                organization.reference_date = Convert.ToDateTime(request.ReferenceDate);
                organization.vehicle_default_opt_in = request.VehicleDefaultOptIn;
                organization.driver_default_opt_in = request.DriverDefaultOptIn;
                var OrgId = await organizationtmanager.Update(organization);

                if (OrgId.Id == 0)
                {
                    response.Message = "Organization ID not exist";
                }
                else if (OrgId.Id == -1)
                {
                    response.Message = "This organization is already exist";
                }
                else
                {
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Organization Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Organization Updated", 1, 2, organization.Id.ToString());
                    response.Code = Responcecode.Success;
                    response.Message = "Organization updated";
                    request.Id = organization.Id;
                    response.Organization = _mapper.TOOrgUpdateResponse(request);
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Orgganization Service: Updated : " + ex.Message + " " + ex.StackTrace);
                return await Task.FromResult(new OrganizationUpdateData
                {
                    Code = Responcecode.Failed,
                    Message = "Organization update failed due to - " + ex.Message,
                    Organization = null
                });
            }
        }

        public override async Task<OrganizationGetData> Get(IdRequest request, ServerCallContext context)
        {

            net.atos.daf.ct2.organization.entity.OrganizationResponse organization = new net.atos.daf.ct2.organization.entity.OrganizationResponse();
            OrganizationGetData response = new OrganizationGetData();
            _logger.LogInformation("Get Organization .");
            organization = await organizationtmanager.Get(request.Id);
            response.Organization = _mapper.ToOrganizationResponse(organization);
            response.Message = "Get";
            response.Code = Responcecode.Success;
            return await Task.FromResult(response);
        }

        public override async Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToOrganizationPreference(request);
                preference.Exists = false;
                preference = await preferencemanager.Create(preference);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Create Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.Id)).Result;
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Created";
                response.AccountPreference = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in organization service:create preference with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Creation Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> UpdatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToOrganizationPreference(request);
                preference.Exists = false;
                preference = await preferencemanager.Update(preference);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Update Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Update Preference", 1, 2, Convert.ToString(preference.Id)).Result;
                if (preference.Id.HasValue) request.Id = preference.Id.Value;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Preference Updated";
                response.AccountPreference = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in organization service:update preference with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference update Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> DeletePreference(IdRequest request, ServerCallContext context)
        {
            try
            {
                var result = await preferencemanager.Delete(request.Id, Preference.PreferenceType.Account);
                var auditResult = auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Oeganization Component", "Delete Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Preference", 1, 2, Convert.ToString(request.Id)).Result;
                // response 
                AccountPreferenceResponse response = new AccountPreferenceResponse();
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Preference Delete.";
                }
                else
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Preference Not Found.";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in organization service:delete organization preference with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Preference Deletion Faile due to - " + ex.Message,
                    AccountPreference = null
                });
            }
        }

        public override async Task<OrganizationPreferenceResponse> GetPreference(IdRequest request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreferenceFilter preferenceFilter = new Preference.AccountPreferenceFilter();
                preferenceFilter.Id = request.Id;
                preferenceFilter.PreferenceType = Preference.PreferenceType.Organization;
                _logger.LogInformation("Get account preference.");
                var result = await organizationtmanager.GetPreference(preferenceFilter.Id);
                // response 
                OrganizationPreferenceResponse response = new OrganizationPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Get";
                response.OrganizationPreference = _mapper.ToPreferenceResponse(result);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in organization service:get organization preference with exception - " + ex.Message + ex.StackTrace);
                return await Task.FromResult(new OrganizationPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Preference Get Faile due to - " + ex.Message
                });
            }
        }
    }
}

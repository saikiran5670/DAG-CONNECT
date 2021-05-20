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
using net.atos.daf.ct2.relationship;
using net.atos.daf.ct2.relationship.entity;
using static net.atos.daf.ct2.utilities.CommonEnums;
using log4net;
using System.Reflection;

namespace net.atos.daf.ct2.organizationservice
{
    public class OrganizationManagementService : OrganizationService.OrganizationServiceBase
    {

       
        private readonly IAuditTraillib _AuditTrail;
        private readonly IAuditTraillib auditlog;

        private ILog _logger;
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IVehicleManager vehicleManager;
        private readonly EntityMapper _mapper;
        private readonly IRelationshipManager _relationshipManager;


        public OrganizationManagementService(
                                             IAuditTraillib AuditTrail,
                                             IOrganizationManager _organizationmanager,
                                             IPreferenceManager _preferencemanager,
                                             IVehicleManager _vehicleManager,
                                             IAuditTraillib _auditlog,
                                             IRelationshipManager relationshipManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 
            _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager = _preferencemanager;
            vehicleManager = _vehicleManager;
            auditlog = _auditlog;
            _mapper = new EntityMapper();
            _relationshipManager = relationshipManager;
        }

        public override async Task<OrganizationprimaryFieldsListResponse> GetAllOrganizations(OrganizationByID request, ServerCallContext context)
        {
            try
            {
                OrganizationprimaryFieldsListResponse objOrganizationprimaryFieldsListResponse = new OrganizationprimaryFieldsListResponse();

                net.atos.daf.ct2.organization.entity.OrganizationByID objOrganizationEntity = new organization.entity.OrganizationByID();
                objOrganizationEntity.id = request.Id;
                objOrganizationEntity.roleId = request.RoleId;
                var data = await organizationtmanager.Get(objOrganizationEntity);
                if (data == null)
                {
                    return null;
                }
                foreach (var item in data)
                {
                    OrganizationprimaryFieldsResponse objOrganizationprimaryFieldsResponse = new OrganizationprimaryFieldsResponse();
                    objOrganizationprimaryFieldsResponse.Id = item.id;
                    objOrganizationprimaryFieldsResponse.Name = item.name;
                    objOrganizationprimaryFieldsListResponse.OrganizationList.Add(objOrganizationprimaryFieldsResponse);
                }
                return objOrganizationprimaryFieldsListResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }
        //Relationship Management

        public override async Task<RelationshipCreateResponse> CreateRelationship(RelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var relationship = new Relationship();
                var response = new RelationshipCreateResponse();
                relationship.OrganizationId = request.OrganizationId;
                relationship.Code = request.Code;
                relationship.Name = request.Name;
                relationship.Level = request.Level;
                relationship.FeaturesetId = request.Featuresetid;
                relationship.Description = request.Description;
                relationship.State = request.State;

                relationship = await _relationshipManager.CreateRelationship(relationship);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Relationship Component", "Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Create", 1, 2, relationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Created";
                request.Id = relationship.Id;
                response.Relationship = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Relationship Creation failed due to - " + ex.Message,
                    Relationship = null
                });
            }
        }

        public override async Task<RelationshipCreateResponse> UpdateRelationship(RelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var relationship = new Relationship();
                var response = new RelationshipCreateResponse();
                relationship.Id = request.Id;
                relationship.OrganizationId = request.OrganizationId;
                relationship.Code = request.Code;
                relationship.Name = request.Name;
                relationship.Level = request.Level;
                relationship.FeaturesetId = request.Featuresetid;
                relationship.Description = request.Description;
                relationship.State = request.State;

                relationship = await _relationshipManager.UpdateRelationship(relationship);
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Relationship Component", "Organization Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Updated", 1, 2, relationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Relatioship Updated Successfully";
                request.Id = relationship.Id;
                response.Relationship = request;

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Relationship Updation failed due to - " + ex.Message,
                    Relationship = null
                });
            }
        }

        public async override Task<RelationshipGetResponse> GetRelationship(RelationshipCreateRequest request, ServerCallContext context)
        {
            try
            {
                var response = new RelationshipGetResponse();
                var relationshipFilter = new RelationshipFilter();
                relationshipFilter.Id = request.Id;
                relationshipFilter.OrganizationId = request.OrganizationId;
                relationshipFilter.Code = request.Code;
                relationshipFilter.FeaturesetId = request.Featuresetid;
                relationshipFilter.Level = request.Level;
                //relationshipFilter.Name = request.Name;
                //relationshipFilter.Description = request.Description;
                //relationshipFilter.State = request.State;
                var orgRelationships = _relationshipManager.GetRelationship(relationshipFilter).Result;
                response.RelationshipList.AddRange(orgRelationships
                                     .Select(x => new RelationshipGetRequest()
                                     {
                                         Id = x.Id,
                                         OrganizationId = x.OrganizationId,
                                         Code = x.Code,
                                         Description = x.Description,
                                         Name = x.Name,
                                         Featuresetid = x.FeaturesetId,
                                         Level = x.Level,
                                         State = x.State,
                                         CreatedAt = x.CreatedAt

                                     }).ToList());
                _logger.Info("Get  relationship details.");
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RelationshipGetResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }


        public async override Task<RelationshipDeleteResponse> DeleteRelationship(RelationshipDeleteRequest request, ServerCallContext context)
        {
            try
            {
                var result = _relationshipManager.DeleteRelationship(request.Id).Result;
                var response = new RelationshipDeleteResponse();
                if (result)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Relationship deleted.";
                }
                else
                {
                    response.Code = Responcecode.Conflict;
                    response.Message = "Relationship cannot be deleted as it is mapped with organiztion.";

                }
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Service", "Relationship Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Delete", 1, 2, request.Id.ToString());
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RelationshipDeleteResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Relationship Deletion Faile due to - " + ex.Message,

                });
            }
        }

        public override async Task<OrgRelationshipCreateResponse> CreateOrgRelationship(OrgRelationshipCreateRequest request, ServerCallContext context)
        {
            OrganizationRelationShip objRelationship = new OrganizationRelationShip();
            try
            {
                OrgRelationshipCreateResponse responce = new OrgRelationshipCreateResponse();

                foreach (var organization in request.TargetOrgId)
                {
                    foreach (var vehgroup in request.VehicleGroupID)
                    {
                        objRelationship.relationship_id = request.RelationShipId;
                        objRelationship.vehicle_group_id = vehgroup;
                        objRelationship.owner_org_id = request.OwnerOrId;
                        objRelationship.created_org_id = request.CreatedOrgId;
                        objRelationship.target_org_id = organization;
                        objRelationship.allow_chain = request.AllowChain;
                        var orgrelationid = await _relationshipManager.CreateRelationShipMapping(objRelationship);
                        request.OrgRelationId = orgrelationid;

                        responce.Code = Responcecode.Success;
                        responce.Relationship.Add(orgrelationid);

                    }
                }
                responce.Code = Responcecode.Success;
                return await Task.FromResult(responce);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new OrgRelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Relationship Create Faile due to - " + ex.Message,

                });
            }
        }

        public override async Task<EndOrgRelationshipResponse> EndOrgRelationShip(EndOrgRelationShipRequest request, ServerCallContext context)
        {
            try
            {
                EndOrgRelationshipResponse objresponce = new EndOrgRelationshipResponse();
                foreach (var item in request.OrgRelationShipid)
                {
                    var orgrelationid = await _relationshipManager.EndRelationShipMapping(item);
                    objresponce.OrgRelationShipid.Add(orgrelationid);
                }


                objresponce.Code = Responcecode.Success;
                objresponce.Message = "Relationships Ended";
                return await Task.FromResult(objresponce);

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new EndOrgRelationshipResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Relationship Create Faile due to - " + ex.Message,

                });
                throw;
            }
        }
        public override async Task<ChainingResponse> AllowChaining(ChainingRequest request, ServerCallContext context)
        {
            try
            {
                var orgrelationid = await _relationshipManager.AllowChaining(request.OrgRelationID, request.AllowChaining);
                return await Task.FromResult(new ChainingResponse
                {
                    Code = Responcecode.Success,
                    Message = "Relationship Ended",
                    OrgRelationShipid = orgrelationid

                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ChainingResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Allow chaining failed due to - " + ex.Message,

                });
                throw;
            }
        }
        
        public async override Task<OrgRelationshipGetResponse> GetOrgRelationshipMapping(OrgRelationshipMappingGetRequest request, ServerCallContext context)
        {
            try
            {
                var response = new OrgRelationshipGetResponse();
                var orgRelationshipFilter = new OrganizationRelationShip();
                orgRelationshipFilter.Id = request.Id;
                orgRelationshipFilter.relationship_id = request.RelationShipId;
                orgRelationshipFilter.vehicle_group_id = request.VehicleGroupID;
                orgRelationshipFilter.owner_org_id = request.OwnerOrId;
                orgRelationshipFilter.created_org_id = request.CreatedOrgId;
                orgRelationshipFilter.target_org_id = request.TargetOrgId;

                var orgRelationships = _relationshipManager.GetRelationshipMapping(orgRelationshipFilter).Result;
                response.OrgRelationshipMappingList.AddRange(orgRelationships
                                     .Select(x => new OrgRelationshipMappingGetRequest()
                                     {
                                         Id = x.Id,
                                         RelationShipId = x.relationship_id,
                                         TargetOrgId = x.target_org_id,
                                         CreatedOrgId = x.created_org_id,
                                         StartDate=x.start_date,
                                         CreatedAt = x.created_at,
                                         EndDate = x.end_date,
                                         AllowChain = x.allow_chain,
                                         OrganizationName = x.OrganizationName,
                                         VehicleGroupID = x.vehicle_group_id,
                                         OrgRelationId = x.relationship_id,
                                         RelationshipName = x.RelationshipName,
                                         VehicleGroupName = x.VehicleGroupName
                                         
                                     }).ToList());
                _logger.Info("Get  relationship mapping details.");
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new OrgRelationshipGetResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
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
                response.Organization = _mapper.TOOrgCreateResponse(request);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
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
                _logger.Error(null, ex);
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
            _logger.Info("Get Organization .");
            organization = await organizationtmanager.Get(request.Id);
            response.Message = "Get";
            if (organization.Id > 0)
            {
                response.Organization = _mapper.ToOrganizationResponse(organization);
                response.Code = Responcecode.Success;
            }
            else
            {
                response.Code = Responcecode.NotFound;
                response.Message = "Organization not found";
            }
            return await Task.FromResult(response);
        }

        public override async Task<OrgDetailResponse> GetOrganizationDetails(IdRequest request, ServerCallContext context)
        {
            net.atos.daf.ct2.organization.entity.OrganizationDetailsResponse organization = new net.atos.daf.ct2.organization.entity.OrganizationDetailsResponse();
            OrgDetailResponse response = new OrgDetailResponse();
            _logger.Info("Get Organization Details .");
            organization = await organizationtmanager.GetOrganizationDetails(request.Id);
            if (organization.id > 0)
            {
                response = _mapper.ToOrganizationDetailsResponse(organization);
            }
            return await Task.FromResult(response);
        }
        public override async Task<GetAllOrgResponse> GetAll(IdRequest request, ServerCallContext context)
        {
            var organization = new OrganizationResponse();
            var response = new GetAllOrgResponse();
            _logger.Info("Get Organization .");
            organization.OrganizationList = await organizationtmanager.GetAll(request.Id);
            response.OrganizationList.AddRange(organization.OrganizationList
                                    .Select(x => new OrgGetResponse()
                                    {
                                        Id = x.Id,
                                        Type = x.type,
                                        Name = x.name,
                                        AddressStreet = x.street,
                                        AddressType = x.address_type,
                                        AddressStreetNumber = x.street_number,
                                        PostalCode = x.postal_code,
                                        City = x.city,
                                        CountryCode = x.country_code,
                                        OrganizationId = x.org_id,
                                        Referenced = x.reference_date,
                                        VehicleOptIn = x.vehicle_default_opt_in,
                                        DriverOptIn = x.driver_default_opt_in,
                                        IsActive = x.state == (char)State.Active ? true : false
                                    }).ToList());


            response.Message = "Get";
            if (organization.OrganizationList.Count > 0)
            {

                response.Code = Responcecode.Success;
            }
            else
            {
                response.Code = Responcecode.NotFound;
            }
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
                _logger.Error(null, ex);
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
               _logger.Error(null, ex);
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
                _logger.Error(null, ex);
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
                _logger.Info("Get account preference.");
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
               _logger.Error(null, ex);
                return await Task.FromResult(new OrganizationPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Organization Preference Get Faile due to - " + ex.Message
                });
            }
        }

        public override async Task<ListOfOrganization> GetOrganizations(IdRequest request, ServerCallContext context)
        {
            net.atos.daf.ct2.organization.entity.Organization organization = new net.atos.daf.ct2.organization.entity.Organization();
            ListOfOrganization response = new ListOfOrganization();
            _logger.Info("GetAllOrganizations .");
            var result = await organizationtmanager.GetAllOrganizations(request.Id);           
            if (result.Count() > 0)
            {
                foreach (net.atos.daf.ct2.organization.entity.Organization entity in result)
                {
                    response.Organizations.Add(_mapper.ToListOfOrganizationResponse(entity));
                }
                response.Code = Responcecode.Success;
                response.Message = "Get";
            }
            else
            {
                response.Code = Responcecode.NotFound;
                response.Message = "Organization not found.";
            }
            return await Task.FromResult(response);            
        }

        public override async Task<LevelResponse> GetLevelByRoleId(LevelByRoleRequest request, ServerCallContext context)
        {
            _logger.Info("GetLevelByRoleId method Called.");
            LevelResponse objLevelResponse = new LevelResponse();
            int level = await organizationtmanager.GetLevelByRoleId(request.OrgId,request.RoleId);
            if (level > 0)
            {
                objLevelResponse.Level = level;
                objLevelResponse.Code = Responcecode.Success;
                objLevelResponse.Message = "Restrived Level";
            }
            else
            {
                objLevelResponse.Level = level;
                objLevelResponse.Code = Responcecode.NotFound;
                objLevelResponse.Message = "Level Not found";
            }
            return await Task.FromResult(objLevelResponse);

        }
    }
}

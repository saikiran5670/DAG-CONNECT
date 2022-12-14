using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organizationservice.entity;
using net.atos.daf.ct2.relationship;
using net.atos.daf.ct2.relationship.entity;
using net.atos.daf.ct2.vehicle;
using static net.atos.daf.ct2.utilities.CommonEnums;
using Preference = net.atos.daf.ct2.accountpreference;

namespace net.atos.daf.ct2.organizationservice
{
    public class OrganizationManagementService : OrganizationService.OrganizationServiceBase
    {
        private readonly IAuditTraillib _auditTrail;
        private readonly IAuditTraillib _auditlog;

        private readonly ILog _logger;
        private readonly IOrganizationManager _organizationtmanager;
        private readonly IPreferenceManager _preferencemanager;
        private readonly IVehicleManager _vehicleManager;
        private readonly EntityMapper _mapper;
        private readonly IRelationshipManager _relationshipManager;


        public OrganizationManagementService(
                                             IAuditTraillib auditTrail,
                                             IOrganizationManager organizationmanager,
                                             IPreferenceManager preferencemanager,
                                             IVehicleManager vehicleManager,
                                             IAuditTraillib auditlog,
                                             IRelationshipManager relationshipManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditTrail = auditTrail;
            _organizationtmanager = organizationmanager;
            _preferencemanager = preferencemanager;
            _vehicleManager = vehicleManager;
            _auditlog = auditlog;
            _mapper = new EntityMapper();
            _relationshipManager = relationshipManager;
        }

        public override async Task<OrganizationprimaryFieldsListResponse> GetAllOrganizations(OrganizationByID request, ServerCallContext context)
        {
            try
            {
                OrganizationprimaryFieldsListResponse objOrganizationprimaryFieldsListResponse = new OrganizationprimaryFieldsListResponse();

                net.atos.daf.ct2.organization.entity.OrganizationByID objOrganizationEntity = new organization.entity.OrganizationByID();
                objOrganizationEntity.Id = request.Id;
                objOrganizationEntity.RoleId = request.RoleId;
                var data = await _organizationtmanager.Get(objOrganizationEntity);
                if (data == null)
                {
                    return null;
                }
                foreach (var item in data)
                {
                    OrganizationprimaryFieldsResponse objOrganizationprimaryFieldsResponse = new OrganizationprimaryFieldsResponse();
                    objOrganizationprimaryFieldsResponse.Id = item.Id;
                    objOrganizationprimaryFieldsResponse.Name = item.Name;
                    objOrganizationprimaryFieldsListResponse.OrganizationList.Add(objOrganizationprimaryFieldsResponse);
                }
                return objOrganizationprimaryFieldsListResponse;
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAllOrganizations)}: With Error:-", ex);
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
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Relationship Component", "Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Create", 1, 2, relationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Created";
                request.Id = relationship.Id;
                response.Relationship = request;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(CreateRelationship)}: With Error:-", ex);
                return await Task.FromResult(new RelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Relationship Component", "Organization Relationship Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Updated", 1, 2, relationship.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Relatioship Updated Successfully";
                request.Id = relationship.Id;
                response.Relationship = request;

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(UpdateRelationship)}: With Error:-", ex);
                return await Task.FromResult(new RelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                                         Code = x.Code.ToUpper(),
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
                _logger.Error($"{nameof(GetRelationship)}: With Error:-", ex);
                return await Task.FromResult(new RelationshipGetResponse
                {
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                    response.Message = "Relationship cannot be deleted as it is mapped with organization.";

                }
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Service", "Relationship Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Relationship Delete", 1, 2, request.Id.ToString());
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(DeleteRelationship)}: With Error:-", ex);
                return await Task.FromResult(new RelationshipDeleteResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,

                });
            }
        }

        public override async Task<OrgRelationshipCreateResponse> CreateOrgRelationship(OrgRelationshipCreateRequest request, ServerCallContext context)
        {
            OrganizationRelationShip objRelationship = new OrganizationRelationShip();
            try
            {
                OrgRelationshipCreateResponse response = new OrgRelationshipCreateResponse();

                //Check if vehicle groups of type 'G' contains any Visible vehicles
                // if yes then discard the request
                var conflictResult = await _organizationtmanager.GetVisibleVehiclesGroupCheck(request.VehicleGroupID.ToArray(), request.OwnerOrgId);

                if (conflictResult.Count() > 0)
                {
                    response.OrgRelationshipVehicleConflictList
                        .AddRange(conflictResult.Select(x => new OrgRelationshipVehicleConflict
                        {
                            VehicleGroupName = x.Name,
                            ConflictedVINs = x.VINs
                        }));
                    response.Code = Responcecode.Forbidden;
                    return await Task.FromResult(response);
                }

                var relationships = await _relationshipManager.GetOrgRelationships(request.OwnerOrgId);
                int relationsCount = 0;
                if (request.Isconfirmed == false)
                {
                    foreach (var organization in request.TargetOrgId)
                    {
                        foreach (var vehgroup in request.VehicleGroupID)
                        {
                            if (relationships.Any(i => i.Target_org_id == organization && i.Vehicle_group_id == vehgroup &&
                                                       i.Relationship_id == request.RelationShipId && !i.End_date.HasValue))
                            {
                                relationsCount++;
                                response.Code = Responcecode.Conflict;
                            }
                        }
                        request.Isconfirmed = relationsCount == 0;
                    }
                }

                int orgRelationId = 0;
                foreach (var organization in request.TargetOrgId)
                {
                    foreach (var vehgroup in request.VehicleGroupID)
                    {
                        objRelationship.Relationship_id = request.RelationShipId;
                        objRelationship.Vehicle_group_id = vehgroup;
                        objRelationship.Owner_org_id = request.OwnerOrgId;
                        objRelationship.Created_org_id = request.CreatedOrgId;
                        objRelationship.Target_org_id = organization;
                        objRelationship.Allow_chain = request.AllowChain;
                        if (relationships.Any(i => i.Target_org_id == objRelationship.Target_org_id && i.Vehicle_group_id == objRelationship.Vehicle_group_id &&
                                                   i.Relationship_id == objRelationship.Relationship_id && !i.End_date.HasValue))
                        {
                            OrgRelationshipMappingGetRequest presetRelationships = new OrgRelationshipMappingGetRequest();
                            presetRelationships.RelationShipId = request.RelationShipId;
                            presetRelationships.VehicleGroupID = vehgroup;
                            presetRelationships.OwnerOrId = request.OwnerOrgId;
                            presetRelationships.CreatedOrgId = request.CreatedOrgId;
                            presetRelationships.TargetOrgId = organization;
                            response.OrgRelationshipMappingList.Add(presetRelationships);
                        }
                        else if (request.Isconfirmed)
                        {
                            orgRelationId = await _relationshipManager.CreateRelationShipMapping(objRelationship);
                            response.Code = Responcecode.Success;
                        }

                        request.OrgRelationId = orgRelationId;

                        //responce.Code = Responcecode.Success;
                        response.Relationship.Add(orgRelationId);

                    }
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(CreateOrgRelationship)}: With Error:-", ex);
                return await Task.FromResult(new OrgRelationshipCreateResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                _logger.Error($"{nameof(EndOrgRelationShip)}: With Error:-", ex);
                return await Task.FromResult(new EndOrgRelationshipResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,

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
                _logger.Error($"{nameof(AllowChaining)}: With Error:-", ex);
                return await Task.FromResult(new ChainingResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,

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
                orgRelationshipFilter.Relationship_id = request.RelationShipId;
                orgRelationshipFilter.Vehicle_group_id = request.VehicleGroupID;
                orgRelationshipFilter.Owner_org_id = request.OwnerOrId;
                orgRelationshipFilter.Created_org_id = request.CreatedOrgId;
                orgRelationshipFilter.Target_org_id = request.TargetOrgId;

                var orgRelationships = _relationshipManager.GetRelationshipMapping(orgRelationshipFilter).Result;
                response.OrgRelationshipMappingList.AddRange(orgRelationships
                                     .Select(x => new OrgRelationshipMappingGetRequest()
                                     {
                                         Id = x.Id,
                                         RelationShipId = x.Relationship_id,
                                         TargetOrgId = x.Target_org_id,
                                         CreatedOrgId = x.Created_org_id,
                                         StartDate = x.Start_date.Value,
                                         CreatedAt = x.Created_at,
                                         EndDate = x.End_date.Value,
                                         AllowChain = x.Allow_chain,
                                         OrganizationName = x.OrganizationName,
                                         VehicleGroupID = x.Vehicle_group_id,
                                         OrgRelationId = x.Relationship_id,
                                         RelationshipName = x.RelationshipName,
                                         VehicleGroupName = x.VehicleGroupName

                                     }).ToList());
                _logger.Info("Get  relationship mapping details.");
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetOrgRelationshipMapping)}: With Error:-", ex);
                return await Task.FromResult(new OrgRelationshipGetResponse
                {
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                organization.ReferenceDate = Convert.ToDateTime(request.ReferenceDate);
                organization = await _organizationtmanager.Create(organization);
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Organization Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Organization Create", 1, 2, organization.Id.ToString());
                response.Code = Responcecode.Success;
                response.Message = "Created";
                request.Id = organization.Id;
                response.Organization = _mapper.TOOrgCreateResponse(request);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(Create)}: With Error:-", ex);
                return await Task.FromResult(new OrganizationCreateData
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                organization.VehicleDefaultOptIn = request.VehicleDefaultOptIn;
                organization.DriverDefaultOptIn = request.DriverDefaultOptIn;
                var OrgId = await _organizationtmanager.Update(organization);

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
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Organization Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Organization Updated", 1, 2, organization.Id.ToString());
                    response.Code = Responcecode.Success;
                    response.Message = "Organization updated";
                    request.Id = organization.Id;
                    response.Organization = _mapper.TOOrgUpdateResponse(request);
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(Update)}: With Error:-", ex);
                return await Task.FromResult(new OrganizationUpdateData
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
                    Organization = null
                });
            }
        }

        public override async Task<OrganizationGetData> Get(IdRequest request, ServerCallContext context)
        {

            net.atos.daf.ct2.organization.entity.OrganizationResponse organization; //= new net.atos.daf.ct2.organization.entity.OrganizationResponse();
            OrganizationGetData response = new OrganizationGetData();
            _logger.Info("Get Organization .");
            organization = await _organizationtmanager.Get(request.Id);
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
            net.atos.daf.ct2.organization.entity.OrganizationDetailsResponse organization;//= new net.atos.daf.ct2.organization.entity.OrganizationDetailsResponse();
            OrgDetailResponse response = new OrgDetailResponse();
            _logger.Info("Get Organization Details .");
            organization = await _organizationtmanager.GetOrganizationDetails(request.Id);
            if (organization.Id > 0)
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
            organization.OrganizationList = await _organizationtmanager.GetAll(request.Id);
            response.OrganizationList.AddRange(organization.OrganizationList
                                    .Select(x => new OrgGetResponse()
                                    {
                                        Id = x.Id,
                                        Type = x.Type,
                                        Name = x.Name,
                                        AddressStreet = x.Street,
                                        AddressType = x.AddressType,
                                        AddressStreetNumber = x.StreetNumber,
                                        PostalCode = x.PostalCode,
                                        City = x.City,
                                        CountryCode = x.CountryCode,
                                        OrganizationId = x.OrgId,
                                        Referenced = x.ReferenceDate,
                                        VehicleOptIn = x.VehicleDefaultOptIn,
                                        DriverOptIn = x.DriverDefaultOptIn,
                                        IsActive = x.State == (char)State.Active ? true : false
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

        public override async Task<GetAllContextOrgsResponse> GetAllOrganizationsForContext(Empty empty, ServerCallContext context)
        {
            try
            {
                var contextOrgs = await _organizationtmanager.GetAllOrganizationsForContext();

                var response = new GetAllContextOrgsResponse();
                foreach (var item in contextOrgs)
                {
                    var listItem = new ContextOrgsList();
                    listItem.Id = item.Id;
                    listItem.Name = item.Name;
                    response.ContextOrgs.Add(listItem);
                }
                response.Code = contextOrgs.Count() > 0 ? Responcecode.Success : Responcecode.NotFound;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAllOrganizationsForContext)}: With Error:-", ex);
                return await Task.FromResult(new GetAllContextOrgsResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG
                });
            }
        }

        public override async Task<AccountPreferenceResponse> CreatePreference(AccountPreference request, ServerCallContext context)
        {
            try
            {
                Preference.AccountPreference preference = new Preference.AccountPreference();
                preference = _mapper.ToOrganizationPreference(request);
                preference.Exists = false;
                preference = await _preferencemanager.Create(preference);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Create Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Preference", 1, 2, Convert.ToString(preference.Id)).Result;
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
                _logger.Error($"{nameof(CreatePreference)}: With Error:-", ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                preference = await _preferencemanager.Update(preference);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Organization Component", "Update Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Update Preference", 1, 2, Convert.ToString(preference.Id)).Result;
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
                _logger.Error($"{nameof(UpdatePreference)}: With Error:-", ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
                    AccountPreference = null
                });
            }
        }
        public override async Task<AccountPreferenceResponse> DeletePreference(IdRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _preferencemanager.Delete(request.Id, Preference.PreferenceType.Account);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Oeganization Component", "Delete Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Delete Preference", 1, 2, Convert.ToString(request.Id)).Result;
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
                _logger.Error($"{nameof(DeletePreference)}: With Error:-", ex);
                return await Task.FromResult(new AccountPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG,
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
                var result = await _organizationtmanager.GetPreference(preferenceFilter.Id);
                // response 
                OrganizationPreferenceResponse response = new OrganizationPreferenceResponse();
                response.Code = Responcecode.Success;
                response.Message = "Get";
                response.OrganizationPreference = _mapper.ToPreferenceResponse(result);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetPreference)}: With Error:-", ex);
                return await Task.FromResult(new OrganizationPreferenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = OrganizationConstants.INTERNAL_SERVER_MSG
                });
            }
        }

        public override async Task<ListOfOrganization> GetOrganizations(IdRequest request, ServerCallContext context)
        {
            net.atos.daf.ct2.organization.entity.Organization organization = new net.atos.daf.ct2.organization.entity.Organization();
            ListOfOrganization response = new ListOfOrganization();
            _logger.Info("GetAllOrganizations .");
            var result = await _organizationtmanager.GetAllOrganizations(request.Id);
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
            int level = await _organizationtmanager.GetLevelByRoleId(request.OrgId, request.RoleId);
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

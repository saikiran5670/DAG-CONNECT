using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Organization;
using net.atos.daf.ct2.portalservice.Entity.Relationship;
using Newtonsoft.Json;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using OrganizationBusinessService = net.atos.daf.ct2.organizationservice;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("organization")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationController : BaseController
    {
        private readonly OrganizationMapper _mapper;

        private readonly ILog _logger;
        private readonly AuditHelper _auditHelper;
        private readonly FeatureSetMapper _featureSetMapper;
        private readonly RelationshipMapper _relationshipMapper;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private readonly OrganizationService.OrganizationServiceClient _organizationClient;
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private readonly string _fk_Constraint = "violates foreign key constraint";
        public IConfiguration Configuration { get; }

        public OrganizationController(
                                      OrganizationService.OrganizationServiceClient organizationClient,
                                      AccountBusinessService.AccountService.AccountServiceClient accountClient,
                                      FeatureService.FeatureServiceClient featureclient,
                                      VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient,
                                      IConfiguration configuration, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor,
                                      SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._organizationClient = organizationClient;
            _accountClient = accountClient;
            _mapper = new OrganizationMapper();
            _relationshipMapper = new RelationshipMapper();
            _featureSetMapper = new FeatureSetMapper(featureclient);
            _vehicleClient = vehicleClient;
            Configuration = configuration;
            _auditHelper = auditHelper;
        }

        //OrgRelationship
        [HttpPost]
        [Route("relationship/create")]
        public async Task<IActionResult> CreateRelationship(RelationshipPortalRequest request)
        {
            try
            {
                if (request.FeatureIds.Count >= 1)
                {
                    var featureSetId = await _featureSetMapper.RetrieveFeatureSetIdById(request.FeatureIds);
                    request.FeaturesetId = featureSetId;
                }
                else
                {
                    return StatusCode(400, "Please provide relationship features");
                }

                if (request.FeaturesetId > 0)
                {
                    _logger.Info("Relationship create function called ");
                    //Assign context orgId
                    request.OrganizationId = GetContextOrgId();
                    if (request.OrganizationId == 0)
                    {
                        return StatusCode(400, "Please provide OrganizationId:");
                    }
                    if (string.IsNullOrEmpty(request.Name) || (request.Name.Trim().Length < 1))
                    {
                        return StatusCode(400, "Please provide relationship name:");
                    }

                    var objRequest = _relationshipMapper.ToRelationshipRequest(request);
                    var orgResponse = await _organizationClient.CreateRelationshipAsync(objRequest);
                    if (orgResponse.Relationship == null)
                    {
                        return StatusCode(500, "Failed to create relationship.");
                    }
                    else
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                         "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "CreateRelationship  method in Organnization controller", 0, orgResponse.Relationship.Id, JsonConvert.SerializeObject(request),
                  _userDetails);
                        return Ok("Relationship Created :" + orgResponse);
                    }
                }
                else
                {
                    return StatusCode(500, "Featureset not created");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                 "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "CreateRelationship  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request),
                _userDetails);
                _logger.Error($"{nameof(CreateRelationship)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }


        [HttpPut]
        [Route("relationship/update")]
        public async Task<IActionResult> UpdateRelationship(RelationshipPortalRequest request)
        {
            try
            {

                if (request.FeaturesetId > 0)
                {
                    _logger.Info("Relationship update function called ");

                    if (request.OrganizationId == 0 || request.Id == 0 || request.Level == 0 || string.IsNullOrEmpty(request.Code))
                    {
                        return StatusCode(400, "Please provide OrganizationId, Level,Code and org relationship id:");
                    }
                    if (string.IsNullOrEmpty(request.Name) || (request.Name.Trim().Length < 1))
                    {
                        return StatusCode(400, "Please provide  relationship name:");
                    }
                    if (request.FeatureIds.Count >= 1)
                    {
                        var featureSetId = await _featureSetMapper.UpdateFeatureSetIdById(request.FeatureIds, request.FeaturesetId);
                        request.FeaturesetId = featureSetId;
                    }
                    else
                    {
                        return StatusCode(400, "Please provide relationship features");
                    }
                    //Assign context orgId
                    request.OrganizationId = GetContextOrgId();
                    var objRequest = _relationshipMapper.ToRelationshipRequest(request);
                    var orgResponse = await _organizationClient.UpdateRelationshipAsync(objRequest);
                    if (orgResponse.Relationship.Id < 1)
                    {
                        return StatusCode(400, "Relationship not updated :" + request.Id);
                    }
                    else
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                       "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                       "UPDATERelationship  method in Organnization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);
                        return Ok("Relationship updated :" + orgResponse);
                    }
                }
                else
                {
                    return StatusCode(500, "Please provide Featureset id");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                     "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                     "UPDATERelationship  method in Organnization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);

                _logger.Error($"{nameof(UpdateRelationship)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("relationship/get")]
        public async Task<IActionResult> GetRelationship([FromQuery] RelationshipFilter filterRequest)
        {
            try
            {
                _logger.Info("Organization relationship get function called ");

                //Assign context orgId
                if (filterRequest.OrganizationId > 0)
                {
                    filterRequest.OrganizationId = GetContextOrgId();
                }

                var request = new RelationshipCreateRequest()
                {
                    Id = filterRequest.Id,
                    Featuresetid = filterRequest.FeaturesetId,
                    OrganizationId = filterRequest.OrganizationId,
                    Level = _userDetails.RoleLevel,
                    Code = filterRequest.Code ?? string.Empty
                };

                var orgResponse = await _organizationClient.GetRelationshipAsync(request);
                orgResponse.RelationshipList.Where(S => S.Featuresetid > 0)
                                               .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.Featuresetid).Result); return S; }).ToList();
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetRelationship)}: With Error:-", ex);

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }
        [HttpGet]
        [Route("relationship/getlevelcode")]
        public async Task<IActionResult> GetRelationshipLevelCode()
        {
            try
            {
                var level = _userDetails.RoleLevel;
                var levelCode = new RelationshipLevelCode();
                var relationshipLevels = Enum.GetValues(typeof(RelationshipLevel))
                     .Cast<RelationshipLevel>()
                     .Select(t => new Level
                     {
                         Id = ((int)t),
                         Name = t.ToString()
                     }).ToList();

                levelCode.Levels = relationshipLevels.Where(x => x.Id >= level).ToList();

                levelCode.Codes = Enum.GetValues(typeof(RelationshipCode))
                     .Cast<RelationshipCode>()
                     .Select(t => new Code
                     {
                         Id = ((int)t),
                         Name = t.ToString()
                     }).ToList();

                _logger.Info("Relationship get level and code function called ");

                return Ok(await Task.FromResult(levelCode));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetRelationshipLevelCode)}: With Error:-", ex);

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpDelete]
        [Route("relationship/delete")]
        public async Task<IActionResult> DeleteRelationship(int relationshipId)
        {
            var relationshipRequest = new RelationshipDeleteRequest();
            try
            {
                // Validation                 
                if (relationshipId <= 0)
                {
                    return StatusCode(400, "Relationship id is required.");
                }
                relationshipRequest.Id = relationshipId;
                var response = await _organizationClient.DeleteRelationshipAsync(relationshipRequest);
                response.RelationshipRequest = relationshipRequest;
                if (response != null && response.Code == organizationservice.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                   "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                   "DeleteRelationship  method in Organnization controller", relationshipId, relationshipId, JsonConvert.SerializeObject(relationshipRequest), _userDetails);
                    return Ok(response);
                }
                else if (response.Code == organizationservice.Responcecode.Conflict)
                {
                    return StatusCode(409, "Relationship cannot be deleted as it is mapped with organiztion.");
                }
                else { return StatusCode(404, "Relationship not configured."); }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "RelationshipManagement Component",
                  "RelationshipManagement service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                  "DeleteRelationship  method in Organnization controller", relationshipId, relationshipId, JsonConvert.SerializeObject(relationshipRequest), _userDetails);
                _logger.Error($"{nameof(DeleteRelationship)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        //Organization

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(OrganizationRequest request)
        {
            try
            {
                Organization organization = new Organization();
                _logger.Info("Organization create function called ");


                if (string.IsNullOrEmpty(request.Org_id) || (request.Org_id.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization org_id:");
                }
                if (string.IsNullOrEmpty(request.Name) || (request.Name.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization name:");
                }
                //Assign context orgId
                request.Org_id = GetContextOrgId().ToString();

                organization.OrganizationId = request.Org_id;
                organization.Name = request.Name;
                organization.Type = request.Type;
                organization.AddressType = request.Address_type;
                organization.AddressStreet = request.Street_number;
                organization.AddressStreetNumber = request.Street_number;
                organization.City = request.City;
                organization.CountryCode = request.Country_code;
                organization.PostalCode = request.Postal_code;
                organization.Reference_date = request.Reference_date;
                var objRequest = _mapper.ToOragnizationRequest(request);
                OrganizationBusinessService.OrganizationCreateData orgResponse = await _organizationClient.CreateAsync(objRequest);
                if (orgResponse.Organization.Id < 1)
                {
                    return StatusCode(400, "This organization is already exist :" + request.Org_id);
                }
                else
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                 "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                 "Create  method in Organnization controller", 0, orgResponse.Organization.Id, JsonConvert.SerializeObject(request), _userDetails);

                    return Ok(orgResponse);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                 "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(Create)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(OrganizationRequest request)
        {
            try
            {
                var objRequest = _mapper.ToOragnizationUpdateRequest(request);
                Organization organization = new Organization();

                _logger.Info("Organization update function called ");

                if (request.Id < 1)
                {
                    return StatusCode(400, "Please provide organization ID:");
                }

                //Assign context orgId
                request.Id = GetContextOrgId();

                //if (string.IsNullOrEmpty(request.org_id) || (request.org_id.Trim().Length < 1))
                //{
                //    return StatusCode(400, "Please provide organization org_id:");
                //}
                //if (string.IsNullOrEmpty(request.name) || (request.name.Trim().Length < 1))
                //{
                //    return StatusCode(400, "Please provide organization name:");
                //}

                organization.Id = request.Id;
                //organization.OrganizationId = request.org_id;
                //organization.Name = request.name;
                //organization.Type = request.type;
                //organization.AddressType = request.address_type;
                //organization.AddressStreet = request.street_number;
                //organization.AddressStreetNumber = request.street_number;
                //organization.City = request.city;
                organization.VehicleDefaultOptIn = request.Vehicle_default_opt_in;
                organization.DriverDefaultOptIn = request.Driver_default_opt_in;

                OrganizationBusinessService.OrganizationUpdateData orgResponse = await _organizationClient.UpdateAsync(objRequest);
                if (orgResponse.Organization.Id == 0)
                {
                    return StatusCode(400, "Organization ID not exist: " + request.Id);
                }
                else if (orgResponse.Organization.Id == -1)
                {
                    return StatusCode(400, "This organization is already exist :" + request.Org_id);
                }
                else
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "Update  method in Organnization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(orgResponse.Organization);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
          "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
          "Update  method in Organnization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(Update)}: With Error:-", ex);
                //  await auditlog.AddLogs(DateTime.Now,2,"Organization Component","Organization Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.FAILED,"Update method in organization manager",0,0,JsonConvert.SerializeObject(request));      
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(int organizationId)
        {
            try
            {
                //var objRequest = _mapper.ToOragnizationRequest(organizationId);
                OrganizationBusinessService.IdRequest idRequest = new OrganizationBusinessService.IdRequest();

                // OrganizationBusinessService.OrganizationGetData orgResponse = await organizationClient.GetAsync(idRequest);
                // return Ok("Organization Created :" +orgResponse); 
                _logger.Info("Organization get function called ");
                if (organizationId < 1)
                {
                    return StatusCode(400, "Please provide organization ID:");
                }
                //Assign context orgId
                idRequest.Id = GetContextOrgId();
                OrganizationBusinessService.OrganizationGetData orgResponse = await _organizationClient.GetAsync(idRequest);

                return Ok(orgResponse.Organization);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(Get)}: With Error:-", ex);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("GetOrganizationInfo")]
        public async Task<IActionResult> GetOrganizationInfo(int organizationId)
        {
            try
            {
                IdRequest idRequest = new IdRequest();

                if (organizationId < 1)
                {
                    return StatusCode(400, "Please provide organization ID.");
                }

                //Context org id is not required here
                //idRequest.Id = GetContextOrgId();

                idRequest.Id = organizationId;
                OrgDetailResponse orgResponse = await _organizationClient.GetOrganizationDetailsAsync(idRequest);

                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetOrganizationInfo)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAll(int organizationId)
        {
            try
            {
                var idRequest = new IdRequest();
                if (organizationId > 0)
                {
                    organizationId = GetContextOrgId();
                }

                idRequest.Id = organizationId;
                _logger.Info("Organization get all function called ");
                var orgResponse = await _organizationClient.GetAllAsync(idRequest);
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAll)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getallcontextorganizations")]
        public async Task<IActionResult> GetAllOrganizationsForContext()
        {
            try
            {
                var orgResponse = await _organizationClient.GetAllOrganizationsForContextAsync(new Google.Protobuf.WellKnownTypes.Empty());
                if (orgResponse.Code == OrganizationBusinessService.Responcecode.Success)
                    return Ok(orgResponse.ContextOrgs);
                else if (orgResponse.Code == OrganizationBusinessService.Responcecode.NotFound)
                    return NotFound("Organizations detail not found.");
                else
                    return StatusCode(500, orgResponse.Message);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAllOrganizationsForContext)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        //Begin Account Preference
        [HttpPost]
        [Route("preference/create")]
        public async Task<IActionResult> CreatePreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0) || (request.PageRefreshTime <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId ,PageRefreshTime is required");
                }
                request.PreferenceType = "O";
                request.RefId = GetContextOrgId();
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.CreatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
         "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
         "CreatePreference  method in Organnization controller", 0, preference.AccountPreference.Id, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                      "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                      "CreatePreference  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(CreatePreference)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }


        [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdatePreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Preference Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                request.PreferenceType = "O";
                //Assign context orgId
                request.RefId = GetContextOrgId();
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = null;
                if (request.Id == 0)
                    preference = await _accountClient.CreatePreferenceAsync(accountPreference);
                else
                    preference = await _accountClient.UpdatePreferenceAsync(accountPreference);

                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                    "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "UpdatePreference  method in Organization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                   "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                   "UpdatePreference  method in Organnization controller", request.Id, request.Id, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(UpdatePreference)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeletePreference(int preferenceId)
        {
            AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
            try
            {

                // Validation                 
                if (preferenceId <= 0)
                {
                    return StatusCode(400, "The preferenceId Id is required");
                }
                request.Id = preferenceId;
                AccountBusinessService.AccountPreferenceResponse response = await _accountClient.DeletePreferenceAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                   "Organization service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                   "DeletePreference  method in Organnization controller", preferenceId, preferenceId, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok("Preference Deleted.");
                }
                else if (response != null && response.Code == AccountBusinessService.Responcecode.NotFound)
                {
                    return StatusCode(404, "Preference not found.");
                }
                else
                {
                    return StatusCode(500, "preference is null" + response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                  "Organization service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                  "DeletePreference  method in Organnization controller", preferenceId, preferenceId, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(DeletePreference)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("preference/get")]
        public async Task<IActionResult> GetPreference(int organizationId)
        {
            try
            {


                OrganizationBusinessService.IdRequest idRequest = new OrganizationBusinessService.IdRequest();
                idRequest.Id = organizationId;
                OrganizationBusinessService.OrganizationPreferenceResponse objResponse = new OrganizationBusinessService.OrganizationPreferenceResponse();
                // Validation                 
                if ((organizationId <= 0))
                {
                    return StatusCode(400, "Organization Id is required");
                }
                //Assign context orgId
                organizationId = GetContextOrgId();

                objResponse = await _organizationClient.GetPreferenceAsync(idRequest);
                return Ok(objResponse.OrganizationPreference);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetPreference)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }
        // End - Account Preference    


        ////Organization relationships 
        ///
        [HttpPost]
        [Route("orgrelationship/Create")]
        public async Task<IActionResult> CreateOrgRelationShip(OrganizationRelationShipCreate request)
        {
            try
            {

                if ((request.TargetOrgId.Count() <= 0))
                {
                    return StatusCode(400, "Select atleast 1 organization");
                }
                if (request.VehicleGroupId.Count() <= 0)
                {
                    return StatusCode(400, "Select atleast 1 Vehicle group");
                }
                if (request.RelationShipId == 0)
                {
                    return StatusCode(400, "RelationShip id required.");
                }
                //Assign context orgId               
                request.OwnerOrgId = GetContextOrgId();
                request.CreatedOrgId = GetContextOrgId();
                OrgRelationshipCreateRequest objRelationship = new OrgRelationshipCreateRequest();
                objRelationship.RelationShipId = request.RelationShipId;
                objRelationship.VehicleGroupID.Add(request.VehicleGroupId);
                objRelationship.OwnerOrgId = request.OwnerOrgId;
                objRelationship.CreatedOrgId = request.CreatedOrgId;
                objRelationship.TargetOrgId.Add(request.TargetOrgId);
                objRelationship.AllowChain = request.Allow_chain;
                objRelationship.Isconfirmed = request.IsConfirm;
                var createResponse = await _organizationClient.CreateOrgRelationshipAsync(objRelationship);
                if (createResponse.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                  "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "CreateOrgRelationShip method in Organization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(createResponse);
                }
                if (createResponse.Code == OrganizationBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, createResponse);
                }
                else if (createResponse.Code == OrganizationBusinessService.Responcecode.Forbidden)
                {
                    return StatusCode(403, createResponse);
                }
                else
                {
                    return StatusCode(500, "Error in creating organization relationship.");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                 "Organization service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "CreateOrgRelationShip  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(CreateOrgRelationShip)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("orgrelationship/EndRelation")]
        public async Task<IActionResult> EndOrganizationRelationShip(int[] Relationshipid)
        {
            EndOrgRelationShipRequest request = new EndOrgRelationShipRequest();

            try
            {
                if (Relationshipid.Count() <= 0)
                {
                    return StatusCode(400, "Select atleast 1 relationship");
                }
                request.OrgRelationShipid.Add(Relationshipid);
                var UpdateResponce = await _organizationClient.EndOrgRelationShipAsync(request);
                if (UpdateResponce.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "EndOrganizationRelationShip  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(UpdateResponce);
                }
                else
                {
                    return StatusCode(500, "Error in creating relationships");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
               "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
               "EndOrganizationRelationShip  method in Organnization controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error($"{nameof(EndOrganizationRelationShip)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("orgrelationship/AllowChain")]
        public async Task<IActionResult> AllowChaining(ChainingRequest request)
        {
            try
            {
                if (request.OrgRelationID == 0)
                {
                    return StatusCode(400, "Select atleast 1 organization");
                }

                var UpdateResponce = await _organizationClient.AllowChainingAsync(request);
                if (UpdateResponce.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                         "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                         "AllowChaining  method in Organnization controller", request.OrgRelationID, request.OrgRelationID, JsonConvert.SerializeObject(request), _userDetails);
                    UpdateResponce.Message = "Updated allow chain";
                    return Ok(UpdateResponce);
                }
                else
                {
                    return StatusCode(500, "Error in updating relationships");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Organization Component",
                       "Organization service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                       "AllowChaining  method in Organnization controller", request.OrgRelationID, request.OrgRelationID, JsonConvert.SerializeObject(request), _userDetails);

                _logger.Error($"{nameof(AllowChaining)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("orgrelationship/Getorgrelationdetails")]
        public async Task<IActionResult> GetOrganizationDetails(int organizationId)
        {
            try
            {
                //Assign context orgId
                organizationId = GetContextOrgId();
                VehicleBusinessService.OrganizationIdRequest organizationIdRequest = new VehicleBusinessService.OrganizationIdRequest();
                organizationIdRequest.OrganizationId = Convert.ToInt32(organizationId);
                VehicleBusinessService.OrgVehicleGroupListResponse vehicleResponse = await _vehicleClient.GetVehicleGroupsForOrgRelationshipMappingAsync(organizationIdRequest);

                //get Organizations List
                var idRequest = new IdRequest();
                idRequest.Id = 0;
                var organizationList = await _organizationClient.GetAllAsync(idRequest);

                // Get Relations
                RelationshipCreateRequest request = new RelationshipCreateRequest();
                request.Level = _userDetails.RoleLevel;
                var relationList = await _organizationClient.GetRelationshipAsync(request);

                RelationShipMappingDetails details = new RelationShipMappingDetails();
                details.VehicleGroup = new List<VehileGroupData>();
                details.OrganizationData = new List<OrganizationData>();
                details.RelationShipData = new List<RelationshipData>();
                foreach (var item in vehicleResponse.OrgVehicleGroupList)
                {
                    details.VehicleGroup.Add(new VehileGroupData
                    {
                        VehiclegroupID = Convert.ToInt32(item.VehicleGroupId == null ? 0 : item.VehicleGroupId),
                        GroupName = item.VehicleGroupName
                    });
                }
                foreach (var item in organizationList.OrganizationList)
                {
                    if (item.Id != organizationId)
                    {
                        details.OrganizationData.Add(new OrganizationData
                        {
                            OrganizationId = Convert.ToInt32(item.Id),
                            OrganizationName = item.Name
                        });
                    }
                }

                foreach (var item in relationList.RelationshipList)
                {
                    if (!(item.Code.ToLower().Equals("owner") || item.Code.ToLower().Equals("oem")))
                    {
                        details.RelationShipData.Add(new RelationshipData
                        {
                            RelationId = Convert.ToInt32(item.Id),
                            RelationName = item.Name
                        });
                    }
                }
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetOrganizationDetails)}: With Error:-", ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("orgrelationship/get")]
        public async Task<IActionResult> GetRelationshipMapping([FromQuery] OrganizationMappingFilter filterRequest)
        {
            try
            {
                //Assign context orgid
                filterRequest.Created_org_id = GetContextOrgId();
                var request = new OrgRelationshipMappingGetRequest()
                {
                    Id = filterRequest.Id,
                    TargetOrgId = filterRequest.Target_org_id,
                    CreatedOrgId = filterRequest.Created_org_id,
                    RelationShipId = filterRequest.Relationship_id,
                    VehicleGroupID = filterRequest.Vehicle_group_id
                };
                _logger.Info("Organization relationship mapping get function called ");
                var orgResponse = await _organizationClient.GetOrgRelationshipMappingAsync(request);
                //orgResponse.RelationshipList.Where(S => S.Featuresetid > 0)
                //                               .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.Featuresetid).Result); return S; }).ToList();
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetRelationshipMapping)}: With Error:-", ex);

                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getallorganizations")]
        public async Task<IActionResult> GetAllOrganizations([FromQuery] OrganizationBusinessService.OrganizationByID objOrganizationByID)
        {
            try
            {
                _logger.Info("Organization GetAllOrganizations function called ");
                //Assign context orgId
                objOrganizationByID.Id = GetContextOrgId();

                if (objOrganizationByID.RoleId == 0)
                {
                    var request = Request;
                    var Headers = request.Headers;
                    objOrganizationByID.RoleId = Convert.ToInt32(Headers["roleid"]);
                }
                var data = await _organizationClient.GetAllOrganizationsAsync(objOrganizationByID);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAllOrganizations)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("GetOrganizations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<IActionResult> GetOrganizations(int OrganizationId)
        {
            try
            {
                //Assign context orgId
                OrganizationId = GetContextOrgId();
                _logger.Info("Organization GetOrganizations function called ");
                IdRequest idRequest = new IdRequest();
                idRequest.Id = OrganizationId;
                var data = await _organizationClient.GetOrganizationsAsync(idRequest);
                return Ok(data.Organizations);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetOrganizations)}: With Error:-", ex);
                return StatusCode(500, OrganizationConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getlevel")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            try
            {
                //Assign context orgId
                orgId = GetUserSelectedOrgId();
                LevelByRoleRequest request = new LevelByRoleRequest();
                request.OrgId = orgId;
                request.RoleId = roleId;
                LevelResponse response = await _organizationClient.GetLevelByRoleIdAsync(request);
                if (response != null && response.Code == organizationservice.Responcecode.Success)
                {
                    return response.Level;
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }
    }
}



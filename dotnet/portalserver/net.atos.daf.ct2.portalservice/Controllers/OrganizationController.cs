using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganizationBusinessService = net.atos.daf.ct2.organizationservice;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.portalservice.Entity.Organization;
using AccountBusinessService = net.atos.daf.ct2.accountservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.organizationservice;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Entity.Relationship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("organization")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationController : ControllerBase
    {
        private readonly ILogger<OrganizationController> logger;
        private readonly OrganizationMapper _mapper;
        private readonly FeatureSetMapper _featureSetMapper;
        private readonly FeatureService.FeatureServiceClient _featureclient;
        private readonly RelationshipMapper _relationshipMapper;
        private readonly AccountBusinessService.AccountService.AccountServiceClient _accountClient;
        private readonly OrganizationBusinessService.OrganizationService.OrganizationServiceClient organizationClient;
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        public OrganizationController(ILogger<OrganizationController> _logger,
                                      OrganizationService.OrganizationServiceClient _organizationClient,
                                      AccountBusinessService.AccountService.AccountServiceClient accountClient,
                                      FeatureService.FeatureServiceClient featureclient,
                                      VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient)
        {
            logger = _logger;
            organizationClient = _organizationClient;
            _accountClient = accountClient;
            _mapper = new OrganizationMapper();
            _relationshipMapper = new RelationshipMapper();
            _featureSetMapper = new FeatureSetMapper(featureclient);
            _vehicleClient = vehicleClient;
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
                    logger.LogInformation("Relationship create function called ");
                    if (request.OrganizationId == 0)
                    {
                        return StatusCode(400, "Please provide OrganizationId:");
                    }
                    if (string.IsNullOrEmpty(request.Name) || (request.Name.Trim().Length < 1))
                    {
                        return StatusCode(400, "Please provide relationship name:");
                    }

                    var objRequest = _relationshipMapper.ToRelationshipRequest(request);
                    var orgResponse = await organizationClient.CreateRelationshipAsync(objRequest);
                    if (orgResponse.Relationship.Id < 1)
                    {
                        return StatusCode(400, "Relationship not created. ");
                    }
                    else
                    {
                        return Ok("Relationship Created :" + orgResponse);
                    }
                }
                else
                {
                    return StatusCode(500, "Featureset id not created");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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
                    logger.LogInformation("Relationship update function called ");
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
                    var objRequest = _relationshipMapper.ToRelationshipRequest(request);
                    var orgResponse = await organizationClient.UpdateRelationshipAsync(objRequest);
                    if (orgResponse.Relationship.Id < 1)
                    {
                        return StatusCode(400, "Relationship not updated :" + request.Id);
                    }
                    else
                    {
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
                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }



        [HttpGet]
        [Route("relationship/get")]
        public async Task<IActionResult> GetRelationship([FromQuery] RelationshipCreateRequest request)
        {
            try
            {
                logger.LogInformation("Organization relationship get function called ");
                var orgResponse = await organizationClient.GetRelationshipAsync(request);
                orgResponse.RelationshipList.Where(S => S.Featuresetid > 0)
                                               .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.Featuresetid).Result); return S; }).ToList();
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("relationship/getlevelcode")]
        public async Task<IActionResult> GetRelationshipLevelCode()
        {
            try
            {


                var levelCode = new RelationshipLevelCode();
                levelCode.Levels = Enum.GetValues(typeof(RelationshipLevel))
                     .Cast<RelationshipLevel>()
                     .Select(t => new Level
                     {
                         Id = ((int)t),
                         Name = t.ToString()
                     }).ToList();

                levelCode.Codes = Enum.GetValues(typeof(RelationshipCode))
                     .Cast<RelationshipCode>()
                     .Select(t => new Code
                     {
                         Id = ((int)t),
                         Name = t.ToString()
                     }).ToList();

                logger.LogInformation("Relationship get level and code function called ");

                return Ok(levelCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("relationship/delete")]
        public async Task<IActionResult> DeleteRelationship(int relationshipId)
        {
            try
            {
                // Validation                 
                if (relationshipId <= 0)
                {
                    return StatusCode(400, "OrgRelationship id is required.");
                }
                var relationshipRequest = new RelationshipDeleteRequest();
                relationshipRequest.Id = relationshipId;
                var response = await organizationClient.DeleteRelationshipAsync(relationshipRequest);
                response.RelationshipRequest = relationshipRequest;
                if (response != null && response.Code == organizationservice.Responcecode.Success)
                    return Ok(response);
                else if (response.Code == organizationservice.Responcecode.Conflict)
                {
                    return StatusCode(409, "Relationship cannot be deleted as it is mapped with organiztion.");
                }
                else { return StatusCode(404, "Relationship not configured."); }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in Relationship service:delete Relationship with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        //Organization

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> create(OrganizationRequest request)
        {
            try
            {
                Organization organization = new Organization();
                logger.LogInformation("Organization create function called ");
                if (string.IsNullOrEmpty(request.org_id) || (request.org_id.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization org_id:");
                }
                if (string.IsNullOrEmpty(request.name) || (request.name.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization name:");
                }
                organization.OrganizationId = request.org_id;
                organization.Name = request.name;
                organization.Type = request.type;
                organization.AddressType = request.address_type;
                organization.AddressStreet = request.street_number;
                organization.AddressStreetNumber = request.street_number;
                organization.City = request.city;
                organization.CountryCode = request.country_code;
                organization.PostalCode = request.postal_code;
                organization.reference_date = request.reference_date;
                var objRequest = _mapper.ToOragnizationRequest(request);
                OrganizationBusinessService.OrganizationCreateData orgResponse = await organizationClient.CreateAsync(objRequest);
                if (orgResponse.Organization.Id < 1)
                {
                    return StatusCode(400, "This organization is already exist :" + request.org_id);
                }
                else
                {
                    return Ok(orgResponse);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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

                logger.LogInformation("Organization update function called ");
                if (request.Id < 1)
                {
                    return StatusCode(400, "Please provide organization ID:");
                }

                if (string.IsNullOrEmpty(request.org_id) || (request.org_id.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization org_id:");
                }
                if (string.IsNullOrEmpty(request.name) || (request.name.Trim().Length < 1))
                {
                    return StatusCode(400, "Please provide organization name:");
                }

                organization.Id = request.Id;
                organization.OrganizationId = request.org_id;
                organization.Name = request.name;
                organization.Type = request.type;
                organization.AddressType = request.address_type;
                organization.AddressStreet = request.street_number;
                organization.AddressStreetNumber = request.street_number;
                organization.City = request.city;
                organization.CountryCode = request.country_code;
                organization.reference_date = request.reference_date;

                OrganizationBusinessService.OrganizationUpdateData orgResponse = await organizationClient.UpdateAsync(objRequest);
                if (orgResponse.Organization.Id == 0)
                {
                    return StatusCode(400, "Organization ID not exist: " + request.Id);
                }
                else if (orgResponse.Organization.Id == -1)
                {
                    return StatusCode(400, "This organization is already exist :" + request.org_id);
                }
                else
                {
                    return Ok(orgResponse);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                //  await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Organization Component","Organization Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.FAILED,"Update method in organization manager",0,0,JsonConvert.SerializeObject(request));      
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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
                idRequest.Id = organizationId;
                // OrganizationBusinessService.OrganizationGetData orgResponse = await organizationClient.GetAsync(idRequest);
                // return Ok("Organization Created :" +orgResponse); 
                logger.LogInformation("Organization get function called ");
                if (organizationId < 1)
                {
                    return StatusCode(400, "Please provide organization ID:");
                }
                OrganizationBusinessService.OrganizationGetData orgResponse = await organizationClient.GetAsync(idRequest);

                return Ok(orgResponse.Organization);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                //return StatusCode(500,"Internal Server Error.");
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAll(int organizationId)
        {
            try
            {
                var idRequest = new IdRequest();
                idRequest.Id = organizationId;
                logger.LogInformation("Organization get all function called ");
                var orgResponse = await organizationClient.GetAllAsync(idRequest);
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Account Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.CreatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPost]
        [Route("preference/update")]
        public async Task<IActionResult> UpdatePreference(AccountPreferenceRequest request)
        {
            try
            {
                // Validation                 
                if ((request.Id <= 0) || (request.LanguageId <= 0) || (request.TimezoneId <= 0) || (request.CurrencyId <= 0) ||
                    (request.UnitId <= 0) || (request.VehicleDisplayId <= 0) || (request.DateFormatTypeId <= 0) || (request.TimeFormatId <= 0) ||
                    (request.LandingPageDisplayId <= 0)
                    )
                {
                    return StatusCode(400, "The Preference Id, LanguageId, TimezoneId, CurrencyId, UnitId, VehicleDisplayId,DateFormatId, TimeFormatId, LandingPageDisplayId is required");
                }
                var accountPreference = _mapper.ToAccountPreference(request);
                AccountBusinessService.AccountPreferenceResponse preference = await _accountClient.UpdatePreferenceAsync(accountPreference);
                if (preference != null && preference.Code == AccountBusinessService.Responcecode.Success)
                {
                    return Ok(_mapper.ToAccountPreference(preference.AccountPreference));
                }
                else
                {
                    return StatusCode(500, "preference is null" + preference.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("preference/delete")]
        public async Task<IActionResult> DeletePreference(int preferenceId)
        {
            try
            {
                AccountBusinessService.IdRequest request = new AccountBusinessService.IdRequest();
                // Validation                 
                if (preferenceId <= 0)
                {
                    return StatusCode(400, "The preferenceId Id is required");
                }
                request.Id = preferenceId;
                AccountBusinessService.AccountPreferenceResponse response = await _accountClient.DeletePreferenceAsync(request);
                if (response != null && response.Code == AccountBusinessService.Responcecode.Success)
                {
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
                logger.LogError("Error in account service:create preference with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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

                objResponse = await organizationClient.GetPreferenceAsync(idRequest);
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                logger.LogError("Error in organization service: get preference with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
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
                OrgRelationshipCreateRequest objRelationship = new OrgRelationshipCreateRequest();
                objRelationship.RelationShipId = request.RelationShipId;
                objRelationship.VehicleGroupID.Add(request.VehicleGroupId);
                objRelationship.OwnerOrId = request.OwnerOrgId;
                objRelationship.CreatedOrgId = request.CreatedOrgId;
                objRelationship.TargetOrgId.Add(request.TargetOrgId);
                objRelationship.AllowChain = request.allow_chain;
                var CreateResponce = await organizationClient.CreateOrgRelationshipAsync(objRelationship);
                if (CreateResponce.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    return Ok(CreateResponce);
                }
                else
                {
                    return StatusCode(500, "Error in creating relationships");
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:create orgRelations with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("orgrelationship/EndRelation")]
        public async Task<IActionResult> EndOrganizationRelationShip(int[] Relationshipid)
        {
            try
            {
                if (Relationshipid.Count() <= 0)
                {
                    return StatusCode(400, "Select atleast 1 relationship");
                }
                EndOrgRelationShipRequest request = new EndOrgRelationShipRequest();
                request.OrgRelationShipid.Add(Relationshipid);
                var UpdateResponce = await organizationClient.EndOrgRelationShipAsync(request);
                if (UpdateResponce.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    return Ok(UpdateResponce);
                }
                else
                {
                    return StatusCode(500, "Error in creating relationships");
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:End relation with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
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
              
                var UpdateResponce = await organizationClient.AllowChainingAsync(request);
                if (UpdateResponce.Code == OrganizationBusinessService.Responcecode.Success)
                {
                    return Ok(UpdateResponce);
                }
                else
                {
                    return StatusCode(500, "Error in creating relationships");
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:Allow chaining with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("orgrelationship/Getorgrelationdetails")]
        public async Task<IActionResult> GetOrganizationDetails(int OrganizationId)
        {
            try
            {
                //if (OrganizationId == 0)
                //{
                //    return StatusCode(400, "Select atleast 1 organization");
                //}

                //get vehicle group
                //VehicleBusinessService.OrgvehicleIdRequest orgvehicleIdRequest = new VehicleBusinessService.OrgvehicleIdRequest();
                //orgvehicleIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                //orgvehicleIdRequest.VehicleId = Convert.ToInt32(0);
                //VehicleBusinessService.VehicleGroupDetailsResponse response = await _vehicleClient.GetVehicleGroupAsync(orgvehicleIdRequest);

                VehicleBusinessService.OrganizationIdRequest OrganizationIdRequest = new VehicleBusinessService.OrganizationIdRequest();
                OrganizationIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                VehicleBusinessService.OrgVehicleGroupListResponse Vehicleresponse = await _vehicleClient.GetOrganizationVehicleGroupdetailsAsync(OrganizationIdRequest);
               
                
                //get Organizations List
                var idRequest = new IdRequest();
                idRequest.Id = 0;
                var OrganizationList = await organizationClient.GetAllAsync(idRequest);
               
                
                // Get Relations
                RelationshipCreateRequest request = new RelationshipCreateRequest();
                var RelationList = await organizationClient.GetRelationshipAsync(request);

                //var result = _relationshipMapper.MaprelationData(RelationList.RelationshipList, response, OrganizationList.OrganizationList);

                RelationShipMappingDetails details = new RelationShipMappingDetails();
                details.VehicleGroup = new List<VehileGroupData>();
                details.OrganizationData = new List<OrganizationData>();
                details.RelationShipData = new List<RelationshipData>();
                foreach (var item in Vehicleresponse.OrgVehicleGroupList.Where(I=> I.IsGroup == true))
                {
                    
                    details.VehicleGroup.Add(new VehileGroupData
                        { VehiclegroupID = Convert.ToInt32(item.VehicleGroupId == null ? 0 : item.VehicleGroupId),
                            GroupName=item.VehicleGroupName});
                }
                foreach (var item in OrganizationList.OrganizationList)
                {
                    
                    details.OrganizationData.Add(new OrganizationData
                    {
                        OrganizationId = Convert.ToInt32(item.Id),
                        OrganizationName = item.Name
                    });
                }
                foreach (var item in RelationList.RelationshipList)
                {
                    
                    details.RelationShipData.Add(new RelationshipData
                    {
                        RelationId = Convert.ToInt32(item.Id),
                        RelationName = item.Name
                    });
                }
                return Ok(details);

            }
            catch (Exception ex)
            {
                logger.LogError("Error in account service:Allow chaining with exception - " + ex.Message + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("orgrelationship/get")]
        public async Task<IActionResult> GetRelationshipMapping([FromQuery] OrganizationMappingFilter filterRequest)
        {
            try
            {
                var request = new OrgRelationshipMappingGetRequest()
                {
                    Id = filterRequest.Id,
                    TargetOrgId = filterRequest.target_org_id,
                    CreatedOrgId = filterRequest.created_org_id,
                    RelationShipId = filterRequest.relationship_id,
                    VehicleGroupID = filterRequest.vehicle_group_id
                };
                logger.LogInformation("Organization relationship mapping get function called ");
                var orgResponse = await organizationClient.GetOrgRelationshipMappingAsync(request);
                //orgResponse.RelationshipList.Where(S => S.Featuresetid > 0)
                //                               .Select(S => { S.FeatureIds.AddRange(_featureSetMapper.GetFeatureIds(S.Featuresetid).Result); return S; }).ToList();
                return Ok(orgResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);

                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getallorganizations")]
        public async Task<IActionResult> GetAllOrganizations([FromQuery] OrganizationBusinessService.OrganizationByID objOrganizationByID)
        {
            try
            {
                logger.LogInformation("Organization GetAllOrganizations function called ");
                
                var data = await organizationClient.GetAllOrganizationsAsync(objOrganizationByID);
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception in Organization GetAllOrganizations {ex.Message} {ex.StackTrace}");
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}



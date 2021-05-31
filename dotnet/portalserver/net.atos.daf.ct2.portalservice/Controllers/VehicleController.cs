using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Vehicle;
using Newtonsoft.Json;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class VehicleController : BaseController
    {
        //private readonly ILogger<VehicleController> _logger;
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private readonly Mapper _mapper;

        private ILog _logger;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly AuditHelper _auditHelper;

        public VehicleController(VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient, AuditHelper auditHelper, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _vehicleClient = vehicleClient;
            _mapper = new Mapper();
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }


        //[HttpPost]
        //[Route("create")]
        //public async Task<IActionResult> Create(VehicleCreateRequest request)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Create method in vehicle API called.");


        //        if (string.IsNullOrEmpty(request.Name))
        //        {
        //            return StatusCode(401, "invalid Vehicle Name: The Vehicle Name is Empty.");
        //        }
        //        if (string.IsNullOrEmpty(request.License_Plate_Number))
        //        {
        //            return StatusCode(401, "invalid Vehicle License Plate Number: The Vehicle License Plate Number is Empty.");
        //        }
        //        if (string.IsNullOrEmpty(request.VIN))
        //        {
        //            return StatusCode(401, "invalid Vehicle VIN: The Vehicle VIN is Empty.");
        //        }

        //        var vehicleRequest = _mapper.ToVehicleCreate(request);
        //        VehicleBusinessService.VehicleCreateResponce vehicleResponse = await _vehicleClient.CreateAsync(vehicleRequest);
        //        var response = _mapper.ToVehicleCreate(vehicleResponse.Vehicle);

        //        if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
        //           && vehicleResponse.Message == "There is an error creating account.")
        //        {
        //            return StatusCode(500, "There is an error creating account.");
        //        }
        //        else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
        //        {
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            return StatusCode(500, "accountResponse is null");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
        //        // check for fk violation
        //        if (ex.Message.Contains(FK_Constraint))
        //        {
        //            return StatusCode(500, "Internal Server Error.(01)");
        //        }
        //        // check for fk violation
        //        if (ex.Message.Contains(SocketException))
        //        {
        //            return StatusCode(500, "Internal Server Error.(02)");
        //        }
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}




        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(VehicleRequest request)
        {
            try
            {
                _logger.Info("Update method in vehicle API called.");

                // Validation 
                if (request.ID <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }

                if (request.Organization_Id <= 0)
                {
                    return StatusCode(400, "The organization id is required.");
                }
                //Assign context orgId
                request.Organization_Id = GetContextOrgId();

                var vehicleRequest = new VehicleBusinessService.VehicleRequest();
                vehicleRequest = _mapper.ToVehicle(request);
                VehicleBusinessService.VehicleResponce vehicleResponse = await _vehicleClient.UpdateAsync(vehicleRequest);

                if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, vehicleResponse.Message);
                }
                else if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                  "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
                   Request);

                    var response = _mapper.ToVehicle(vehicleResponse.Vehicle);
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                 "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
                  Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] VehicleFilterRequest vehicleFilter)
        {
            try
            {
                _logger.Info("Get method in vehicle API called.");
                //Assign context orgId               
                vehicleFilter.OrganizationId = GetContextOrgId();
                var vehicleFilterRequest = _mapper.ToVehicleFilter(vehicleFilter);
                VehicleBusinessService.VehicleListResponce vehicleListResponse = await _vehicleClient.GetAsync(vehicleFilterRequest);
                List<VehicleResponse> response = new List<VehicleResponse>();
                response = _mapper.ToVehicles(vehicleListResponse);

                if (vehicleListResponse != null && vehicleListResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehicleListResponse.Vehicles != null && vehicleListResponse.Vehicles.Count > 0)
                    {
                        return Ok(vehicleListResponse.Vehicles);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehicleListResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("group/create")]
        public async Task<IActionResult> CreateGroup(VehicleGroupRequest group)
        {
            try
            {
                _logger.Info("Create Group method in vehicle API called.");


                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.CreateRequired);
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.InvalidData);
                }

                char groupType = Convert.ToChar(group.GroupType);
                if (!EnumValidator.ValidateGroupType(groupType))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.InvalidGroupType);
                }

                //Assign context orgId
                group.OrganizationId = GetContextOrgId();

                VehicleBusinessService.VehicleGroupRequest accountGroupRequest = new VehicleBusinessService.VehicleGroupRequest();


                accountGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                   "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                   "CreateGroup  method in Vehicle controller", 0, response.VehicleGroup.Id, JsonConvert.SerializeObject(group),
                    Request);
                    return Ok(_mapper.ToVehicleGroup(response));
                }
                else if (response != null && response.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Vehicle Group.");
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is empty " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                      "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                      "CreateGroup  method in Vehicle controller", 0, 0, JsonConvert.SerializeObject(group), Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPut]
        [Route("group/update")]
        public async Task<IActionResult> UpdateGroup(VehicleGroupRequest group)
        {
            try
            {
                _logger.Info("Update Group method in vehicle API called.");

                if (group.Id == 0)
                {
                    return StatusCode(401, "invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.CreateRequired);
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.InvalidData);
                }

                //Assign context orgId
                group.OrganizationId = GetContextOrgId();

                char groupType = Convert.ToChar(group.GroupType);
                if (!EnumValidator.ValidateGroupType(groupType))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.InvalidGroupType);
                }


                VehicleBusinessService.VehicleGroupRequest vehicleGroupRequest = new VehicleBusinessService.VehicleGroupRequest();
                vehicleGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.UpdateGroupAsync(vehicleGroupRequest);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                     "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "UpdateGroup  method in Vehicle controller", group.Id, group.Id, JsonConvert.SerializeObject(group), Request);
                    return Ok(_mapper.ToVehicleGroup(response));
                }
                else if (response != null && response.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Vehicle Group.");
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is null " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                    "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                    "UpdateGroup  method in Vehicle controller", group.Id, group.Id, JsonConvert.SerializeObject(group), Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("group/delete")]
        public async Task<IActionResult> DeleteGroup(long GroupId)
        {
            VehicleBusinessService.VehicleGroupIdRequest request = new VehicleBusinessService.VehicleGroupIdRequest();
            try
            {
                _logger.Info("Delete Group method in vehicle API called.");

                if ((Convert.ToInt32(GroupId) <= 0))
                {
                    return StatusCode(400, "The vehicle group id is required.");
                }

                request.GroupId = Convert.ToInt32(GroupId);
                VehicleBusinessService.VehicleGroupDeleteResponce response = await _vehicleClient.DeleteGroupAsync(request);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                  "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "DeleteGroup  method in Vehicle controller", Convert.ToInt32(GroupId), Convert.ToInt32(GroupId), JsonConvert.SerializeObject(request), Request);
                    return Ok(response.Result);
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is null " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "DeleteGroup  method in Vehicle controller", Convert.ToInt32(GroupId), Convert.ToInt32(GroupId), JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpPost]
        [Route("group/getgroupdetails")]
        public async Task<IActionResult> GetGroupDetails(VehicleGroupFilterRequest groupFilter)
        {
            try
            {
                _logger.Info("Get Group detais method in vehicle API called.");
                //Assign context orgId
                groupFilter.OrganizationId = GetContextOrgId();
                VehicleBusinessService.GroupFilterRequest VehicleGroupRequest = new VehicleBusinessService.GroupFilterRequest();
                VehicleGroupRequest = _mapper.ToVehicleGroupFilter(groupFilter);
                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetGroupDetailsAsync(VehicleGroupRequest);

                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("group/getvehiclelist")]
        public async Task<IActionResult> GetVehiclesByVehicleGroup([FromQuery] int GroupId)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (Convert.ToInt32(GroupId) <= 0)
                {
                    return StatusCode(401, "invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }


                VehicleBusinessService.VehicleGroupIdRequest VehicleGroupIdRequest = new VehicleBusinessService.VehicleGroupIdRequest();
                VehicleGroupIdRequest.GroupId = GroupId;

                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByVehicleGroupAsync(VehicleGroupIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("organization/get")]
        public async Task<IActionResult> GetOrganizationVehicleGroupdetails([FromQuery] long OrganizationId)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }
                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.OrganizationIdRequest OrganizationIdRequest = new VehicleBusinessService.OrganizationIdRequest();
                OrganizationIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                VehicleBusinessService.OrgVehicleGroupListResponse response = await _vehicleClient.GetOrganizationVehicleGroupdetailsAsync(OrganizationIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.OrgVehicleGroupList != null && response.OrgVehicleGroupList.Count > 0)
                    {
                        return Ok(response.OrgVehicleGroupList);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("getGroup")]
        public async Task<IActionResult> GetVehicleGroup([FromQuery] int OrganizationId, int VehicleId)
        {
            try
            {
                _logger.Info("Get vehicle group list by orgnization & vehicle id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }
                if (Convert.ToInt32(VehicleId) <= 0)
                {
                    return StatusCode(401, "invalid vehicle ID: The vehicle Id is Empty.");
                }

                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.OrgvehicleIdRequest orgvehicleIdRequest = new VehicleBusinessService.OrgvehicleIdRequest();
                orgvehicleIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                orgvehicleIdRequest.VehicleId = Convert.ToInt32(VehicleId);
                VehicleBusinessService.VehicleGroupDetailsResponse response = await _vehicleClient.GetVehicleGroupAsync(orgvehicleIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.VehicleGroups != null && response.VehicleGroups.Count > 0)
                    {
                        return Ok(response.VehicleGroups);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle group details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("group/getvehicles")]
        public async Task<IActionResult> GetVehiclesByAccountGroup([FromQuery] int AccountGroupId, int Organization_Id)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (AccountGroupId == 0)
                {
                    return StatusCode(401, "invalid Account Group Id: The Account group id is Empty.");
                }
                //Assign context orgId
                Organization_Id = GetContextOrgId();

                VehicleBusinessService.OrgAccountIdRequest orgAccountIdRequest = new VehicleBusinessService.OrgAccountIdRequest();
                orgAccountIdRequest.OrganizationId = Convert.ToInt32(Organization_Id);
                orgAccountIdRequest.AccountGroupId = Convert.ToInt32(AccountGroupId);
                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByAccountGroupAsync(orgAccountIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("setoptinstatus")]
        public async Task<IActionResult> SetOptInStatus(VehicleBusinessService.VehicleOptInRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }

                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.SetOptInStatusAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle opt in status.")
                {
                    return StatusCode(500, "There is an error updating vehicle opt in status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "SetOptInStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);

                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
              "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
              "SetOptInStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("setotastatus")]
        public async Task<IActionResult> SetOTAStatus(VehicleBusinessService.VehicleOtaRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }

                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.SetOTAStatusAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle ota status.")
                {
                    return StatusCode(500, "There is an error updating vehicle ota status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
            "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
            "SetOTAStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
          "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
          "SetOTAStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("terminate")]
        public async Task<IActionResult> Terminate(VehicleBusinessService.VehicleTerminateRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }

                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.TerminateAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle terminate status.")
                {
                    return StatusCode(500, "There is an error updating vehicle terminate status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
       "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
       "Terminate  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
   "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
   "Terminate  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), Request);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getvehicle")]
        public async Task<IActionResult> GetVehicle([FromQuery] int vehicleId)
        {
            try
            {
                _logger.Info("Get method in vehicle API called.");

                VehicleBusinessService.VehicleIdRequest Vid = new VehicleBusinessService.VehicleIdRequest();
                Vid.VehicleId = vehicleId;
                VehicleBusinessService.VehicleDetailsResponce vehicleListResponse = await _vehicleClient.GetVehicleAsync(Vid);

                if (vehicleListResponse != null && vehicleListResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehicleListResponse.Vehicle != null)
                    {
                        return Ok(vehicleListResponse.Vehicle);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehicleListResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }



        [HttpGet]
        [Route("group/getvehiclegrouplist")]
        public async Task<IActionResult> GetGroupDetailsWithVehicleCount([FromQuery] int OrganizationId)
        {
            try
            {
                _logger.Info("Get Group detais method in vehicle API called.");

                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.VehicleGroupLandingRequest VehicleGroupRequest = new VehicleBusinessService.VehicleGroupLandingRequest();
                VehicleGroupRequest = _mapper.ToVehicleGroupLandingFilter(OrganizationId);
                VehicleBusinessService.VehicleGroupLandingResponse response = await _vehicleClient.GetVehicleGroupWithVehCountAsync(VehicleGroupRequest);

                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.VehicleGroupLandingDetails != null && response.VehicleGroupLandingDetails.Count > 0)
                    {
                        return Ok(response.VehicleGroupLandingDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("group/getvehiclesDetails")]
        public async Task<IActionResult> GetVehiclesByGroup([FromQuery] DynamicVehicleGroupRequest dynamicVehicleGroupRequest)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                //Assign context orgId
                dynamicVehicleGroupRequest.OrganizationId = GetContextOrgId();

                if (dynamicVehicleGroupRequest.GroupType != null ? EnumValidator.ValidateGroupType(Convert.ToChar(dynamicVehicleGroupRequest.GroupType)) : false)
                {

                    if (Convert.ToInt32(dynamicVehicleGroupRequest.GroupId) <= 0 && Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'g')
                    {
                        return StatusCode(401, PortalConstants.VehicleValidation.GroupIdRequired);
                    }
                    else
                    {
                        if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'g')
                        {

                            VehicleBusinessService.VehicleGroupIdRequest VehicleGroupIdRequest = new VehicleBusinessService.VehicleGroupIdRequest();
                            VehicleGroupIdRequest.GroupId = dynamicVehicleGroupRequest.GroupId;
                            VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByVehicleGroupAsync(VehicleGroupIdRequest);

                            if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                            {
                                if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                                {
                                    return Ok(response.GroupRefDetails);
                                }
                                else
                                {
                                    return StatusCode(404, "vehicle details are not found.");
                                }
                            }
                            else
                            {
                                return StatusCode(500, response.Message);
                            }
                        }
                    }

                    if (Convert.ToInt32(dynamicVehicleGroupRequest.OrganizationId) <= 0 && Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd')
                    {
                        return StatusCode(401, PortalConstants.VehicleValidation.OrganizationIdRequired);
                    }
                    else
                    {
                        char functionenumType;
                        if (!string.IsNullOrEmpty(dynamicVehicleGroupRequest.FunctionEnum))
                        {
                            functionenumType = Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum);
                        }
                        else
                        {
                            return StatusCode(401, PortalConstants.VehicleValidation.FunctionTypeRequired);
                        }

                        if (EnumValidator.ValidateFunctionEnumType(functionenumType))
                        {
                            VehicleBusinessService.VehicleListResponce response = null;

                            if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd' && Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum.ToLower().Trim()) == 'v')
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest DynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                DynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                DynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                DynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicVisibleVehicleAsync(DynamicVehicleGroupIdRequest);
                            }
                            else if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd' && Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum.ToLower().Trim()) == 'o')
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest DynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                DynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                DynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                DynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicOwnedVehicleAsync(DynamicVehicleGroupIdRequest);
                            }
                            else
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest DynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                DynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                DynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                DynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicAllVehicleAsync(DynamicVehicleGroupIdRequest);
                            }

                            if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                            {
                                if (response.Vehicles != null && response.Vehicles.Count > 0)
                                {
                                    return Ok(response.Vehicles);
                                }
                                else
                                {
                                    return StatusCode(404, "vehicle details are not found.");
                                }
                            }
                            else
                            {
                                return StatusCode(500, response.Message);
                            }
                        }
                        else
                        {
                            return StatusCode(401, PortalConstants.VehicleValidation.InvalidFunctionEnumType);
                        }
                    }
                }
                else
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.InvalidGroupType);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }



        [HttpGet]
        [Route("GetRelationshipVehicles")]
        public async Task<IActionResult> GetRelationshipVehicles([FromQuery] int OrganizationId, int VehicleId)
        {
            try
            {
                _logger.Info("GetRelationshipVehicles method in vehicle API called.");

                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.OrgvehicleIdRequest orgvehicleIdRequest = new VehicleBusinessService.OrgvehicleIdRequest();
                orgvehicleIdRequest.OrganizationId = OrganizationId;
                orgvehicleIdRequest.VehicleId = VehicleId;

                VehicleBusinessService.VehicleListResponce vehicleListResponse = await _vehicleClient.GetRelationshipVehiclesAsync(orgvehicleIdRequest);
                List<VehicleResponse> response = new List<VehicleResponse>();
                response = _mapper.ToVehicles(vehicleListResponse);

                if (vehicleListResponse != null && vehicleListResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehicleListResponse.Vehicles != null && vehicleListResponse.Vehicles.Count > 0)
                    {
                        return Ok(vehicleListResponse.Vehicles);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehicleListResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getvehiclebysubscriptionid/{subscriptionId}")]
        public async Task<IActionResult> GetVehicleBySubscriptionId([FromRoute] int subscriptionId)
        {
            try
            {
                _logger.Info("GetVehicleBySubscriptionId method in vehicle API called.");
                if (subscriptionId <= 0)
                {
                    return StatusCode(400, string.Empty);
                }
                VehicleBusinessService.subscriptionIdRequest Vid = new VehicleBusinessService.subscriptionIdRequest();
                Vid.SubscriptionId = subscriptionId;
                var response = await _vehicleClient.GetVehicleBySubscriptionIdAsync(Vid);

                if (response != null)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


    }

}

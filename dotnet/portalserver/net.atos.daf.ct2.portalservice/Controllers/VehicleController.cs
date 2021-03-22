using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.portalservice.Entity.Vehicle;
using System.Text;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    public class VehicleController : Controller
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        public VehicleController(ILogger<VehicleController> logger, VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient)
        {
            _logger = logger;
            _vehicleClient = vehicleClient;
            _mapper = new Mapper();
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
                _logger.LogInformation("Create method in vehicle API called.");

                // Validation 
                if (request.ID <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }
                var vehicleRequest = _mapper.ToVehicle(request);
                VehicleBusinessService.VehicleResponce vehicleResponse = await _vehicleClient.UpdateAsync(vehicleRequest);
                var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Update : " + ex.Message + " " + ex.StackTrace);
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
                _logger.LogInformation("Get method in vehicle API called.");

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
                _logger.LogError("Error in vehicle service:get vehicle with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("group/create")]
        public async Task<IActionResult> CreateGroup(VehicleGroupRequest group)
        {
            try
            {
                _logger.LogInformation("Create Group method in vehicle API called.");

                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, "invalid vehicle group name: The vehicle group name is Empty.");
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, "The vehicle group name and vehicle group description should be valid.");
                }


                VehicleBusinessService.VehicleGroupRequest accountGroupRequest = new VehicleBusinessService.VehicleGroupRequest();
                accountGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
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
                _logger.LogError("Error in vehicle service:create vehicle group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpPut]
        [Route("group/update")]
        public async Task<IActionResult> UpdateGroup(VehicleGroupRequest group)
        {
            try
            {
                _logger.LogInformation("Update Group method in vehicle API called.");

                if (group.Id == 0)
                {
                    return StatusCode(401, "invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, "invalid vehicle group name: The vehicle group name is Empty.");
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, "The vehicle group name and vehicle group description should be valid.");
                }

                VehicleBusinessService.VehicleGroupRequest vehicleGroupRequest = new VehicleBusinessService.VehicleGroupRequest();
                vehicleGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.UpdateGroupAsync(vehicleGroupRequest);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
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
                _logger.LogError("Error in vehicle service:update vehicle group with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("group/delete")]
        public async Task<IActionResult> DeleteGroup(long GroupId)
        {
            try
            {
                _logger.LogInformation("Delete Group method in vehicle API called.");

                if ((Convert.ToInt32(GroupId) <= 0))
                {
                    return StatusCode(400, "The vehicle group id is required.");
                }
                VehicleBusinessService.VehicleGroupIdRequest request = new VehicleBusinessService.VehicleGroupIdRequest();
                request.GroupId = Convert.ToInt32(GroupId);
                VehicleBusinessService.VehicleGroupDeleteResponce response = await _vehicleClient.DeleteGroupAsync(request);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    return Ok(response.Result);
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is null " + response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:DeleteGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpPost]
        [Route("group/getgroupdetails")]
        public async Task<IActionResult> GetGroupDetails(VehicleGroupFilterRequest groupFilter)
        {
            try
            {
                _logger.LogInformation("Get Group detais method in vehicle API called.");

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
                _logger.LogError("Vehicle Service:Get group details : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("group/getvehiclelist")]
        public async Task<IActionResult> GetVehiclesByVehicleGroup([FromQuery] int GroupId)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");

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
                _logger.LogError("Vehicle Service:Get vehicle list by group ID  : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("organization/get")]
        public async Task<IActionResult> GetOrganizationVehicleGroupdetails([FromQuery] long OrganizationId)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }

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
                _logger.LogError("Error in vehicle service:get vehicle details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("getGroup")]
        public async Task<IActionResult> GetVehicleGroup([FromQuery] int OrganizationId, int VehicleId)
        {
            try
            {
                _logger.LogInformation("Get vehicle group list by orgnization & vehicle id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }
                if (Convert.ToInt32(VehicleId) <= 0)
                {
                    return StatusCode(401, "invalid vehicle ID: The vehicle Id is Empty.");
                }

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
                _logger.LogError("Error in vehicle service:get vehicle details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("group/getvehicles")]
        public async Task<IActionResult> GetVehiclesByAccountGroup([FromQuery] int AccountGroupId, int Organization_Id)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");

                if (AccountGroupId == 0)
                {
                    return StatusCode(401, "invalid Account Group Id: The Account group id is Empty.");
                }

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
                _logger.LogError("Error in vehicle service:get vehicle details with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("setoptinstatus")]
        public async Task<IActionResult> SetOptInStatus(VehicleBusinessService.VehicleOptInRequest request)
        {
            try
            {
                _logger.LogInformation("Create method in vehicle API called.");

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
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:SetOptInStatus : " + ex.Message + " " + ex.StackTrace);
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
                _logger.LogInformation("Create method in vehicle API called.");

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
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:SetOTAStatus : " + ex.Message + " " + ex.StackTrace);
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
                _logger.LogInformation("Create method in vehicle API called.");

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
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Terminate : " + ex.Message + " " + ex.StackTrace);
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
                _logger.LogInformation("Get method in vehicle API called.");

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
                _logger.LogError("Error in vehicle service:get vehicle with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getvehiclebysubscriptionid/{subscriptionId}")]
        public async Task<IActionResult> GetVehicleBySubscriptionId([FromRoute] string subscriptionId)
        {
            try
            {
                _logger.LogInformation("GetVehicleBySubscriptionId method in vehicle API called.");

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
                _logger.LogError("Error in vehicle service:GetVehicleBySubscriptionId with exception - " + ex.Message + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }

}

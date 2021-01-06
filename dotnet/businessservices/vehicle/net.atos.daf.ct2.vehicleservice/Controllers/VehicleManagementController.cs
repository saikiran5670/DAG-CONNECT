using System;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehiclerepository;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.vehicleservice.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class VehicleManagementController : ControllerBase
    {
        private readonly ILogger<VehicleManagementController> logger;

        private readonly IVehicleManagement vehicleManagement;
        public VehicleManagementController(IVehicleManagement _vehicleManagement, ILogger<VehicleManagementController> _logger)
        {
            vehicleManagement = _vehicleManagement;
            logger = _logger;
        }


        [HttpPost]
        [Route("AddVehicle")]
         public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
            try
            {
                logger.LogInformation("Add Vehicle function called -" + vehicle.CreatedBy);
                if (string.IsNullOrEmpty(vehicle.VIN))
                {
                    return StatusCode(501, "The VIN  can not blank.");
                }
                if (string.IsNullOrEmpty(vehicle.RegistrationNo))
                {
                    return StatusCode(502, "The Registration No can not blank");
                }
                 if (string.IsNullOrEmpty(vehicle.ChassisNo))
                {
                    return StatusCode(503, "The Chassis No can not blank");
                }

                int vehicleId = await vehicleManagement.AddVehicle(vehicle);
                logger.LogInformation("Vehicle added with Vehicle Id - " + vehicleId);
                return Ok(vehicleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("UpdateVehicle")]
        public async Task<IActionResult> UpdateVehicle(Vehicle vehicle)
        {
            try
            {
                logger.LogInformation("UpdateVehicle function called -" + vehicle.UpdatedBy);
                 if (string.IsNullOrEmpty(vehicle.VIN))
                {
                    return StatusCode(501, "The VIN  can not blank.");
                }
                if (string.IsNullOrEmpty(vehicle.RegistrationNo))
                {
                    return StatusCode(502, "The Registration No can not blank");
                }
                 if (string.IsNullOrEmpty(vehicle.ChassisNo))
                {
                    return StatusCode(503, "The Chassis No can not blank");
                }

                int vehicleId = await vehicleManagement.UpdateVehicle(vehicle);
                logger.LogInformation("Vehicle updated with Vehicle Id - " + vehicleId);
                return Ok(vehicleId);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("DeleteVehicle")]
        public async Task<IActionResult> DeleteVehicle(int vehicleID, int userId)
        {
            try
            {
                logger.LogInformation("DeleteVehicle function called " + vehicleID + "userId -" + userId);
                if (string.IsNullOrEmpty(vehicleID.ToString()))
                {
                    return StatusCode(501, "The vehicle ID is Empty.");
                }

                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }

                int DeletedVehicleID = await vehicleManagement.DeleteVehicle(vehicleID, userId);
                logger.LogInformation("Vehicle deleted with Vehicle Id - " + vehicleID);

                return Ok(DeletedVehicleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("GetVehicleByID")]
         public async Task<IActionResult> GetVehicleByID(int vehicleID,int orgid)
        {
            try
            {
                logger.LogInformation("GetVehicle function called " + vehicleID);

                if (string.IsNullOrEmpty(vehicleID.ToString()))
                {
                    return StatusCode(501, "The vehicleID is Empty.");
                }

                return Ok(await vehicleManagement.GetVehicleByID(vehicleID,orgid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

         [HttpPost]
        [Route("AddVehicleGroup")]
        public async Task<IActionResult> AddVehicleGroup(VehicleGroup vehicleGroup)
        {
            try
            {
                logger.LogInformation("AddVehicleGroup function called -" + vehicleGroup.CreatedBy);
                if (string.IsNullOrEmpty(vehicleGroup.Name))
                {
                    return StatusCode(501, "Vehicle group name can not blank.");
                }               

                int vehicleGroupId =await vehicleManagement.AddVehicleGroup(vehicleGroup);
                logger.LogInformation("Vehicle group added with VehicleGroup Id - " + vehicleGroupId);
                return Ok(vehicleGroupId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
              //  return StatusCode(500, "Internal Server Error.");
                return StatusCode(500, p);
            }
        }

        [HttpPut]
        [Route("UpdateVehicleGroup")]
       public async Task<IActionResult> UpdateVehicleGroup(VehicleGroup vehicleGroup)
        {
            try
            {
                logger.LogInformation("UpdateVehicleGroup function called -" + vehicleGroup.UpdatedBy);
                 if (string.IsNullOrEmpty(vehicleGroup.Name))
                {
                   return StatusCode(501, "Vehicle group name can not blank.");
                }             

                int vehicleGroupId =await vehicleManagement.UpdateVehicleGroup(vehicleGroup);
                logger.LogInformation("Vehicle Group updated with Vehicle group Id - " + vehicleGroupId);
                return Ok(vehicleGroupId);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }
       
        [HttpPut]
        [Route("DeleteVehicleGroup")]
        public async Task<IActionResult> DeleteVehicleGroup(int vehicleGroupID, int userId)
        {
            try
            {
                logger.LogInformation("DeleteVehicleGroup function called " + vehicleGroupID + "userId -" + userId);
                if (string.IsNullOrEmpty(vehicleGroupID.ToString()))
                {
                    return StatusCode(501, "The vehicle group ID is Empty.");
                }

                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }

                int DeletedVehicleGroupID =await vehicleManagement.DeleteVehicleGroup(vehicleGroupID, userId);
                logger.LogInformation("Vehicle group deleted with Vehicle group Id - " + vehicleGroupID);

                return Ok(DeletedVehicleGroupID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("GetVehicleGroupByID")]
         public async Task<IActionResult> GetVehicleGroupByID(int vehicleGroupID,int orgid)
        {
            try
            {
                logger.LogInformation("GetVehicleGroup function called " + vehicleGroupID);

                if (string.IsNullOrEmpty(vehicleGroupID.ToString()))
                {
                    return StatusCode(501, "The vehicleGroupID is Empty.");
                }

                return Ok(await vehicleManagement.GetVehicleGroupByID(vehicleGroupID,orgid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
               // return StatusCode(500, "Internal Server Error.");
                return StatusCode(500, p);
            }
        }

        [HttpGet]
        [Route("GetVehicleGroupByOrgID")]
        public async Task<IActionResult> GetVehicleGroupByOrgID(int vehOrgID)     
        {
            try
            {
                logger.LogInformation("GetVehicleGroupByOrgID function called " + vehOrgID);

                if (string.IsNullOrEmpty(vehOrgID.ToString()))
                {
                    return StatusCode(501, "The vehicleorg id is Empty.");
                }

                return Ok(await vehicleManagement.GetVehicleGroupByOrgID(vehOrgID));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
               // return StatusCode(500, "Internal Server Error.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetVehiclesByOrgID")]
        public async Task<IActionResult> GetVehiclesByOrgID(int vehOrgID)
        {
            try
            {
                logger.LogInformation("GetVehiclesByOrgID function called " + vehOrgID);

                if (string.IsNullOrEmpty(vehOrgID.ToString()))
                {
                    return StatusCode(501, "The VehicleOrgID is Empty.");
                }

                return Ok(await vehicleManagement.GetVehiclesByOrgID(vehOrgID));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }

         [HttpGet]
        [Route("GetServiceSubscribersByOrgID")]
         public async Task<IActionResult> GetServiceSubscribersByOrgID(int orgid)
        {
            try
            {
                logger.LogInformation("GetServiceSubscribersByOrgID function called " + orgid);

                if (string.IsNullOrEmpty(orgid.ToString()))
                {
                    return StatusCode(501, "The orgid is Empty.");
                }

                return Ok(await vehicleManagement.GetServiceSubscribersByOrgID(orgid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
               // return StatusCode(500, "Internal Server Error.");
                return StatusCode(500, p);
            }
        }

        // [HttpGet]
        // [Route("GetUsersDetailsByGroupID")]
        // public async Task<IActionResult> GetUsersDetailsByGroupID(int orgid,int usergroupid)
        // {
        //     try
        //     {
        //         logger.LogInformation("GetUsersDetailsByGroupID function called " + orgid);

        //         if (string.IsNullOrEmpty(orgid.ToString()))
        //         {
        //             return StatusCode(501, "The orgid is Empty.");
        //         }
        //         if (string.IsNullOrEmpty(usergroupid.ToString()))
        //         {
        //             return StatusCode(501, "The usergroupid is Empty.");
        //         }

        //         return Ok(await vehicleManagement.GetUsersDetailsByGroupID(orgid,usergroupid));
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex.Message);
        //         var p = ex.Message;
        //        // return StatusCode(500, "Internal Server Error.");
        //         return StatusCode(500, p);
        //     }
        // }

    }
}

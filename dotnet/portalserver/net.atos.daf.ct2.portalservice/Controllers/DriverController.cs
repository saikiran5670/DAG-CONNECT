using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DriverBusinessService = net.atos.daf.ct2.driverservice;
using net.atos.daf.ct2.portalservice.Entity.Driver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using net.atos.daf.ct2.portalservice.Common;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{

    [ApiController]
    [Route("driver")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DriverController : ControllerBase
    {
       private readonly AuditHelper _auditHelper;
        private readonly ILogger<DriverController> logger;
        private readonly DriverMapper mapper;

        private readonly DriverBusinessService.DriverService.DriverServiceClient driverClient;
        private string FK_Constraint = "violates foreign key constraint";
        // private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        public DriverController(ILogger<DriverController> _logger, DriverBusinessService.DriverService.DriverServiceClient _driverClient, AuditHelper auditHelper)
        {
            logger = _logger;
            driverClient = _driverClient;
            mapper = new DriverMapper();
            _auditHelper = auditHelper;
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(int organizationId, int driverId)
        {
            try
            {
                if (organizationId < 0)
                {
                    return StatusCode(404, "Please provide the organizationId.");
                }
                else if (driverId < 0)
                {
                    return StatusCode(404, "Please provide the driverId.");
                }
                DriverBusinessService.IdRequest idRequest = new DriverBusinessService.IdRequest();
                idRequest.DriverID = driverId;
                idRequest.OrgID = organizationId;

                logger.LogInformation("Driver get function called ");
                if (organizationId < 1)
                {
                    return StatusCode(400, "Please provide organization ID:");
                }
                DriverBusinessService.DriverDataList drvResponse = await driverClient.GetAsync(idRequest);
                if (drvResponse.Code == DriverBusinessService.Responcecode.NotFound)
                {
                    return StatusCode(404, "Driver not found");
                }

                return Ok(drvResponse.Driver);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(net.atos.daf.ct2.driverservice.DriverUpdateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FirstName))
                {
                    return StatusCode(404, "Please provde driver first name.");
                }
                else if (string.IsNullOrEmpty(request.LastName))
                {
                    return StatusCode(404, "Please provde driver last name.");
                }
                else if (request.Id <= 0)
                {
                    return StatusCode(404, "Please provde correct driverId.");
                }
                else if (request.OrganizationId <= 0)
                {
                    return StatusCode(404, "Please provde correct organizationId.");
                }

                logger.LogInformation("Driver update function called ");
                DriverBusinessService.DriverUpdateResponse Response = await driverClient.UpdateAsync(request);

                
                  await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Update method in Driver controller",request.Id,request.Id, JsonConvert.SerializeObject(request),
                                              Request);   

                return Ok(Response.Driver);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "Update method in Driver controller",request.Id,request.Id, JsonConvert.SerializeObject(request),
                                              Request); 

                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int organizationId, int driverId)
        {
             DriverBusinessService.IdRequest idRequest = new DriverBusinessService.IdRequest();
            try
            {
                if (organizationId <= 0)
                {
                    return StatusCode(404, "Please provide the correct organizationId.");
                }
                else if (driverId <= 0)
                {
                    return StatusCode(404, "Please provide the correct driverId.");
                }

            //    DriverBusinessService.IdRequest idRequest = new DriverBusinessService.IdRequest();
                idRequest.DriverID = driverId;
                idRequest.OrgID = organizationId;
                logger.LogInformation("Driver update function called ");
                DriverBusinessService.DriverDeleteResponse response = await driverClient.DeleteAsync(idRequest);

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Delete method in Driver controller",idRequest.DriverID,idRequest.DriverID, JsonConvert.SerializeObject(driverId),
                                              Request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Delete method in Driver controller",idRequest.DriverID,idRequest.DriverID, JsonConvert.SerializeObject(driverId),
                                              Request);

                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("updateoptinoptout")]
        public async Task<IActionResult> UpdateOptinOptout(net.atos.daf.ct2.driverservice.OptOutOptInRequest Optrequest)
        {
            try
            {
                if (Optrequest.OrgID <= 0)
                {
                    return StatusCode(404, "Please provide the correct organizationId.");
                }
                else if (string.IsNullOrEmpty(Optrequest.Optoutoptinstatus))
                {
                    return StatusCode(404, "Please provide the Optoutoptinstatus.");
                }
                else if (!((Optrequest.Optoutoptinstatus.ToUpper() =="U") || (Optrequest.Optoutoptinstatus.ToUpper() == "I") || (Optrequest.Optoutoptinstatus.ToUpper() == "H")))
                {
                    return StatusCode(404, "Please provide correct Optoutoptinstatus.");
                }
                logger.LogInformation("Driver UpdateOptinOptout function called ");
                DriverBusinessService.OptOutOptInResponse response = await driverClient.UpdateOptinOptoutAsync(Optrequest);
                
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "UpdateOptinOptout method in Driver controller", 0, 0, JsonConvert.SerializeObject(Optrequest),
                                              Request);


                return Ok(response);
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

        [HttpPost]
        [Route("importdrivers")]
        public async Task<IActionResult> ImportDrivers(List<DriverRequest> drivers)
        {
            try
            {
                if (drivers.Count <= 0)
                {
                    return StatusCode(404, "Please provide the driver list to import.");
                }
                logger.LogInformation("Driver import function called ");
                net.atos.daf.ct2.driverservice.DriverImportRequest request = new DriverBusinessService.DriverImportRequest();
                var driverInValidList = new List<net.atos.daf.ct2.driverservice.DriverReturns>();
                request = mapper.ToDriverImport(drivers, out driverInValidList);                

                if (request.OrgID <= 0)
                {
                    return StatusCode(404, "Please provide the oganizationid import.");
                }

                if (request.Drivers.Count <= 0)
                {
                    return Ok(driverInValidList);
                }
                    DriverBusinessService.DriverImportData response = await driverClient.ImportDriversAsync(request);
                if (response!=null && response.Code== DriverBusinessService.Responcecode.Failed)
                    return StatusCode(500, response.Message);

                if (response.Driver == null)
                    response = new DriverBusinessService.DriverImportData();

                response.Driver.AddRange(driverInValidList);

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "ImportDrivers method in Driver controller",0, 0, JsonConvert.SerializeObject(drivers),
                                              Request);
                return Ok(response.Driver);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "ImportDrivers method in Driver controller",0, 0, JsonConvert.SerializeObject(drivers),
                                              Request);

                logger.LogError(ex.Message + " " + ex.StackTrace);
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}

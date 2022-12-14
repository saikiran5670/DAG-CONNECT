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
using net.atos.daf.ct2.portalservice.Entity.Driver;
using Newtonsoft.Json;
using DriverBusinessService = net.atos.daf.ct2.driverservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{

    [ApiController]
    [Route("driver")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DriverController : BaseController
    {
        private readonly AuditHelper _auditHelper;
        private readonly DriverMapper _mapper;
        private readonly ILog _logger;
        private readonly DriverBusinessService.DriverService.DriverServiceClient _driverClient;
        private readonly string _fk_Constraint = "violates foreign key constraint";

        public DriverController(DriverBusinessService.DriverService.DriverServiceClient driverClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _driverClient = driverClient;
            _mapper = new DriverMapper();
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
                idRequest.OrgID = GetContextOrgId();

                if (organizationId < 1)
                {
                    return StatusCode(400, "Please provide organization ID.");
                }
                DriverBusinessService.DriverDataList drvResponse = await _driverClient.GetAsync(idRequest);
                if (drvResponse.Code == DriverBusinessService.Responcecode.NotFound)
                {
                    return StatusCode(404, "Driver not found");
                }

                return Ok(drvResponse.Driver);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(Get)}: With Error:-", ex);
                return StatusCode(500, DriverConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(DriverBusinessService.DriverUpdateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FirstName))
                {
                    return StatusCode(404, "Please provide driver first name.");
                }
                else if (string.IsNullOrEmpty(request.LastName))
                {
                    return StatusCode(404, "Please provide driver last name.");
                }
                else if (request.Id <= 0)
                {
                    return StatusCode(404, "Please provide correct driverId.");
                }
                else if (request.OrganizationId <= 0)
                {
                    return StatusCode(404, "Please provide correct organizationId.");
                }
                request.OrganizationId = GetContextOrgId();

                DriverBusinessService.DriverUpdateResponse Response = await _driverClient.UpdateAsync(request);

                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                           "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "Update method in Driver controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                            _userDetails);

                return Ok(Response.Driver);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "Update method in Driver controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                                              _userDetails);

                _logger.Error($"{nameof(Update)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, DriverConstants.INTERNAL_SERVER_MSG);
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
                idRequest.OrgID = GetContextOrgId();
                _logger.Info("Driver update function called ");
                DriverBusinessService.DriverDeleteResponse response = await _driverClient.DeleteAsync(idRequest);

                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Delete method in Driver controller", idRequest.DriverID, idRequest.DriverID, JsonConvert.SerializeObject(driverId),
                                              _userDetails);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "Delete method in Driver controller", idRequest.DriverID, idRequest.DriverID, JsonConvert.SerializeObject(driverId),
                                              _userDetails);

                _logger.Error($"{nameof(Delete)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, DriverConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPut]
        [Route("updateoptinoptout")]
        public async Task<IActionResult> UpdateOptinOptout(DriverBusinessService.OptOutOptInRequest Optrequest)
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
                else if (!((Optrequest.Optoutoptinstatus.ToUpper() == "U") || (Optrequest.Optoutoptinstatus.ToUpper() == "I") || (Optrequest.Optoutoptinstatus.ToUpper() == "H")))
                {
                    return StatusCode(404, "Please provide correct Optoutoptinstatus.");
                }

                Optrequest.OrgID = GetContextOrgId();
                DriverBusinessService.OptOutOptInResponse response = await _driverClient.UpdateOptinOptoutAsync(Optrequest);

                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "UpdateOptinOptout method in Driver controller", 0, 0, JsonConvert.SerializeObject(Optrequest),
                                              _userDetails);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "UpdateOptinOptout method in Driver controller", 0, 0, JsonConvert.SerializeObject(Optrequest),
                                              _userDetails);

                _logger.Error($"{nameof(UpdateOptinOptout)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, DriverConstants.INTERNAL_SERVER_MSG);
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

                DriverBusinessService.DriverImportRequest request = new DriverBusinessService.DriverImportRequest();
                var driverInValidList = new List<DriverBusinessService.DriverReturns>();
                request = _mapper.ToDriverImport(drivers, out driverInValidList);

                if (request.OrgID <= 0)
                {
                    return StatusCode(404, "Please provide the oganizationid import.");
                }

                if (request.Drivers.Count <= 0)
                {
                    return Ok(driverInValidList);
                }

                request.OrgID = GetContextOrgId();
                DriverBusinessService.DriverImportData response = await _driverClient.ImportDriversAsync(request);
                if (response != null && response.Code == DriverBusinessService.Responcecode.Failed)
                    return StatusCode(500, response.Message);

                if (response.Driver == null)
                    response = new DriverBusinessService.DriverImportData();

                response.Driver.AddRange(driverInValidList);

                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                             "ImportDrivers method in Driver controller", 0, 0, JsonConvert.SerializeObject(drivers),
                                              _userDetails);
                return Ok(response.Driver);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Driver Component",
                                             "Driver service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "ImportDrivers method in Driver controller", 0, 0, JsonConvert.SerializeObject(drivers),
                                              _userDetails);

                _logger.Error($"{nameof(ImportDrivers)}: With Error:-", ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, DriverConstants.INTERNAL_SERVER_MSG);
            }
        }
    }
}

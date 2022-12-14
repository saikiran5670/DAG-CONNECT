using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.mapservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Corridor;
using net.atos.daf.ct2.portalservice.Entity.POI;
using Newtonsoft.Json;
using Alert = net.atos.daf.ct2.alertservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("corridor")]
    public class LandmarkCorridorController : BaseController
    {

        private readonly ILog _logger;
        private readonly CorridorService.CorridorServiceClient _corridorServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly CorridorMapper _corridorMapper;
        private readonly Alert.AlertService.AlertServiceClient _alertServiceClient;
        private readonly HereMapAddressProvider _hereMapAddressProvider;
        private readonly poiservice.POIService.POIServiceClient _poiServiceClient;
        private readonly MapService.MapServiceClient _mapServiceClient;
        public LandmarkCorridorController(CorridorService.CorridorServiceClient corridorServiceClient, AuditHelper auditHelper,
            Alert.AlertService.AlertServiceClient alertServiceClient, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper
            , MapService.MapServiceClient mapServiceClient, poiservice.POIService.POIServiceClient poiServiceClient) : base(httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorServiceClient = corridorServiceClient;
            _auditHelper = auditHelper;
            _corridorMapper = new CorridorMapper();
            _alertServiceClient = alertServiceClient;
            _poiServiceClient = poiServiceClient;
            _mapServiceClient = mapServiceClient;
            _hereMapAddressProvider = new HereMapAddressProvider(_mapServiceClient, _poiServiceClient);
        }

        [HttpGet]
        [Route("getcorridorlist")]
        public async Task<IActionResult> GetCorridorList([FromQuery] net.atos.daf.ct2.portalservice.Entity.POI.CorridorRequest request)
        {
            try
            {
                if (request.OrganizationId <= 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }

                _logger.Info("GetCorridorList method in POI API called.");
                net.atos.daf.ct2.corridorservice.CorridorRequest objCorridorRequest = new net.atos.daf.ct2.corridorservice.CorridorRequest();
                objCorridorRequest.OrganizationId = GetContextOrgId();
                objCorridorRequest.CorridorId = request.CorridorId;//non mandatory field
                var data = await _corridorServiceClient.GetCorridorListAsync(objCorridorRequest);

                if (data != null && data.Code == net.atos.daf.ct2.corridorservice.Responsecode.Success)
                {
                    if (objCorridorRequest.OrganizationId > 0 && objCorridorRequest.CorridorId > 0)
                    {
                        if (data.CorridorEditViewList != null && data.CorridorEditViewList.Count > 0)
                        {
                            for (int i = 0; i < data.CorridorEditViewList.Count; i++)
                            {
                                if (data.CorridorEditViewList[i].EndPoint == "" && data.CorridorEditViewList[i].EndLat > 0 && data.CorridorEditViewList[i].EndLong > 0)
                                {
                                    GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(data.CorridorEditViewList[i].EndLat, data.CorridorEditViewList[i].EndLong);
                                    data.CorridorEditViewList[i].EndPoint = getMapRequestLatest.Address;
                                }
                                if (data.CorridorEditViewList[i].StartPoint == "" && data.CorridorEditViewList[i].StartLat > 0 && data.CorridorEditViewList[i].StartLong > 0)
                                {
                                    GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(data.CorridorEditViewList[i].StartLat, data.CorridorEditViewList[i].StartLong);
                                    data.CorridorEditViewList[i].StartPoint = getMapRequestLatest.Address;
                                }
                            }
                            return Ok(data.CorridorEditViewList);
                        }
                        else
                        {
                            return StatusCode(404, "Corridor details are not found");
                        }
                    }
                    else
                    {
                        if (data.CorridorGridViewList != null && data.CorridorGridViewList.Count > 0)
                        {
                            for (int i = 0; i < data.CorridorGridViewList.Count; i++)
                            {
                                if (data.CorridorGridViewList[i].EndPoint == "" && data.CorridorGridViewList[i].EndLat > 0 && data.CorridorGridViewList[i].EndLong > 0)
                                {
                                    GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(data.CorridorGridViewList[i].EndLat, data.CorridorGridViewList[i].EndLong);
                                    data.CorridorGridViewList[i].EndPoint = getMapRequestLatest.Address;
                                }
                                if (data.CorridorGridViewList[i].StartPoint == "" && data.CorridorGridViewList[i].StartLat > 0 && data.CorridorGridViewList[i].StartLong > 0)
                                {
                                    GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(data.CorridorGridViewList[i].StartLat, data.CorridorGridViewList[i].StartLong);
                                    data.CorridorGridViewList[i].StartPoint = getMapRequestLatest.Address;
                                }
                            }
                            return Ok(data.CorridorGridViewList);
                        }
                        else
                        {
                            return StatusCode(404, "Corridor details are not found");
                        }
                    }
                }
                else
                {
                    return StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetCorridorList)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }


        [HttpPost]
        [Route("addroutecorridor")]

        public async Task<IActionResult> AddRouteCorridor(Entity.Corridor.CorridorRequest request)
        {
            try
            {

                if (request.OrganizationId == 0)
                {
                    return StatusCode(400, "Organization_Id Required .");
                }
                //As more Via routes will be included
                //if (request.ViaAddressDetails.Count > 5)
                //{
                //    return StatusCode(400, "You cannot enter more than 5 via Routes.");
                //}
                request.OrganizationId = GetContextOrgId();
                var mapRequest = _corridorMapper.MapCorridor(request);
                var data = await _corridorServiceClient.AddRouteCorridorAsync(mapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "AddRouteCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            _userDetails);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddRouteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);

                _logger.Error($"{nameof(AddRouteCorridor)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }


        [HttpPost]
        [Route("addexistingtripcorridor")]

        public async Task<IActionResult> AddExistingTripCorridor(ExistingTripCorridor request)
        {
            try
            {

                if (request.OrganizationId == 0)
                {
                    //bool hasRights = HasAdminPrivilege();
                    //if (!hasRights)
                    return StatusCode(400, "Organization_Id Required .");
                }
                if (request.ExistingTrips.Count == 0)
                {
                    return StatusCode(400, "ExistingTrips required");
                }
                request.OrganizationId = GetContextOrgId();
                var MapRequest = _corridorMapper.MapExistingTripCorridorRequest(request);
                var data = await _corridorServiceClient.AddExistingTripCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "AddExistingTripCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            _userDetails);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddExistingTripCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);

                _logger.Error($"{nameof(AddExistingTripCorridor)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("updateexistingtripcorridor")]
        public async Task<IActionResult> UpdateExistingTripCorridor(ExistingTripCorridor request)
        {
            try
            {
                if (request.OrganizationId == 0 && request.Id == 0)
                {
                    //bool hasRights = HasAdminPrivilege();
                    //if (!hasRights)
                    return StatusCode(400, "Organization_Id and Id are Required .");
                }
                if (request.ExistingTrips.Count == 0)
                {
                    return StatusCode(400, "ExistingTrips required");
                }
                request.OrganizationId = GetContextOrgId();
                var MapRequest = _corridorMapper.MapExistingTripCorridorRequest(request);
                var data = await _corridorServiceClient.UpdateExistingTripCorridorAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "UpdateExistingTripCorridor method in Landmark Corridor controller", data.CorridorID, data.CorridorID, JsonConvert.SerializeObject(request),
                                            _userDetails);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "UpdateExistingTripCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);

                _logger.Error($"{nameof(UpdateExistingTripCorridor)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpDelete]
        [Route("deletecorridor")]

        public async Task<IActionResult> DeleteCorridor([FromQuery] Entity.Corridor.DeleteCorridorIdRequest request)
        {
            try
            {
                bool hasRights = HasAdminPrivilege();

                if (request.Id <= 0)
                {
                    return StatusCode(400, "Corridor id is required.");
                }

                Alert.LandmarkIdRequest landmarkIdRequest = new Alert.LandmarkIdRequest();
                landmarkIdRequest.LandmarkId.Add(request.Id);

                Alert.LandmarkIdExistResponse isLandmarkavalible = await _alertServiceClient.IsLandmarkActiveInAlertAsync(landmarkIdRequest);

                if (isLandmarkavalible.IsLandmarkActive)
                {
                    return StatusCode(409, "Corridor is used in alert.");
                }
                var MapRequest = _corridorMapper.MapId(request);
                var data = await _corridorServiceClient.DeleteCorridorAsync(MapRequest);


                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                         "DeleteCorridor method in Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.NotFound)
                {
                    return StatusCode(404, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "DeleteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);
                _logger.Error($"{nameof(DeleteCorridor)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateRouteCorridor(Entity.Corridor.CorridorRequest request)
        {
            try
            {
                if (request.OrganizationId == 0)
                {
                    return StatusCode(400, "Organization Id is required.");
                }
                //As more Via routes will be included
                //if (request.ViaAddressDetails.Count > 5)
                //{
                //    return StatusCode(400, "You cannot enter more than 5 via Routes.");
                //}
                request.OrganizationId = GetContextOrgId();
                UpdateRouteCorridorRequest objUpdateRouteCorridorRequest = new UpdateRouteCorridorRequest();
                objUpdateRouteCorridorRequest.Request = _corridorMapper.MapCorridor(request);
                var data = await _corridorServiceClient.UpdateRouteCorridorAsync(objUpdateRouteCorridorRequest);
                if (data != null && data.Response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                           "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "UpdateRouteCorridor method in Landmark Corridor controller", data.Response.CorridorID, data.Response.CorridorID, JsonConvert.SerializeObject(request),
                                            _userDetails);
                    return Ok(data.Response);
                }
                else if (data != null && data.Response.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Response.Message);
                }
                else
                {
                    return StatusCode(500, data.Response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Corridor Component",
                                         "Corridor service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "UpdateRouteCorridor method in Landmark Corridor controller", 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);
                _logger.Error($"{nameof(UpdateRouteCorridor)}: With Error:-", ex);
                return StatusCode(500, LandmarkConstants.INTERNAL_SERVER_MSG);
            }
        }
    }

}

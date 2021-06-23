using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.mapservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Report;
using net.atos.daf.ct2.reportservice;
using Newtonsoft.Json;
using static net.atos.daf.ct2.reportservice.ReportService;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("report")]
    public class ReportController : BaseController
    {
        private readonly ILog _logger;
        private readonly ReportServiceClient _reportServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly Mapper _mapper;
        private readonly HereMapAddressProvider _hereMapAddressProvider;
        private readonly poiservice.POIService.POIServiceClient _poiServiceClient;
        private readonly MapService.MapServiceClient _mapServiceClient;

        public ReportController(ReportServiceClient reportServiceClient, AuditHelper auditHelper,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper,
                               MapService.MapServiceClient mapServiceClient, poiservice.POIService.POIServiceClient poiServiceClient
                               ) : base(httpContextAccessor, sessionHelper)
        {
            _reportServiceClient = reportServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _poiServiceClient = poiServiceClient;
            _mapServiceClient = mapServiceClient;
            _hereMapAddressProvider = new HereMapAddressProvider(_mapServiceClient, _poiServiceClient);
        }

        #region Select User Preferences

        [HttpGet]
        [Route("getreportdetails")]
        [Route("getdetails")]
        public async Task<IActionResult> GetReportDetails()
        {
            try
            {
                var response = await _reportServiceClient.GetReportDetailsAsync(new TempPara { TempID = 0 });
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.GET_REPORT_DETAILS_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getuserpreferencereportdatacolumn")]
        [Route("userpreference/get")]
        public async Task<IActionResult> GetUserPreferenceReportDataColumn(int reportId, int accountId, int organizationId)
        {
            try
            {
                if (!(reportId > 0)) return BadRequest(ReportConstants.REPORT_REQUIRED_MSG);
                if (!(accountId > 0)) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);
                var response = await _reportServiceClient.GetUserPreferenceReportDataColumnAsync(new IdRequest { ReportId = reportId, AccountId = accountId, OrganizationId = organizationId });
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, ReportConstants.USER_PREFERENCE_FAILURE_MSG2));
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region - Trip Report Table Details
        [HttpGet]
        [Route("gettripdetails")]
        [Route("trip/getdetails")]
        public async Task<IActionResult> GetFilteredTripDetails([FromQuery] TripFilterRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_STARTDATE_MSG);
                if (!(request.EndDateTime > 0)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_ENDDATE_MSG);
                if (string.IsNullOrEmpty(request.VIN)) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_VINREQUIRED_MSG);
                if (request.StartDateTime > request.EndDateTime) return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG);

                _logger.Info("GetFilteredTripDetailsAsync method in Report (Trip Report) API called.");
                var data = await _reportServiceClient.GetFilteredTripDetailsAsync(request);

                data.TripData.Select(x =>
                {
                    x = _hereMapAddressProvider.UpdateTripReportAddress(x);
                    return x;
                }).ToList();


                if (data?.TripData?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region Create User Preference
        [HttpPost]
        [Route("createuserpreference")]
        [Route("userpreference/create")]
        public async Task<IActionResult> CreateUserPreference(net.atos.daf.ct2.portalservice.Entity.Report.UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            try
            {
                var request = _mapper.MapCreateUserPrefences(objUserPreferenceCreateRequest);
                var response = await _reportServiceClient.CreateUserPreferenceAsync(request);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.");

                switch (response.Code)
                {
                    case Responsecode.Success:
                        await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS, "Report preference created successfully", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                 _userDetails);
                        return Ok(response);
                    case Responsecode.Failed:
                        return StatusCode((int)response.Code, response.Message);
                    case Responsecode.InternalServerError:
                        return StatusCode((int)response.Code, response.Message);
                    default:
                        return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 $"createuserpreference method Failed. Error:{ex.Message}", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, $"{ex.Message} {ex.StackTrace}");
            }
        }
        #endregion

        #region Select User Preferences
        [HttpGet]
        [Route("getvinsfromtripstatisticsandvehicledetails")]
        [Route("trip/getparameters")]
        public async Task<IActionResult> GetVinsFromTripStatisticsAndVehicleDetails(int accountId, int organizationId)
        {
            try
            {
                if (!(accountId > 0)) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);
                var response = await _reportServiceClient
                                            .GetVinsFromTripStatisticsWithVehicleDetailsAsync
                                            (
                                              new VehicleListRequest { AccountId = accountId, OrganizationId = organizationId }
                                            );

                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response);
                if (response.Code == Responsecode.Failed)
                    return StatusCode((int)response.Code, response);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG2, accountId, organizationId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                //await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                // "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                // $"GetVinsFromTripStatisticsAndVehicleDetails method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(accountId),
                //  Request);
                // check for fk violation
                _logger.Error(null, ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region - Driver Time Management Report Table Details
        [HttpPost]
        [Route("drivetime/getdetails")]
        public async Task<IActionResult> GetDriversActivity([FromBody] Entity.Report.DriversTimeFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (request.DriverIds.Count <= 0) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                ActivityFilterRequest objMultipleDrivers = JsonConvert.DeserializeObject<ActivityFilterRequest>(filters);
                _logger.Info("GetDriversActivityAsync method in Report (Multiple Driver Time details Report) API called.");
                var data = await _reportServiceClient.GetDriversActivityAsync(objMultipleDrivers);
                if (data?.DriverActivities?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("drivetime/getdetailssingle")]
        public async Task<IActionResult> GetDriverActivity([FromBody] Entity.Report.SingleDriverTimeFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (string.IsNullOrEmpty(request.VIN)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (string.IsNullOrEmpty(request.DriverId)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                SingleDriverActivityFilterRequest objSingleDriver = JsonConvert.DeserializeObject<SingleDriverActivityFilterRequest>(filters);
                _logger.Info("GetDriverActivityAsync method in Report (Single Driver Time details Report) API called.");
                var data = await _reportServiceClient.GetDriverActivityAsync(objSingleDriver);
                if (data?.DriverActivities?.Count > 0)
                {
                    data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("drivetime/getparameters")]
        public async Task<IActionResult> GetDriverActivityParameters([FromBody] IdRequestForDriverActivity request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (!(request.OrganizationId > 0)) { return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG); }
                if (!(request.AccountId > 0)) { return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG); }

                _logger.Info("GetDriverActivityParameters method in Report API called.");
                var data = await _reportServiceClient.GetDriverActivityParametersAsync(request);

                if (data.Code.ToString() == "NotFound")
                {
                    return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                }
                else
                {
                    data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("search/getparameters")]
        public async Task<IActionResult> GetReportSearchParameter([FromBody] IdRequestForDriverActivity request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (!(request.OrganizationId > 0)) { return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG); }
                if (!(request.AccountId > 0)) { return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG); }

                _logger.Info("GetReportSearchParameter method in Report API called.");
                var data = await _reportServiceClient.GetReportSearchParameterAsync(request);
                if (data?.VehicleDetailsWithAccountVisibiltyList?.Count > 0)
                {
                    data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region Eco Score Report

        #region Eco Score Report - Create Profile

        [HttpPost]
        [Route("ecoscore/createprofile")]
        public async Task<IActionResult> Create([FromBody] EcoScoreProfileCreateRequest request)
        {
            try
            {
                var grpcRequest = _mapper.MapCreateEcoScoreProfile(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();
                var response = await _reportServiceClient.CreateEcoScoreProfileAsync(grpcRequest);
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED, "Eco Score profile created successfully", 0, 0, JsonConvert.SerializeObject(request),
                                 _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Eco score Report - Update Profile

        [HttpPut]
        [Route("ecoscore/updateprofile")]
        public async Task<IActionResult> Update([FromBody] EcoScoreProfileUpdateRequest request)
        {
            try
            {
                bool hasRights = await HasAdminPrivilege();
                var grpcRequest = _mapper.MapUpdateEcoScoreProfile(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();
                Metadata headers = new Metadata();
                headers.Add("hasRights", Convert.ToString(hasRights));
                var response = await _reportServiceClient.UpdateEcoScoreProfileAsync(grpcRequest, headers);
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED, "Eco Score profile updated successfully", 0, 0, JsonConvert.SerializeObject(request),
                                 _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region Eco Score Report - Get Profile & KPI details

        [HttpGet]
        [Route("ecoscore/getprofiles")]
        public async Task<IActionResult> GetEcoScoreProfiles(bool isGlobal)
        {
            try
            {
                var organizationId = !isGlobal ? GetContextOrgId() : 0;
                var response = await _reportServiceClient.GetEcoScoreProfilesAsync(new GetEcoScoreProfileRequest { OrgId = organizationId });
                if (response?.Profiles?.Count > 0)
                {
                    response.Message = ReportConstants.GET_ECOSCORE_PROFILE_SUCCESS_MSG;
                    return Ok(response);
                }
                else
                    return StatusCode((int)response.Code, response.Message);
            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED, ReportConstants.GET_ECOSCORE_PROFILE_SUCCESS_MSG, 0, 0, Convert.ToString(isGlobal),
                                 _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("ecoscore/getprofilekpis")]
        public async Task<IActionResult> GetEcoScoreProfileKPIs(int profileId)
        {
            try
            {
                var response = await _reportServiceClient.GetEcoScoreProfileKPIDetailsAsync(new GetEcoScoreProfileKPIRequest { ProfileId = profileId });
                if (response?.Profile?.Count > 0)
                {
                    response.Message = ReportConstants.GET_ECOSCORE_PROFILE_KPI_SUCCESS_MSG;
                    return Ok(response);
                }
                else
                    return StatusCode((int)response.Code, response.Message);
            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED, ReportConstants.GET_ECOSCORE_PROFILE_KPI_SUCCESS_MSG, 0, 0, Convert.ToString(profileId),
                                 _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Eco Score Report - Delete Profile

        [HttpDelete]
        [Route("ecoscore/deleteprofile")]
        public async Task<IActionResult> DeleteEcoScoreProfile([FromQuery] EcoScoreProfileDeleteRequest request)
        {
            try
            {
                bool hasRights = await HasAdminPrivilege();
                var grpcRequest = new reportservice.DeleteEcoScoreProfileRequest();
                grpcRequest.ProfileId = request.ProfileId;
                Metadata headers = new Metadata();
                headers.Add("hasRights", Convert.ToString(hasRights));
                var response = await _reportServiceClient.DeleteEcoScoreProfileAsync(grpcRequest, headers);
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED, ReportConstants.DELETE_ECOSCORE_PROFILE_KPI_SUCCESS_MSG, 0, 0, Convert.ToString(request.ProfileId),
                                 _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region  Eco Score Report By All Drivers
        [HttpPost]
        [Route("ecoscore/getdetailsbyalldriver")]
        public async Task<IActionResult> GetEcoScoreReportByAllDrivers([FromBody] EcoScoreReportByAllDriversRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_DATEMISMATCH_MSG); }

                var grpcRequest = _mapper.MapEcoScoreReportByAllDriver(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();

                var response = await _reportServiceClient.GetEcoScoreReportByAllDriversAsync(grpcRequest);
                if (response?.DriverRanking?.Count > 0)
                {
                    response.Message = ReportConstants.GET_ECOSCORE_REPORT_SUCCESS_MSG;
                    return Ok(response);
                }
                else
                {
                    return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #endregion

        #region Fleet utilization report details
        [HttpPost]
        [Route("fleetutilization/getdetails")]
        public async Task<IActionResult> GetFleetUtilizationDetails([FromBody] Entity.Report.FleetUtilizationFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetUtilizationFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetUtilizationFilterRequest>(filters);
                _logger.Info("GetFleetUtilizationDetails method in Report (for Fleet Utilization details by vehicle) API called.");
                var data = await _reportServiceClient.GetFleetUtilizationDetailsAsync(objFleetFilter);
                if (data?.FleetDetails?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("fleetutilization/getcalenderdata")]
        public async Task<IActionResult> GetCalenderData([FromBody] Entity.Report.FleetUtilizationFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_UTILIZATION_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetUtilizationFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetUtilizationFilterRequest>(filters);
                _logger.Info("GetFleetUtilizationDetails method in Report (for Fleet Utilization details by vehicle) API called.");
                var data = await _reportServiceClient.GetFleetCalenderDetailsAsync(objFleetFilter);
                if (data?.CalenderDetails?.Count > 0)
                {
                    data.Message = ReportConstants.GET_TRIP_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_TRIP_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion
    }
}

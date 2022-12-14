using System;
using System.Collections.Generic;
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
using POI = net.atos.daf.ct2.portalservice.Entity.POI;
using net.atos.daf.ct2.portalservice.Entity.Report;
using net.atos.daf.ct2.reportservice;
using Newtonsoft.Json;
using static net.atos.daf.ct2.reportservice.ReportService;
using net.atos.daf.ct2.vehicleservice;
using Alert = net.atos.daf.ct2.portalservice.Entity.Alert;

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
        private readonly VehicleService.VehicleServiceClient _vehicleClient;

        public ReportController(ReportServiceClient reportServiceClient, AuditHelper auditHelper,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper,
                               MapService.MapServiceClient mapServiceClient, poiservice.POIService.POIServiceClient poiServiceClient,
                               VehicleService.VehicleServiceClient vehicleClient
                               ) : base(httpContextAccessor, sessionHelper)
        {
            _reportServiceClient = reportServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _poiServiceClient = poiServiceClient;
            _mapServiceClient = mapServiceClient;
            _hereMapAddressProvider = new HereMapAddressProvider(_mapServiceClient, _poiServiceClient);
            _vehicleClient = vehicleClient;
        }


        #region User Perferences

        #region Select User Preferences

        [HttpGet]
        //[Route("getreportdetails")]
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
                _logger.Error($"{nameof(GetReportDetails)}: With Error:-", ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }

                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        //[Route("getuserpreferencereportdatacolumn")]
        [Route("userpreference/get")]
        public async Task<IActionResult> GetUserPreferenceReportDataColumn(int reportId, int accountId, int organizationId)
        {
            try
            {
                organizationId = GetContextOrgId();
                if (reportId < 1) return BadRequest(ReportConstants.REPORT_REQUIRED_MSG);
                if (accountId < 1) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (organizationId < 1) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);
                var response = await _reportServiceClient
                                        .GetUserPreferenceReportDataColumnAsync(
                                            new IdRequest
                                            {
                                                ReportId = reportId,
                                                AccountId = _userDetails.AccountId,
                                                RoleId = _userDetails.RoleId,
                                                OrganizationId = GetUserSelectedOrgId(),
                                                ContextOrgId = GetContextOrgId()
                                            });
                if (response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                     "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     $"GetUserPreferenceReportDataColumn method", 1, 2, Convert.ToString(reportId),
                      _userDetails);
                    return Ok(response);
                }
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, string.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, accountId, reportId, response.Message));
                else
                    return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                     "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                     $"GetUserPreferenceReportDataColumn method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(reportId),
                      _userDetails);
                _logger.Error($"{nameof(GetUserPreferenceReportDataColumn)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Create User Preference
        [HttpPost]
        //[Route("createuserpreference")]
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
                _logger.Error($"{nameof(CreateUserPreference)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #endregion

        #region Trip Report
        #region Select Trip User Preferences
        [HttpGet]
        [Route("trip/getparameters")]
        [Route("fleetfuel/getparameters")]
        [Route("fleetutilization/getparameters")]
        [Route("fuelbenchmarking/getparameters")]
        [Route("fueldeviation/getparameters")]
        [Route("vehicleperformance/getparameters")]
        public async Task<IActionResult> GetVinsFromTripStatisticsAndVehicleDetails(int accountId, int organizationId)
        {
            try
            {
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());
                organizationId = GetContextOrgId();
                if (!(accountId > 0)) return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG);
                if (!(organizationId > 0)) return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG);

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));

                var response = await _reportServiceClient
                                            .GetVinsFromTripStatisticsWithVehicleDetailsAsync
                                            (
                                              new VehicleListRequest { AccountId = accountId, OrganizationId = organizationId },
                                              headers
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
                _logger.Error($"{nameof(GetVinsFromTripStatisticsAndVehicleDetails)}: With Error:-", ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getvisibility")]
        public async Task<IActionResult> GetVisibility([FromQuery] int featureId, string type)
        {
            try
            {
                var organizationId = GetContextOrgId();
                var featureIds = GetMappedFeatureIdByStartWithName("Alerts.");
                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("type", type);
                var response = await _reportServiceClient
                                            .GetVisibilityAsync
                                            (
                                              new VehicleListRequest { AccountId = _userDetails.AccountId, OrganizationId = organizationId },
                                              headers
                                            );
                return Ok(response);
            }
            catch (Exception ex)
            {
                // check for fk violation
                _logger.Error($"{nameof(GetVisibility)}: With Error:-", ex);
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        #endregion

        #region - Trip Report Table Details
        [HttpGet]
        //[Route("gettripdetails")]
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
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(Alert.AlertConstants.ALERT_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                request.AccountId = _userDetails.AccountId;
                request.OrganizationId = GetContextOrgId();
                var data = await _reportServiceClient.GetFilteredTripDetailsAsync(request, headers);

                foreach (var trip in data.TripData)
                {
                    _hereMapAddressProvider.UpdateTripReportAddress(trip);
                }


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
                _logger.Error($"{nameof(GetFilteredTripDetails)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion
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
                _logger.Error($"{nameof(GetDriverActivity)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                _logger.Error($"{nameof(GetDriverActivity)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("drivetime/getdetails/chart")]
        public async Task<IActionResult> GetDriverActivityChartDetails([FromBody] Entity.Report.DriverTimeChartFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (string.IsNullOrEmpty(request.DriverId)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_DRIVERIDREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_TRIP_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                DriverActivityChartFilterRequest objDriverChartData = JsonConvert.DeserializeObject<DriverActivityChartFilterRequest>(filters);
                _logger.Info("GetDriverActivityAsync method in Report (Single Driver Time details Report) API called.");
                var data = await _reportServiceClient.GetDriverActivityChartDetailsAsync(objDriverChartData);
                if (data?.DriverActivitiesChartData?.Count > 0)
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
                _logger.Error($"{nameof(GetDriverActivityChartDetails)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("drivetime/getparameters")]
        [Route("ecoscore/getparameters")]
        public async Task<IActionResult> GetDriverActivityParameters([FromBody] IdRequestForDriverActivity request)
        {
            try
            {
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                request.OrganizationId = GetContextOrgId();
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (!(request.OrganizationId > 0)) { return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG); }
                if (!(request.AccountId > 0)) { return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG); }

                _logger.Info("GetDriverActivityParameters method in Report API called.");

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));

                if (HttpContext.Request.Path.Value.ToLower() == "/report/ecoscore/getparameters")
                {
                    var data = await _reportServiceClient.GetDriverEcoScoreParametersAsync(request, headers);

                    if (data.Code.ToString() == Responcecode.NotFound.ToString())
                    {
                        return StatusCode(404, ReportConstants.GET_DRIVER_TIME_FAILURE_MSG);
                    }
                    else
                    {
                        data.Message = ReportConstants.GET_DRIVER_TIME_SUCCESS_MSG;
                        return Ok(data);
                    }
                }
                else
                {
                    var data = await _reportServiceClient.GetDriverActivityParametersAsync(request, headers);

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

            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetDriverActivityParameters)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("search/getparameters")]
        public async Task<IActionResult> GetReportSearchParameter([FromBody] IdRequestForDriverActivity request)
        {
            try
            {
                request.OrganizationId = GetContextOrgId();
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG); }
                if (!(request.OrganizationId > 0)) { return BadRequest(ReportConstants.ORGANIZATION_REQUIRED_MSG); }
                if (!(request.AccountId > 0)) { return BadRequest(ReportConstants.ACCOUNT_REQUIRED_MSG); }

                _logger.Info("GetReportSearchParameter method in Report API called.");

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(0));

                var data = await _reportServiceClient.GetReportSearchParameterAsync(request, headers);
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
                _logger.Error($"{nameof(GetReportSearchParameter)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Eco Score Report

        #region Eco Score Report - Get Profile & KPI details

        [HttpGet]
        [Route("ecoscore/getprofiles")]
        public async Task<IActionResult> GetEcoScoreProfiles(bool isGlobal)
        {
            try
            {
                var organizationId = !isGlobal ? GetContextOrgId() : 0;

                char used_type = 'N';
                if (_userDetails.UserFeatures.Any(x => x.Name.Contains("Report.ECOScoreReport")))
                    used_type = 'A';
                if (_userDetails.UserFeatures.Any(x => x.Name.Contains("Report.ECOScoreReport.Advance")))
                    used_type = 'D';

                Metadata headers = new Metadata();
                headers.Add("used_type", Convert.ToString(used_type));

                var response = await _reportServiceClient.GetEcoScoreProfilesAsync(new GetEcoScoreProfileRequest { OrgId = organizationId }, headers);
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
                _logger.Error($"{nameof(GetEcoScoreProfiles)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                _logger.Error($"{nameof(GetEcoScoreProfileKPIs)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        #endregion

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
                _logger.Error($"{nameof(Create)}: CreateEcoScoreProfile With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                bool hasRights = HasAdminPrivilege();
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
                _logger.Error($"{nameof(Update)}: UpdateEcoScoreProfile With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                bool hasRights = HasAdminPrivilege();
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
                _logger.Error($"{nameof(DeleteEcoScoreProfile)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                _logger.Error($"{nameof(GetEcoScoreReportByAllDrivers)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region  Eco Score Report - Compare Drivers
        [HttpPost]
        [Route("ecoscore/comparedrivers")]
        public async Task<IActionResult> GetEcoScoreReportCompareDrivers([FromBody] EcoScoreReportCompareDriversRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_DATEMISMATCH_MSG); }
                if (request.DriverIds.Count < 2 || request.DriverIds.Count > 4) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_COMPAREDRIVER_MSG); }

                var grpcRequest = _mapper.MapEcoScoreReportCompareDriver(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();

                var response = await _reportServiceClient.GetEcoScoreReportCompareDriversAsync(grpcRequest);
                if (response?.Drivers?.Count > 0)
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
                _logger.Error($"{nameof(GetEcoScoreReportCompareDrivers)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region  Eco Score Report - Single Driver
        [HttpPost]
        [Route("ecoscore/singledriver")]
        public async Task<IActionResult> GetEcoScoreReportSingleDriver([FromBody] EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_DATEMISMATCH_MSG); }

                var grpcRequest = _mapper.MapEcoScoreReportSingleDriver(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();

                var response = await _reportServiceClient.GetEcoScoreReportSingleDriverAsync(grpcRequest);
                if (response?.SingleDriver?.Count > 0)
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
                _logger.Error($"{nameof(GetEcoScoreReportSingleDriver)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("ecoscore/trendlines")]
        public async Task<IActionResult> GetEcoScoreReportTrendlines([FromBody] EcoScoreReportSingleDriverRequest request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_ECOSCORE_REPORT_VALIDATION_DATEMISMATCH_MSG); }

                var grpcRequest = _mapper.MapEcoScoreReportSingleDriver(request);
                grpcRequest.AccountId = _userDetails.AccountId;
                grpcRequest.OrgId = GetContextOrgId();

                var response = await _reportServiceClient.GetEcoScoreReportTrendlinesAsync(grpcRequest);
                if (response?.Trendlines?.Count > 0)
                {
                    response.Message = ReportConstants.GET_ECOSCORE_REPORT_TRENDLINE_SUCCESS_MSG;
                    return Ok(response);
                }
                else
                {
                    return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetEcoScoreReportTrendlines)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Eco Score Report - User Preferences

        /// <summary>
        /// Initially created for Eco Score report. Later can be generalized.
        /// </summary>
        /// <param name="objUserPreferenceCreateRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reportuserpreference/create")]
        public async Task<IActionResult> CreateReportUserPreference(Entity.Report.ReportUserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            try
            {
                var request = _mapper.MapCreateReportUserPreferences(objUserPreferenceCreateRequest, _userDetails.AccountId, GetContextOrgId());
                var response = await _reportServiceClient.CreateReportUserPreferenceAsync(request);

                if (response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                            "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS, "Report use preference created successfully", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                _userDetails);
                    return Ok(response.Message);
                }
                else
                {
                    return StatusCode((int)response.Code, response.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                 $"{ nameof(CreateReportUserPreference) } method Failed. Error : {ex.Message}", 0, 0, JsonConvert.SerializeObject(objUserPreferenceCreateRequest),
                                  _userDetails);
                _logger.Error($"{nameof(CreateReportUserPreference)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        /// <summary>
        /// Initially created for Eco Score report. Later can be generalized.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("reportuserpreference/get")]
        public async Task<IActionResult> GetReportUserPreference(int reportId)
        {
            try
            {
                if (reportId < 1) return BadRequest(ReportConstants.REPORT_REQUIRED_MSG);

                GetReportUserPreferenceRequest userPrefRequest = new GetReportUserPreferenceRequest();
                userPrefRequest.ReportId = reportId;
                userPrefRequest.AccountId = _userDetails.AccountId;
                userPrefRequest.RoleId = _userDetails.RoleId;
                userPrefRequest.OrganizationId = GetUserSelectedOrgId();
                userPrefRequest.ContextOrgId = GetContextOrgId();

                var subReportResponse = await _reportServiceClient.CheckIfSubReportExistAsync(new CheckIfSubReportExistRequest { ReportId = reportId });

                // Send sub report features from session to find attributes of related reports
                SessionFeatures[] objUserFeatures;
                if (subReportResponse != null)
                {
                    var sessionFeatures = GetUserSubscribeFeatures();
                    if (subReportResponse.HasSubReports == "Y" && subReportResponse.FeatureId > 0)
                    {
                        var featureName = sessionFeatures?.Where(x => x.FeatureId == subReportResponse.FeatureId)?.Select(x => x.Name)?.FirstOrDefault();

                        if (!string.IsNullOrEmpty(featureName))
                        {
                            var logbookFeatureToExclude = sessionFeatures.Where(x => x.Name.Equals("FleetOverview.LogBook"));
                            var requiredFeatures = sessionFeatures.Where(x => x.Name.StartsWith(featureName)).Except(logbookFeatureToExclude);

                            if (requiredFeatures.Count() > 0)
                            {
                                string strFeature = JsonConvert.SerializeObject(requiredFeatures);
                                objUserFeatures = JsonConvert.DeserializeObject<SessionFeatures[]>(strFeature);
                                if (objUserFeatures != null) { userPrefRequest.UserFeatures.AddRange(objUserFeatures); }
                            }
                        }
                    }
                    else if (subReportResponse.HasSubReports == "N" && subReportResponse.FeatureId > 0)
                    {
                        if (!(sessionFeatures?.Any(x => x.FeatureId == subReportResponse.FeatureId) ?? false))
                            return StatusCode(404, "No data found.");
                    }
                }

                var response = await _reportServiceClient.GetReportUserPreferenceAsync(userPrefRequest);
                if (response.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                     "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     $"{ nameof(GetReportUserPreference) } method", 1, 2, Convert.ToString(reportId),
                      _userDetails);
                    return Ok(new { TargetProfileId = response.TargetProfileId, UserPreferences = response.UserPreference });
                }
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, string.Format(ReportConstants.USER_PREFERENCE_FAILURE_MSG, response.Message));
                else
                    return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                 "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetReportUserPreference) } method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetReportUserPreference)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                    data.Message = ReportConstants.GET_FLEET_UTILIZATION_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_UTILIZATION_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetUtilizationDetails)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
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
                    data.Message = ReportConstants.GET_FLEET_UTILIZATION_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_UTILIZATION_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetCalenderData)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region FleetOverview
        [HttpGet]
        [Route("fleetoverview/getfilterdetails")]
        public async Task<IActionResult> GetFleetOverviewFilter()
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var alertFeatureIds = GetMappedFeatureIdByStartWithName(ReportConstants.FLEETOVERVIEW_ALERT_FEATURE_STARTWITH);
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                ReportFleetOverviewFilter reportFleetOverviewFilter = new ReportFleetOverviewFilter();
                var fleetOverviewFilterRequest = new FleetOverviewFilterIdRequest();
                fleetOverviewFilterRequest.AccountId = _userDetails.AccountId;
                fleetOverviewFilterRequest.OrganizationId = GetContextOrgId();
                fleetOverviewFilterRequest.RoleId = _userDetails.RoleId;

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));
                headers.Add("alert_feature_ids", JsonConvert.SerializeObject(alertFeatureIds));

                FleetOverviewFilterResponse response = await _reportServiceClient.GetFleetOverviewFilterAsync(fleetOverviewFilterRequest, headers);

                reportFleetOverviewFilter = _mapper.ToFleetOverviewEntity(response);
                poiservice.POIRequest poiRequest = new poiservice.POIRequest();
                poiRequest.OrganizationId = GetContextOrgId(); //36;
                poiRequest.Type = "POI";
                var data = await _poiServiceClient.GetAllPOIAsync(poiRequest);
                reportFleetOverviewFilter.UserPois = new List<POI.POIResponse>();
                reportFleetOverviewFilter.GlobalPois = new List<POI.POIResponse>();
                foreach (var item in data.POIList)
                {
                    if (item.OrganizationId > 0)
                        reportFleetOverviewFilter.UserPois.Add(_mapper.ToPOIEntity(item));
                    else
                        reportFleetOverviewFilter.GlobalPois.Add(_mapper.ToPOIEntity(item));
                }
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(reportFleetOverviewFilter);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.FLEETOVERVIEW_FILTER_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                 ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetFleetOverviewFilter) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetFleetOverviewFilter)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fleetoverview/getfleetoverviewdetails")]
        public async Task<IActionResult> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter)
        {
            try
            {  // Fetch Feature Ids of the alert for visibility
                var alertFeatureIds = GetMappedFeatureIdByStartWithName(ReportConstants.FLEETOVERVIEW_ALERT_FEATURE_STARTWITH);
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                FleetOverviewDetailsRequest fleetOverviewDetailsRequest = new FleetOverviewDetailsRequest
                {
                    AccountId = _userDetails.AccountId,
                    OrganizationId = GetContextOrgId(),
                    RoleId = _userDetails.RoleId
                };
                fleetOverviewDetailsRequest.GroupIds.AddRange(fleetOverviewFilter.GroupId);
                fleetOverviewDetailsRequest.AlertCategories.AddRange(fleetOverviewFilter.AlertCategory);
                fleetOverviewDetailsRequest.AlertLevels.AddRange(fleetOverviewFilter.AlertLevel);
                fleetOverviewDetailsRequest.HealthStatus.AddRange(fleetOverviewFilter.HealthStatus);
                fleetOverviewDetailsRequest.OtherFilters.AddRange(fleetOverviewFilter.OtherFilter);
                fleetOverviewDetailsRequest.DriverIds.AddRange(fleetOverviewFilter.DriverId);
                fleetOverviewDetailsRequest.Days = fleetOverviewFilter.Days;
                /* Need to comment Start */
                //fleetOverviewDetailsRequest.AccountId = 171;
                // fleetOverviewDetailsRequest.OrganizationId = 36;
                // fleetOverviewDetailsRequest.RoleId = 61;
                /* Need to comment End */

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));
                headers.Add("alert_feature_ids", JsonConvert.SerializeObject(alertFeatureIds));

                FleetOverviewDetailsResponse response = await _reportServiceClient.GetFleetOverviewDetailsAsync(fleetOverviewDetailsRequest, headers);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                {
                    foreach (var fleetoverviewItem in response.FleetOverviewDetailList)
                    {
                        if (fleetoverviewItem.LatestGeolocationAddressId == 0 && fleetoverviewItem.LatestReceivedPositionLattitude != 0 && fleetoverviewItem.LatestReceivedPositionLongitude != 0)
                        {
                            GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(fleetoverviewItem.LatestReceivedPositionLattitude, fleetoverviewItem.LatestReceivedPositionLongitude);
                            fleetoverviewItem.LatestGeolocationAddressId = getMapRequestLatest.Id;
                            fleetoverviewItem.LatestGeolocationAddress = getMapRequestLatest.Address;
                        }
                        if (fleetoverviewItem.LatestWarningGeolocationAddressId == 0 && fleetoverviewItem.LatestWarningPositionLatitude != 0 && fleetoverviewItem.LatestWarningPositionLongitude != 0)
                        {
                            GetMapRequest getMapRequestWarning = _hereMapAddressProvider.GetAddressObject(fleetoverviewItem.LatestWarningPositionLatitude, fleetoverviewItem.LatestWarningPositionLongitude);
                            fleetoverviewItem.LatestWarningGeolocationAddressId = getMapRequestWarning.Id;
                            fleetoverviewItem.LatestWarningGeolocationAddress = getMapRequestWarning.Address;
                        }
                        if (fleetoverviewItem.StartGeolocationAddressId == 0 && fleetoverviewItem.StartPositionLattitude != 0 && fleetoverviewItem.StartPositionLongitude != 0)
                        {
                            GetMapRequest getMapRequestStart = _hereMapAddressProvider.GetAddressObject(fleetoverviewItem.StartPositionLattitude, fleetoverviewItem.StartPositionLongitude);
                            fleetoverviewItem.StartGeolocationAddressId = getMapRequestStart.Id;
                            fleetoverviewItem.StartGeolocationAddress = getMapRequestStart.Address;
                        }
                        for (int i = 0; i < fleetoverviewItem.FleetOverviewAlert.Count; i++)
                        {
                            if (string.IsNullOrEmpty(fleetoverviewItem.FleetOverviewAlert[i].GeolocationAddress) && fleetoverviewItem.FleetOverviewAlert[i].Latitude != 0 && fleetoverviewItem.FleetOverviewAlert[i].Longitude != 0)
                            {
                                GetMapRequest getMapRequestStart = _hereMapAddressProvider.GetAddressObject(fleetoverviewItem.FleetOverviewAlert[i].Latitude, fleetoverviewItem.FleetOverviewAlert[i].Longitude);
                                fleetoverviewItem.FleetOverviewAlert[i].GeolocationAddressId = getMapRequestStart.Id;
                                fleetoverviewItem.FleetOverviewAlert[i].GeolocationAddress = getMapRequestStart.Address;
                            }
                        }
                    }
                    return Ok(response.FleetOverviewDetailList);
                }
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.FLEETOVERVIEW_FILTER_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                 ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetFleetOverviewDetails) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetFleetOverviewDetails)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("fleetoverview/getfilterpoidetails")]
        public async Task<IActionResult> GetFleetOverviewPoiFilter()
        {
            try
            {
                ReportFleetOverviewPoiFilter reportFleetOverviewFilter = new ReportFleetOverviewPoiFilter();
                poiservice.POIRequest poiRequest = new poiservice.POIRequest();
                poiRequest.OrganizationId = GetContextOrgId(); //36;
                poiRequest.Type = "POI";
                var data = await _poiServiceClient.GetAllPOIAsync(poiRequest);
                reportFleetOverviewFilter.UserPois = new List<POI.POIResponse>();
                reportFleetOverviewFilter.GlobalPois = new List<POI.POIResponse>();
                foreach (var item in data.POIList)
                {
                    if (item.OrganizationId > 0)
                        reportFleetOverviewFilter.UserPois.Add(_mapper.ToPOIEntity(item));
                    else
                        reportFleetOverviewFilter.GlobalPois.Add(_mapper.ToPOIEntity(item));
                }
                if (data == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (data.Code.ToString() == Responsecode.Success.ToString())
                    return Ok(reportFleetOverviewFilter);
                if (data.Code.ToString() == Responsecode.InternalServerError.ToString())
                    return StatusCode((int)data.Code, String.Format(ReportConstants.FLEETOVERVIEW_FILTER_FAILURE_MSG, data.Message));
                return StatusCode((int)data.Code, data.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                 ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetFleetOverviewPoiFilter) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetFleetOverviewPoiFilter)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        #endregion

        #region Fleet Fuel Report Details
        [HttpPost]
        [Route("fleetfuel/getdetails/vehicle")]
        public async Task<IActionResult> GetFleetFuelDetailsByVehicle([FromBody] Entity.Report.ReportFleetFuelFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByVehicle method in Report (for Fleet Fuel consumption details by vehicle) API called.");
                var data = await _reportServiceClient.GetFleetFuelDetailsByVehicleAsync(objFleetFilter);
                if (data?.FleetFuelDetails?.Count > 0)
                {
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelDetailsByVehicle)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        /* TODO :: Un Comment Once Setup of Driver is completed      */
        [HttpPost]
        [Route("fleetfuel/getdetails/driver")]
        public async Task<IActionResult> GetFleetFuelDetailsByDriver([FromBody] Entity.Report.ReportFleetFuelFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByDriver method in Report (for Fleet Fuel consumption details by Driver) API called.");
                var data = await _reportServiceClient.GetFleetFuelDetailsByDriverAsync(objFleetFilter);
                if (data?.FleetFuelDetails?.Count > 0)
                {
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelDetailsByDriver)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fleetfuel/getdetails/vehiclegraph")]
        public async Task<IActionResult> GetFleetFuelDetailsForVehicleGraphs([FromBody] Entity.Report.ReportFleetFuelFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByDriver method in Report (for Fleet Fuel consumption details by Driver) API called.");
                var data = await _reportServiceClient.GetFleetFuelDetailsForVehicleGraphsAsync(objFleetFilter);
                if (data?.FleetfuelGraph?.Count > 0)
                {
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelDetailsForVehicleGraphs)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fleetfuel/getdetails/drivergraph")]
        public async Task<IActionResult> GetFleetFuelDetailsForDriverGraphs([FromBody] Entity.Report.ReportFleetFuelFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByDriver method in Report (for Fleet Fuel consumption details by Driver) API called.");
                var data = await _reportServiceClient.GetFleetFuelDetailsForDriverGraphsAsync(objFleetFilter);
                if (data?.FleetfuelGraph?.Count > 0)
                {
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelDetailsForDriverGraphs)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fleetfuel/getdetails/trip")]
        public async Task<IActionResult> GetFleetFuelTripByVehicle([FromBody] Entity.Report.ReportFleetFuelFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VINs.Count <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByVehicle method in Report (for Fleet Fuel consumption details by vehicle) API called.");
                var data = await _reportServiceClient.GetFleetFuelTripDetailsByVehicleAsync(objFleetFilter);
                if (data?.FleetFuelDetails?.Count > 0)
                {
                    foreach (var item in data.FleetFuelDetails)
                    {
                        if (string.IsNullOrEmpty(item.StartPosition) && item.Startpositionlattitude != 0 && item.Startpositionlongitude != 0)
                        {
                            var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.Startpositionlattitude, item.Startpositionlongitude);
                            item.StartPosition = getMapRequestLatest.Address;
                        }
                        if (string.IsNullOrEmpty(item.EndPosition) && item.Endpositionlattitude != 0 && item.Endpositionlongitude != 0)
                        {
                            var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.Endpositionlattitude, item.Endpositionlongitude);
                            item.EndPosition = getMapRequestLatest.Address;
                        }
                    }
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelTripByVehicle)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        [HttpPost]
        [Route("fleetfuel/getdetails/driver/trip")]
        public async Task<IActionResult> GetFleetFuelTripByDriver([FromBody] Entity.Report.ReportFleetFuelDriverFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0)) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG); }
                if (request.VIN.Length <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG); }
                // if (request.DriverId.Length <= 0) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DRIVERID_MSG); }

                if (request.StartDateTime > request.EndDateTime) { return BadRequest(ReportConstants.GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FleetFuelFilterDriverRequest objFleetFilter = JsonConvert.DeserializeObject<FleetFuelFilterDriverRequest>(filters);
                _logger.Info("GetFleetFuelDetailsByDriver method in Report (for Fleet Fuel consumption details by driver) API called.");
                var data = await _reportServiceClient.GetFleetFuelTripDetailsByDriverAsync(objFleetFilter);
                if (data?.FleetFuelDetails?.Count > 0)
                {
                    foreach (var item in data.FleetFuelDetails)
                    {
                        if (string.IsNullOrEmpty(item.StartPosition) && item.Startpositionlattitude != 0 && item.Startpositionlongitude != 0)
                        {
                            var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.Startpositionlattitude, item.Startpositionlongitude);
                            item.StartPosition = getMapRequestLatest.Address;
                        }
                        if (string.IsNullOrEmpty(item.EndPosition) && item.Endpositionlattitude != 0 && item.Endpositionlongitude != 0)
                        {
                            var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.Endpositionlattitude, item.Endpositionlongitude);
                            item.EndPosition = getMapRequestLatest.Address;
                        }
                    }
                    data.Message = ReportConstants.GET_FLEET_FUEL_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FLEET_FUEL_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFleetFuelTripByDriver)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Vehicle Health Summary
        [HttpGet]
        [Route("fleetoverview/getvehiclehealthstatus")]
        public async Task<IActionResult> GetVehicleHealthReport([FromQuery] Entity.Report.VehicleHealthStatusRequest request)
        {
            try
            {
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                string filters = JsonConvert.SerializeObject(request);
                net.atos.daf.ct2.reportservice.VehicleHealthReportRequest objVehicleHealthStatusRequest = JsonConvert.DeserializeObject<VehicleHealthReportRequest>(filters);
                objVehicleHealthStatusRequest.AccountId = _userDetails.AccountId;
                objVehicleHealthStatusRequest.OrganizationId = GetContextOrgId();
                _logger.Info("GetVehicleHealthReport method in Report (for Vehicle Current and History Summary) API called.");

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));

                var data = await _reportServiceClient.GetVehicleHealthReportAsync(objVehicleHealthStatusRequest, headers);

                if (data != null)
                {
                    var vehicleHealthStatus = _mapper.ToVehicleHealthStatus(data);

                    foreach (var hs in vehicleHealthStatus)
                    {
                        //if (hs.LatestGeolocationAddressId == 0 && hs.LatestReceivedPositionLattitude != 0 && hs.LatestReceivedPositionLongitude != 0)
                        //{
                        //    GetMapRequest getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(hs.LatestReceivedPositionLattitude, hs.LatestReceivedPositionLongitude);
                        //    hs.LatestGeolocationAddressId = getMapRequestLatest.Id;
                        //    hs.LatestGeolocationAddress = getMapRequestLatest.Address;
                        //}
                        //if (hs.LatestWarningGeolocationAddressId == 0 && hs.LatestWarningPositionLatitude != 0 && hs.LatestWarningPositionLongitude != 0)
                        //{
                        //    GetMapRequest getMapRequestWarning = _hereMapAddressProvider.GetAddressObject(hs.LatestWarningPositionLatitude, hs.LatestWarningPositionLongitude);
                        //    hs.LatestWarningGeolocationAddressId = getMapRequestWarning.Id;
                        //    hs.LatestWarningGeolocationAddress = getMapRequestWarning.Address;
                        //}
                        //if (hs.StartGeolocationAddressId == 0 && hs.StartPositionLattitude != 0 && hs.StartPositionLongitude != 0)
                        //{
                        //    GetMapRequest getMapRequestStart = _hereMapAddressProvider.GetAddressObject(hs.StartPositionLattitude, hs.StartPositionLongitude);
                        //    hs.StartGeolocationAddressId = getMapRequestStart.Id;
                        //    hs.StartGeolocationAddress = getMapRequestStart.Address;
                        //}
                        if (hs.WarningAddressId == 0 && hs.WarningLat != 0 && hs.WarningLng != 0 && hs.WarningLat != 255 && hs.WarningLng != 255)
                        {
                            GetMapRequest getMapRequestStart = new GetMapRequest();
                            getMapRequestStart = _hereMapAddressProvider.GetAddressObject(hs.WarningLat, hs.WarningLng);
                            hs.WarningAddressId = getMapRequestStart.Id;
                            hs.WarningAddress = getMapRequestStart.Address;
                        }
                    }

                    data.Message = ReportConstants.SUCCESS_MSG;
                    return Ok(vehicleHealthStatus);
                }
                else
                {
                    return StatusCode(404, ReportConstants.FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                $"{ nameof(GetVehicleHealthReport) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(request),
                 _userDetails);
                _logger.Error($"{nameof(GetVehicleHealthReport)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Fuel Deviation Report

        #region Fuel Deviation Report Table Details         
        [HttpPost]
        [Route("fueldeviation/getdetails")]
        public async Task<IActionResult> GetFuelDeviationFilterData(FuelDeviationFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) return BadRequest(ReportConstants.VALIDATION_STARTDATE_MSG);
                if (!(request.EndDateTime > 0)) return BadRequest(ReportConstants.VALIDATION_ENDDATE_MSG);
                if (request.VINs == null || request.VINs?.Count == 0) return BadRequest(ReportConstants.VALIDATION_VINREQUIRED_MSG);
                if (request.StartDateTime > request.EndDateTime) return BadRequest(ReportConstants.VALIDATION_DATEMISMATCH_MSG);

                _logger.Info("GetFilteredFuelDeviationAsync method in Report (Fuel Deviation Report) API called.");
                Metadata headers = new Metadata();
                headers.Add("context_orgId", Convert.ToString(GetContextOrgId()));
                string filters = JsonConvert.SerializeObject(request);
                var response = await _reportServiceClient.GetFilteredFuelDeviationAsync(JsonConvert.DeserializeObject<FuelDeviationFilterRequest>(filters), headers);

                foreach (var item in response.FuelDeviationDetails)
                {
                    if (item.GeoLocationAddressId == 0 && item.EventLatitude != 0 && item.EventLongitude != 0)
                    {
                        var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.EventLatitude, item.EventLongitude);
                        item.GeoLocationAddress = getMapRequestLatest.Address;
                        item.GeoLocationAddressId = getMapRequestLatest.Id;
                    }
                    if (item.StartPositionId == 0 && item.StartPositionLattitude != 0 && item.StartPositionLongitude != 0)
                    {
                        var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.StartPositionLattitude, item.StartPositionLongitude);
                        item.StartPosition = getMapRequestLatest.Address;
                        item.StartPositionId = getMapRequestLatest.Id;
                    }
                    if (item.EndPositionId == 0 && item.EndPositionLattitude != 0 && item.EndPositionLongitude != 0)
                    {
                        var getMapRequestLatest = _hereMapAddressProvider.GetAddressObject(item.EndPositionLattitude, item.EndPositionLongitude);
                        item.EndPosition = getMapRequestLatest.Address;
                        item.EndPositionId = getMapRequestLatest.Id;
                    }
                }
                if (response?.FuelDeviationDetails?.Count > 0)
                {
                    return Ok(new { Data = response.FuelDeviationDetails, Message = ReportConstants.GET_FUEL_DEVIATION_SUCCESS_MSG });
                }
                else
                {
                    return StatusCode((int)response.Code, response.Message);
                }
            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                                "Report service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED, ReportConstants.GET_FUEL_DEVIATION_FAIL_MSG, 0, 0, string.Empty,
                                 _userDetails);
                _logger.Error($"{nameof(GetFuelDeviationFilterData)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        [HttpPost]
        [Route("fueldeviation/charts")]
        public async Task<IActionResult> GetFuelDeviationChartData(FuelDeviationFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0)) return BadRequest(ReportConstants.VALIDATION_STARTDATE_MSG);
                if (!(request.EndDateTime > 0)) return BadRequest(ReportConstants.VALIDATION_ENDDATE_MSG);
                if (request.VINs == null || request.VINs?.Count == 0) return BadRequest(ReportConstants.VALIDATION_VINREQUIRED_MSG);
                if (request.StartDateTime > request.EndDateTime) return BadRequest(ReportConstants.VALIDATION_DATEMISMATCH_MSG);

                _logger.Info("GetFilteredFuelDeviationChart method in Report (Fuel Deviation charts) API called.");
                string filters = JsonConvert.SerializeObject(request);
                var response = await _reportServiceClient.GetFuelDeviationChartsAsync(JsonConvert.DeserializeObject<FuelDeviationFilterRequest>(filters));

                if (response?.FuelDeviationchart?.Count > 0)
                {
                    return Ok(new { Data = response.FuelDeviationchart, Message = ReportConstants.GET_FUEL_DEVIATION_SUCCESS_MSG });
                }
                else
                {
                    return StatusCode((int)response.Code, response.Message);
                }
            }

            catch (Exception ex)
            {

                _logger.Error($"{nameof(GetFuelDeviationChartData)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region Logbook

        [HttpGet]
        [Route("fleetoverview/getlogbookfilters")]
        public async Task<IActionResult> GetLogBookFilter()
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var alertFeatureIds = GetMappedFeatureIdByStartWithName(ReportConstants.FLEETOVERVIEW_ALERT_FEATURE_STARTWITH);

                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());
                var logBookFilterRequest = new LogbookFilterIdRequest();
                logBookFilterRequest.AccountId = _userDetails.AccountId;
                logBookFilterRequest.OrganizationId = GetContextOrgId();
                logBookFilterRequest.RoleId = _userDetails.RoleId;
                // logBookFilterRequest.AccountId = 171;
                //  logBookFilterRequest.OrganizationId = 36;
                // logBookFilterRequest.RoleId = 61;

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));
                headers.Add("alert_feature_ids", JsonConvert.SerializeObject(alertFeatureIds));

                LogbookFilterResponse response = await _reportServiceClient.GetLogbookSearchParameterAsync(logBookFilterRequest, headers);

                // reportFleetOverviewFilter = _mapper.ToFleetOverviewEntity(response);



                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response.LogbookSearchParameter);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.FLEETOVERVIEW_FILTER_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                 ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"{ nameof(GetFleetOverviewFilter) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetLogBookFilter)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fleetoverview/getlogbookdetails")]
        public async Task<IActionResult> GetLogbookDetails([FromBody] LogbookDetailsFilter logbookFilter)
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var alertFeatureIds = GetMappedFeatureIdByStartWithName(ReportConstants.FLEETOVERVIEW_ALERT_FEATURE_STARTWITH);
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                LogbookDetailsRequest logbookDetailsRequest = new LogbookDetailsRequest
                {
                    AccountId = _userDetails.AccountId,
                    OrganizationId = GetContextOrgId(),
                    RoleId = _userDetails.RoleId
                };

                logbookDetailsRequest.GroupIds.AddRange(logbookFilter.GroupId);
                logbookDetailsRequest.VIN.AddRange(logbookFilter.VIN);
                logbookDetailsRequest.AlertLevels.AddRange(logbookFilter.AlertLevel);
                logbookDetailsRequest.AlertType.AddRange(logbookFilter.AlertType);
                logbookDetailsRequest.AlertCategories.AddRange(logbookFilter.AlertCategory);
                logbookDetailsRequest.StartTime = logbookFilter.Start_Time;
                logbookDetailsRequest.EndTime = logbookFilter.End_time;
                /* Need to comment Start */
                // logbookDetailsRequest.AccountId = 171;
                // logbookDetailsRequest.OrganizationId = 36;
                // logbookDetailsRequest.RoleId = 61;
                /* Need to comment End */

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));
                headers.Add("alert_feature_ids", JsonConvert.SerializeObject(alertFeatureIds));

                LogbookDetailsResponse response = await _reportServiceClient.GetLogbookDetailsAsync(logbookDetailsRequest, headers);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == Responsecode.Success)
                    return Ok(response.LogbookDetails);
                if (response.Code == Responsecode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportConstants.FLEETOVERVIEW_FILTER_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                $"{ nameof(GetLogbookDetails) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                 _userDetails);
                _logger.Error($"{nameof(GetLogbookDetails)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }

        }
        #endregion

        #region Fuel Benchmark Details Report

        [HttpPost]
        [Route("fuelbenchmark/vehiclegroup")]
        public async Task<IActionResult> GetFuelBenchmarkByVehicleGroup([FromBody] Entity.Report.ReportFuelBenchmarkFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_ENDDATE_MSG); }
                if (request.StartDateTime > request.EndDateTime)
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FuelBenchmarkRequest objFleetFilter = JsonConvert.DeserializeObject<FuelBenchmarkRequest>(filters);
                var data = await _reportServiceClient.GetFuelBenchmarkByVehicleGroupAsync(objFleetFilter);
                if (data?.FuelBenchmarkDetails != null)
                {
                    if (request.VehicleGroupId > 0)
                    {
                        data.FuelBenchmarkDetails.VehicleGroupId = request.VehicleGroupId;
                        VehicleCountFilterRequest vehicleRequest = new VehicleCountFilterRequest();
                        vehicleRequest.VehicleGroupId = request.VehicleGroupId;
                        vehicleRequest.OrgnizationId = GetContextOrgId();
                        VehicleCountFilterResponse vehicleResponse = await _vehicleClient.GetVehicleAssociatedGroupCountAsync(vehicleRequest);
                        data.FuelBenchmarkDetails.NumberOfTotalVehicles = vehicleResponse.VehicleCount;
                    }
                    else
                    {
                        AssociatedVehicleResponse vehicleGroupResponse = await _reportServiceClient.GetAssociatedVehiclGroupAsync(new VehicleListRequest { AccountId = _userDetails.AccountId, OrganizationId = GetContextOrgId() });
                        if (vehicleGroupResponse.Code == Responsecode.Success)
                        {
                            int vehicleCount = 0;
                            foreach (var item in vehicleGroupResponse.AssociatedVehicle)
                            {
                                if (item.VehicleGroupId > 0)
                                {
                                    VehicleCountFilterRequest vehicleRequest = new VehicleCountFilterRequest();
                                    vehicleRequest.VehicleGroupId = item.VehicleGroupId;
                                    vehicleRequest.OrgnizationId = GetContextOrgId();
                                    VehicleCountFilterResponse vehicleResponse = await _vehicleClient.GetVehicleAssociatedGroupCountAsync(vehicleRequest);
                                    vehicleCount = vehicleCount + vehicleResponse.VehicleCount;
                                }
                            }

                            data.FuelBenchmarkDetails.NumberOfTotalVehicles = vehicleCount;
                        }
                    }

                    data.Message = ReportConstants.GET_FUEL_BENCHMARK_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FUEL_BENCHMARK_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFuelBenchmarkByVehicleGroup)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpPost]
        [Route("fuelbenchmark/timeperiod")]
        public async Task<IActionResult> GetFuelBenchmarkByTimePeriod([FromBody] Entity.Report.ReportFuelBenchmarkFilter request)
        {
            try
            {
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                if (!(request.StartDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_ENDDATE_MSG); }
                if (request.StartDateTime > request.EndDateTime)
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                FuelBenchmarkTimePeriodRequest objFluelBenchMarkFilter = JsonConvert.DeserializeObject<FuelBenchmarkTimePeriodRequest>(filters);
                objFluelBenchMarkFilter.AccountId = _userDetails.AccountId;
                objFluelBenchMarkFilter.OrganizationId = GetContextOrgId();

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_id", Convert.ToString(featureId));

                var data = await _reportServiceClient.GetFuelBenchmarkByTimePeriodAsync(objFluelBenchMarkFilter, headers);
                if (data?.FuelBenchmarkDetails != null)
                {
                    //Vehicle Group
                    if (request.VehicleGroupId > 0)
                    {
                        data.FuelBenchmarkDetails.VehicleGroupId = request.VehicleGroupId;
                        VehicleCountFilterRequest vehicleRequest = new VehicleCountFilterRequest();
                        vehicleRequest.VehicleGroupId = request.VehicleGroupId;
                        vehicleRequest.OrgnizationId = GetContextOrgId();
                        VehicleCountFilterResponse vehicleResponse = await _vehicleClient.GetVehicleAssociatedGroupCountAsync(vehicleRequest);
                        data.FuelBenchmarkDetails.NumberOfTotalVehicles = vehicleResponse.VehicleCount;
                    }
                    //Find vehicle group according to time period 
                    else
                    {
                        AssociatedVehicleResponse vehicleGroupResponse = await _reportServiceClient.GetAssociatedVehiclGroupAsync(new VehicleListRequest { AccountId = _userDetails.AccountId, OrganizationId = GetContextOrgId() }, headers);
                        if (vehicleGroupResponse.Code == Responsecode.Success)
                        {
                            int vehicleCount = 0;
                            foreach (var item in vehicleGroupResponse.AssociatedVehicle)
                            {
                                if (item.VehicleGroupId > 0)
                                {
                                    VehicleCountFilterRequest vehicleRequest = new VehicleCountFilterRequest();
                                    vehicleRequest.VehicleGroupId = item.VehicleGroupId;
                                    vehicleRequest.OrgnizationId = GetContextOrgId();
                                    VehicleCountFilterResponse vehicleResponse = await _vehicleClient.GetVehicleAssociatedGroupCountAsync(vehicleRequest);
                                    vehicleCount = vehicleCount + vehicleResponse.VehicleCount;
                                }
                            }

                            data.FuelBenchmarkDetails.NumberOfTotalVehicles = vehicleCount;
                        }
                    }

                    data.Message = ReportConstants.GET_FUEL_BENCHMARK_SUCCESS_MSG;
                    return Ok(data);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FUEL_BENCHMARK_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetFuelBenchmarkByTimePeriod)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion
        #region VehiclePerformance
        [HttpPost]
        [Route("vehicleperformance/charttemplate")]
        public async Task<IActionResult> GetVehiclePerformanceChartTemplate([FromBody] VehiclePerformanceFilter request)
        {
            try
            {
                if (!(request.StartDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_STARTDATE_MSG); }
                if (!(request.EndDateTime > 0))
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_ENDDATE_MSG); }
                if (request.StartDateTime > request.EndDateTime)
                { return BadRequest(ReportConstants.GET_FUEL_BENCHMARK_VALIDATION_DATEMISMATCH_MSG); }

                string filters = JsonConvert.SerializeObject(request);
                VehPerformanceRequest objVehPerformanceFilter = JsonConvert.DeserializeObject<VehPerformanceRequest>(filters);
                var data = await _reportServiceClient.GetVehiclePerformanceChartTemplateAsync(objVehPerformanceFilter);
                if (data?.VehPerformanceTemplate?.VehPerformanceCharts != null)
                {
                    data.Message = ReportConstants.GET_VEHICLE_PERFORMANCE_SUCCESS_MSG;
                    return Ok(data.VehPerformanceTemplate);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FUEL_BENCHMARK_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetVehiclePerformanceChartTemplate)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        [HttpPost]
        [Route("vehicleperformance/chartdata")]
        public async Task<IActionResult> GetVehPerformanceBubbleChartData(VehiclePerformanceFilter vehiclePerformanceFilter)
        {
            try
            {
                BubbleChartDataRequest bubbleChartDataRequest = new BubbleChartDataRequest();
                bubbleChartDataRequest.VIN = vehiclePerformanceFilter.VIN;
                bubbleChartDataRequest.PerformanceType = vehiclePerformanceFilter.PerformanceType;
                bubbleChartDataRequest.StartDateTime = vehiclePerformanceFilter.StartDateTime;
                bubbleChartDataRequest.EndDateTime = vehiclePerformanceFilter.EndDateTime;

                string filters = JsonConvert.SerializeObject(vehiclePerformanceFilter);
                BubbleChartDataRequest objVehPerformanceFilter = JsonConvert.DeserializeObject<BubbleChartDataRequest>(filters);
                var response = await _reportServiceClient.GetVehPerformanceBubbleChartDataAsync(objVehPerformanceFilter);

                if (response != null)
                {
                    response.Message = ReportConstants.GET_VEHICLE_PERFORMANCE_SUCCESS_MSG;
                    return Ok(response);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FUEL_BENCHMARK_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                $"{ nameof(GetVehPerformanceBubbleChartData) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                 _userDetails);
                _logger.Error($"{nameof(GetVehPerformanceBubbleChartData)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("vehicleperformance/kpi")]
        public async Task<IActionResult> GetVehPerformancetype()
        {
            try
            {

                VehPerformanceTypeRequest vehPerformanceTypeRequest = new VehPerformanceTypeRequest();
                var response = await _reportServiceClient.GetVehPerformanceTypeAsync(vehPerformanceTypeRequest);
                if (response != null)
                {
                    response.Message = ReportConstants.GET_VEHICLE_PERFORMANCE_SUCCESS_MSG;
                    return Ok(response.VehPerformanceType);
                }
                else
                {
                    return StatusCode(404, ReportConstants.GET_FUEL_BENCHMARK_FAILURE_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Report Controller",
                ReportConstants.FLEETOVERVIEW_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                $"{ nameof(GetVehPerformancetype) } method Failed. Error : {ex.Message}", 1, 2, Convert.ToString(_userDetails.AccountId),
                 _userDetails);
                _logger.Error($"{nameof(GetVehPerformancetype)}: With Error:-", ex);
                return StatusCode(500, ReportConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.otasoftwareupdate;
using net.atos.daf.ct2.otasoftwareupdate.common;
using net.atos.daf.ct2.otasoftwareupdateservice.Entity;
using net.atos.daf.ct2.visibility;
using static net.atos.daf.ct2.httpclientservice.HttpClientService;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Services
{

    public class OTASoftwareUpdateManagementService : OTASoftwareUpdateService.OTASoftwareUpdateServiceBase
    {
        private readonly ILog _logger;
        private readonly IOTASoftwareUpdateManager _otaSoftwareUpdateManagement;
        private readonly IVisibilityManager _visibilityManager;
        private readonly HttpClientServiceClient _httpClientServiceClient;
        private readonly CampiagnDataCaching _campiagnDataCaching;
        private readonly Mapper _mapper;

        public OTASoftwareUpdateManagementService(IOTASoftwareUpdateManager otaSoftwareUpdateManagement
                                                  , IVisibilityManager visibilityManager,
                                                    HttpClientServiceClient httpClientServiceClient,
                                                    IMemoryCache cache)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _otaSoftwareUpdateManagement = otaSoftwareUpdateManagement;
            _visibilityManager = visibilityManager;
            _mapper = new Mapper();
            _httpClientServiceClient = httpClientServiceClient;
            _campiagnDataCaching = new CampiagnDataCaching(cache);
        }

        public override async Task<VehicleSoftwareStatusResponse> GetVehicleSoftwareStatus(NoRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleSoftwareStatusList = await _otaSoftwareUpdateManagement.GetVehicleSoftwareStatus();

                var response = new VehicleSoftwareStatusResponse
                {
                    Message = "Successfully fetch records for Vehicle Software Status",
                    Code = ResponseCode.Success
                };

                response.VehicleSoftwareStatusList.AddRange(
                                                vehicleSoftwareStatusList.Select(s =>
                                                        new VehicleSoftwareStatus
                                                        {
                                                            Id = s.Id,
                                                            Type = s.Type ?? string.Empty,
                                                            Enum = s.Enum ?? string.Empty,
                                                            Key = s.Key ?? string.Empty,
                                                            ParentEnum = s.ParentEnum ?? string.Empty,
                                                            FeatureId = s.FeatureId

                                                        }));
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleSoftwareStatusResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<VehicleStatusResponse> GetVehicleStatusList(VehicleStatusRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleStatusList = await _visibilityManager.GetVehicleByAccountVisibilityForOTA(request.AccountId, request.OrgId, request.ContextOrgId, request.FeatureId);
                if (vehicleStatusList.Count() > 0)
                {
                    var vinStatusResponse = await _httpClientServiceClient
                        .GetVehiclesStatusOverviewAsync(
                            _mapper.MapVehiclesStatusOverviewRequest(request.Language, request.Retention, vehicleStatusList.Select(s => s.Vin))
                            );
                    //var vinStatusResponse = await _httpClientServiceClient
                    //    .GetVehiclesStatusOverviewTempAsync(new httpclientservice.VehiclesStatusOverviewRequestTemp { Language = "testing" });
                    //foreach (var item in vehicleStatusList)
                    //{
                    //    item.SoftwareStatus = 
                    //}
                    var response = new VehicleStatusResponse
                    {
                        Message = "Successfully fetch records for Vehicle Software Status",
                        Code = ResponseCode.Success
                    };
                    response.VehicleStatusList.AddRange(
                                                    vehicleStatusList.Select(s =>
                                                            new VehicleStatusList
                                                            {
                                                                VehicleId = s.VehicleId,
                                                                VehicleName = s.VehicleName ?? string.Empty,
                                                                Vin = s.Vin ?? string.Empty,
                                                                RegistrationNo = s.RegistrationNo ?? string.Empty,
                                                                VehicleGroupNames = s.VehicleGroupNames ?? string.Empty,
                                                                ModelYear = s.ModelYear ?? string.Empty,
                                                                Type = s.Type ?? string.Empty,
                                                                SoftwareStatus = vinStatusResponse?.VehiclesStatusOverview?
                                                                                .VehiclesStatusOverviewResults?.Where(w => w.Vin?.ToLower() == s.Vin?.ToLower())?
                                                                                .FirstOrDefault()?.Status ?? string.Empty
                                                            }));
                    return await Task.FromResult(response);
                }
                return await Task.FromResult(new VehicleStatusResponse
                {
                    Message = "No records found for in Vehicle Account visibility.",
                    Code = ResponseCode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleStatusResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<VehicleUpdateDetailResponse> GetVehicleUpdateDetails(VehicleUpdateDetailRequest request, ServerCallContext context)
        {
            try
            {

                var vinStatusResponse = await _httpClientServiceClient
                    .GetVehicleUpdateDetailsAsync(new httpclientservice.VehicleUpdateDetailsRequest
                    {
                        Retention = request.Retention,
                        Vin = request.Vin
                    });
                if (vinStatusResponse?.VehicleUpdateDetails?.Campaigns?.Count() > 0)
                {
                    var vehicleScheduleDetails = await _otaSoftwareUpdateManagement.GetSchduleCampaignByVin(request.Vin);
                    var response = new VehicleUpdateDetailResponse
                    {
                        Message = "Successfully fetch records for Vehicle Software Status",
                        HttpStatusCode = ResponseCode.Success
                    };
                    var count = vehicleScheduleDetails.Count();
                    response.VehicleUpdateDetail.VehicleSoftwareStatus = vinStatusResponse.VehicleUpdateDetails?.VehicleSoftwareStatus;
                    response.VehicleUpdateDetail.Vin = vinStatusResponse.VehicleUpdateDetails?.Vin;
                    response.VehicleUpdateDetail.Campaigns.AddRange(
                                                    vinStatusResponse.VehicleUpdateDetails.Campaigns.Select(s =>
                                                            new Campaign
                                                            {
                                                                CampaignID = s.CampaignID,
                                                                BaselineAssignment = s.BaselineAssignment,
                                                                CampaignSubject = s.CampaignSubject,
                                                                CampaignCategory = s.CampaignCategory,
                                                                CampaignType = s.CampaignType,
                                                                UpdateStatus = s.UpdateStatus,
                                                                EndDate = s.EndDate,
                                                                ScheduleDateTime = count > 0 ? vehicleScheduleDetails?
                                                                                .Where(w => w.CampaignId?.ToLower() == s.CampaignID?.ToLower() && w.BaselineAssignment?.ToLower() == s.BaselineAssignment?.ToLower())?
                                                                                .FirstOrDefault()?.ScheduleDateTime ?? 0 : 0
                                                            }));
                    return await Task.FromResult(response);
                }
                return await Task.FromResult(new VehicleUpdateDetailResponse
                {
                    Message = "No records found for in Vehicle Campaigns.",
                    HttpStatusCode = ResponseCode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleUpdateDetailResponse
                {
                    Message = "Exception :-" + ex.Message,
                    HttpStatusCode = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<CampiagnSoftwareReleaseNoteResponse> GetSoftwareReleaseNote(CampiagnSoftwareReleaseNoteRequest request, ServerCallContext context)
        {
            try
            {
                var releaseNotes = await GetCampaignData(request.CampaignId,
                                                         request.Language,
                                                         request.Retention,
                                                         request.Vins);

                return await Task.FromResult(new CampiagnSoftwareReleaseNoteResponse
                {
                    ReleaseNote = releaseNotes,
                    Message = string.IsNullOrEmpty(releaseNotes) ? "No records found for in Vehicle Campaigns." :
                                                                   "Fetched campaign detials successfully.",
                    HttpStatusCode = ResponseCode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CampiagnSoftwareReleaseNoteResponse
                {
                    Message = "Exception :-" + ex.Message,
                    HttpStatusCode = ResponseCode.InternalServerError
                });
            }
        }

        #region Get Campaign Data logic
        private async Task<string> GetCampaignData(string campaignID, string code, string retention, IEnumerable<string> vins)
        {
            var releaseNotes = await _campiagnDataCaching
                                .GetReleaseNotesFromCache(new CampiagnData
                                {
                                    CampaignId = campaignID,
                                    Code = code
                                });

            if (string.IsNullOrEmpty(releaseNotes))
            {
                releaseNotes = await GetCampaignDataFromDB(campaignID, code, retention, vins);
                if (!string.IsNullOrEmpty(releaseNotes))
                {
                    await _campiagnDataCaching
                                .InsertReleaseNotesToCache(new CampiagnData
                                {
                                    CampaignId = campaignID,
                                    Code = code,
                                    ReleaseNotes = releaseNotes
                                });
                }
            }
            return releaseNotes;
        }
        private async Task<string> GetCampaignDataFromDB(string campaignID, string code, string retention, IEnumerable<string> vins)
        {
            var releaseNotes = await _otaSoftwareUpdateManagement.GetReleaseNotes(campaignID, code);
            if (string.IsNullOrEmpty(releaseNotes))
            {
                releaseNotes = await GetCampaignDataFromAPI(retention, vins);
                if (!string.IsNullOrEmpty(releaseNotes))
                {
                    await _otaSoftwareUpdateManagement.InsertReleaseNotes(campaignID, code, releaseNotes);
                }
            }
            return releaseNotes;
        }
        private async Task<string> GetCampaignDataFromAPI(string retention, IEnumerable<string> vins)
        {
            var request = new httpclientservice.CampiagnSoftwareReleaseNoteRequest
            {
                Retention = retention
            };
            request.Vins.AddRange(vins);
            net.atos.daf.ct2.httpclientservice.CampiagnSoftwareReleaseNoteResponse 
                campiagnDataResponse = await _httpClientServiceClient
                                                            .GetSoftwareReleaseNoteAsync(request);
            return campiagnDataResponse?.ReleaseNote;
        }
        #endregion
    }
}

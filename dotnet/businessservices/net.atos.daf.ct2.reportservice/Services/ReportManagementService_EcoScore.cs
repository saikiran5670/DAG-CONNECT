using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        #region Eco Score Report - Create Profile
        /// <summary>
        /// Eco Score Create profile
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<CreateEcoScoreProfileResponse> CreateEcoScoreProfile(CreateEcoScoreProfileRequest request, ServerCallContext context)
        {
            var response = new CreateEcoScoreProfileResponse();
            try
            {
                var profileRequest = MapCreateProfileRequestToDto(request);

                if (profileRequest.OrganizationId.HasValue)
                {
                    var countByOrg = await _reportManager.GetEcoScoreProfilesCount(request.OrgId);
                    var maxLimit = Convert.ToInt32(_configuration["MaxAllowedEcoScoreProfiles"]);

                    if (countByOrg < maxLimit)
                    {
                        response = await CallCreateEcoScoreProfile(profileRequest);
                    }
                    else
                    {
                        response.Code = Responsecode.Forbidden;
                        response.Message = "Max limit has reached for the creation of Eco-Score profile of requested organization. New profile cannot be created.";
                    }
                }
                else
                {
                    response = await CallCreateEcoScoreProfile(profileRequest);
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CreateEcoScoreProfileResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = $"{nameof(CreateEcoScoreProfile)} failed due to - " + ex.Message
                });
            }
        }

        private async Task<CreateEcoScoreProfileResponse> CallCreateEcoScoreProfile(EcoScoreProfileDto profileRequest)
        {
            await _reportManager.CreateEcoScoreProfile(profileRequest);
            return new CreateEcoScoreProfileResponse()
            {
                Code = Responsecode.Success,
                Message = "Eco-Score profile is created successfully."
            };
        }

        /// <summary>
        /// Mapper to create DTO object from GRPC service request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private EcoScoreProfileDto MapCreateProfileRequestToDto(CreateEcoScoreProfileRequest request)
        {
            var dto = new EcoScoreProfileDto();
            List<EcoScoreProfileKPI> profileKPIs = new List<EcoScoreProfileKPI>();
            dto.Name = request.Name;
            dto.OrganizationId = request.IsDAFStandard ? new int?() : request.OrgId;
            dto.Description = request.Description;
            dto.ActionedBy = Convert.ToString(request.AccountId);
            foreach (var profileKPI in request.ProfileKPIs)
            {
                profileKPIs.Add(new EcoScoreProfileKPI()
                {
                    KPIId = profileKPI.KPIId,
                    LimitValue = profileKPI.LimitValue,
                    TargetValue = profileKPI.TargetValue,
                    LowerValue = profileKPI.LowerValue,
                    UpperValue = profileKPI.UpperValue
                });
            }
            dto.ProfileKPIs = profileKPIs;

            return dto;
        }

        #endregion

        #region Eco Score Report - Get Profile and KPI Details

        /// <summary>
        /// Get list of EcoScore Profiles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<GetEcoScoreProfileResponse> GetEcoScoreProfiles(GetEcoScoreProfileRequest request, ServerCallContext context)
        {
            var response = new GetEcoScoreProfileResponse();
            try
            {
                var result = await _reportManager.GetEcoScoreProfiles(request.OrgId);
                if (result?.Count > 0)
                {
                    response.Code = Responsecode.Success;
                    response.Message = entity.ReportConstants.GET_ECOSCORE_PROFILE_SUCCESS_MSG;
                    response.Profiles.AddRange(MapEcoScoreProfileResponse(result));
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = Responsecode.NotFound.ToString();
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GetEcoScoreProfileResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = $"{nameof(GetEcoScoreProfiles)} failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Mapper to create GRPC service response from DTO object for Get EcoScore Profiles
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<EcoScoreProfiles> MapEcoScoreProfileResponse(List<EcoScoreProfileDto> result)
        {
            List<EcoScoreProfiles> lstProfile = new List<EcoScoreProfiles>();
            foreach (var profile in result)
            {
                lstProfile.Add(new EcoScoreProfiles
                {
                    ProfileId = profile.Id,
                    ProfileName = profile.Name ?? string.Empty,
                    ProfileDescription = profile.Description ?? string.Empty,
                    IsDeleteAllowed = profile.IsDeleteAllowed,
                    OrganizationId = Convert.ToInt32(profile.OrganizationId),
                });
            }
            return lstProfile;
        }

        /// <summary>
        /// Get list of EcoScore Profiles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<GetEcoScoreProfileKPIResponse> GetEcoScoreProfileKPIDetails(GetEcoScoreProfileKPIRequest request, ServerCallContext context)
        {
            var response = new GetEcoScoreProfileKPIResponse();
            try
            {
                var result = await _reportManager.GetEcoScoreProfileKPIDetails(request.ProfileId);
                if (result != null)
                {
                    response.Code = Responsecode.Success;
                    response.Message = entity.ReportConstants.GET_ECOSCORE_PROFILE_KPI_SUCCESS_MSG;
                    response.Profile.Add(MapEcoScoreProfileKPIResponse(result));
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = Responsecode.NotFound.ToString();
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GetEcoScoreProfileKPIResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = $"{nameof(GetEcoScoreProfileKPIDetails)} failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Mapper to create GRPC service response from DTO object for Get EcoScore Profile KPI Details
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private EcoScoreProfileDetails MapEcoScoreProfileKPIResponse(EcoScoreProfileDto result)
        {
            //Profile Header Details
            var objProfile = new EcoScoreProfileDetails();
            objProfile.ProfileId = result.Id;
            objProfile.ProfileName = result.Name ?? string.Empty;
            objProfile.ProfileDescription = result.Description ?? string.Empty;
            objProfile.UpdatedBy = result.ActionedBy ?? string.Empty;
            objProfile.LastUpdate = result.LastUpdate.ToString("MM/dd/yyyy HH:mm:ss");

            if (result.ProfileKPIs != null)
            {
                var lstProfileSection = new List<EcoScoreProfileKPIs>();
                var section = result.ProfileKPIs.Select(x => new { x.SectionId, x.SectionName, x.SectionDescription }).Distinct();
                foreach (var sec in section)
                {
                    //Profile Sections Details
                    var objProfileKPI = new EcoScoreProfileKPIs();
                    objProfileKPI.SectionId = sec.SectionId;
                    objProfileKPI.SectionName = sec.SectionName;
                    objProfileKPI.SectionDescription = sec.SectionDescription;

                    var lstKPI = new List<EcoScoreKPI>();
                    var lstKPIDetails = result.ProfileKPIs.Where(x => x.SectionId == sec.SectionId).ToList();
                    foreach (var kpi in lstKPIDetails)
                    {
                        //Profile KPI Details
                        var objKPI = new EcoScoreKPI();
                        objKPI.EcoScoreKPIId = kpi.KPIId;
                        objKPI.KPIName = kpi.KPIName ?? string.Empty;
                        objKPI.LimitType = kpi.LimitType ?? string.Empty;
                        objKPI.LimitValue = kpi.LimitValue;
                        objKPI.TargetValue = kpi.TargetValue;
                        objKPI.LowerValue = kpi.LowerValue;
                        objKPI.UpperValue = kpi.UpperValue;
                        objKPI.RangeValueType = kpi.RangeValueType ?? string.Empty;
                        objKPI.MaxUpperValue = kpi.MaxUpperValue;
                        objKPI.SequenceNo = kpi.SequenceNo;
                        lstKPI.Add(objKPI);
                    }
                    objProfileKPI.ProfileKPIDetails.AddRange(lstKPI);
                    lstProfileSection.Add(objProfileKPI);
                }
                objProfile.ProfileSection.AddRange(lstProfileSection);
            }
            return objProfile;
        }

        #endregion

        #region Eco Score Report - Update Profile
        public override async Task<UpdateEcoScoreProfileResponse> UpdateEcoScoreProfile(UpdateEcoScoreProfileRequest request, ServerCallContext context)
        {
            var response = new UpdateEcoScoreProfileResponse();
            try
            {
                _logger.Info("Update Eco Score Profile Report .");
                bool isAdminRights = Convert.ToBoolean(context.RequestHeaders.Get("hasrights").Value);
                EcoScoreProfileDto obj = new EcoScoreProfileDto();
                obj.Id = request.ProfileId;
                obj.Name = request.Name;
                obj.OrganizationId = request.OrgId;
                obj.Description = request.Description;
                obj.ActionedBy = Convert.ToString(request.AccountId);
                obj.ProfileKPIs = new List<EcoScoreProfileKPI>();
                foreach (var item in request.ProfileKPIs)
                {
                    var data = new EcoScoreProfileKPI();
                    data.KPIId = item.KPIId;
                    data.LimitValue = item.LimitValue;
                    data.LowerValue = item.LowerValue;
                    data.TargetValue = item.TargetValue;
                    data.UpperValue = item.UpperValue;
                    obj.ProfileKPIs.Add(data);
                }

                var result = await _reportManager.UpdateEcoScoreProfile(obj, isAdminRights);

                if (result > 0)
                {
                    response.Message = obj.Name + entity.ReportConstants.UPDATE_ECOSCORE_PROFILE_SUCCESS_MSG;
                    response.Code = Responsecode.Success;

                }
                else if (result == -1)
                {
                    response.Message = obj.Name + entity.ReportConstants.UPDATE_ECOSCORE_PROFILE_NOT_EXIST_MSG;
                    response.Code = Responsecode.NotFound;
                }
                else if (result == -2)
                {
                    response.Message = entity.ReportConstants.UPDATE_ECOSCORE_PROFILE_DEFAULT_PROFILE_MSG;
                    response.Code = Responsecode.Failed;
                }
                else
                {
                    response.Message = obj.Name + entity.ReportConstants.UPDATE_ECOSCORE_PROFILE_FAIL_MSG;
                    response.Code = Responsecode.Failed;

                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new UpdateEcoScoreProfileResponse
                {
                    Code = Responsecode.Failed,
                    Message = entity.ReportConstants.UPDATE_ECOSCORE_PROFILE_FAIL_MSG + " due to - " + ex.Message
                });
            }
        }


        #endregion

        #region Eco Score Report - Delete Profile
        public override async Task<DeleteEcoScoreProfileResponse> DeleteEcoScoreProfile(DeleteEcoScoreProfileRequest request, ServerCallContext context)
        {
            DeleteEcoScoreProfileResponse response = new DeleteEcoScoreProfileResponse();
            try
            {
                _logger.Info("Delete Eco Score Profile .");
                bool isAdminRights = Convert.ToBoolean(context.RequestHeaders.LastOrDefault().Value);
                var result = await _reportManager.DeleteEcoScoreProfile(request.ProfileId, isAdminRights);
                if (result > 0)
                {
                    response.Message = entity.ReportConstants.DELETE_ECOSCORE_PROFILE_SUCCESS_MSG;
                    response.Code = Responsecode.Success;
                }
                else if (result == 0)
                {
                    response.Message = entity.ReportConstants.DELETE_ECOSCORE_PROFILE_NOT_EXIST_MSG;
                    response.Code = Responsecode.NotFound;
                }
                else if (result == -1)
                {
                    response.Message = entity.ReportConstants.DELETE_ECOSCORE_PROFILE_GLOBAL_PROFILE_MSG;
                    response.Code = Responsecode.Failed;
                }
                else if (result == -2)
                {
                    response.Message = entity.ReportConstants.DELETE_ECOSCORE_PROFILE_DEFAULT_PROFILE_MSG;
                    response.Code = Responsecode.Failed;
                }
                else
                {
                    response.Message = entity.ReportConstants.DELETE_ECOSCORE_PROFILE_FAIL_MSG;
                    response.Code = Responsecode.Failed;
                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }
        #endregion

        #region Eco Score Report By All Drivers

        /// <summary>
        /// Get Eco Score Report by All Drivers
        /// </summary>
        /// <param name="request"> Search Parameter object</param>
        /// <param name="context"> GRPC context</param>
        /// <returns></returns>
        public override async Task<GetEcoScoreReportByAllDriversResponse> GetEcoScoreReportByAllDrivers(GetEcoScoreReportByAllDriversRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _reportManager.GetEcoScoreReportByAllDrivers(MapEcoScoreReportByAllDriversRequest(request));
                var response = new GetEcoScoreReportByAllDriversResponse();
                if (result?.Count > 0)
                {
                    response.DriverRanking.AddRange(MapEcoScoreReportByAllDriversResponse(result));
                    response.Code = Responsecode.Success;
                    response.Message = ReportConstants.GET_REPORT_DETAILS_SUCCESS_MSG;
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = ReportConstants.GET_ECOSCORE_REPORT_NOTFOUND_MSG;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GetEcoScoreReportByAllDriversResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetEcoScoreReportByAllDrivers get failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Mapper to covert GRPC request object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private EcoScoreReportByAllDriversRequest MapEcoScoreReportByAllDriversRequest(GetEcoScoreReportByAllDriversRequest request)
        {
            var objRequest = new EcoScoreReportByAllDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                VINs = request.VINs.ToList<string>(),
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                OrgId = request.OrgId,
                AccountId = request.AccountId,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            return objRequest;
        }

        /// <summary>
        /// Mapper to covert object to  GRPC response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private IEnumerable<EcoScoreReportDriversRanking> MapEcoScoreReportByAllDriversResponse(List<EcoScoreReportByAllDrivers> response)
        {
            var lstDriverRanking = new List<EcoScoreReportDriversRanking>();
            foreach (var item in response)
            {
                var ranking = new EcoScoreReportDriversRanking
                {
                    Ranking = item.Ranking,
                    DriverName = item.DriverName ?? string.Empty,
                    DriverId = item.DriverId ?? string.Empty,
                    EcoScoreRanking = item.EcoScoreRanking,
                    EcoScoreRankingColor = item.EcoScoreRankingColor ?? string.Empty
                };
                lstDriverRanking.Add(ranking);
            }
            return lstDriverRanking;
        }

        #endregion
    }
}

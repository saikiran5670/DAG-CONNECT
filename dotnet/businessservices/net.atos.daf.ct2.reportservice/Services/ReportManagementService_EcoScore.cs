using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reports.entity;

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
                await _reportManager.CreateEcoScoreProfile(profileRequest);
                
                response.Code = Responsecode.Success;
                response.Message = "Eco-Score profile is created successfully.";

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
            dto.ActionedBy = request.AccountId;
            foreach(var profileKPI in request.ProfileKPIs)
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
    }
}

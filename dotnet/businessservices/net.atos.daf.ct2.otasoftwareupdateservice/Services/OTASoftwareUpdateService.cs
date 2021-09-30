using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.otasoftwareupdate;
using net.atos.daf.ct2.visibility;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Services
{

    public class OTASoftwareUpdateManagementService : OTASoftwareUpdateService.OTASoftwareUpdateServiceBase
    {
        private readonly ILog _logger;
        private readonly IOTASoftwareUpdateManager _otaSoftwareUpdateManagement;
        private readonly IVisibilityManager _visibilityManager;

        public OTASoftwareUpdateManagementService(IOTASoftwareUpdateManager otaSoftwareUpdateManagement
                                                  , IVisibilityManager visibilityManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _otaSoftwareUpdateManagement = otaSoftwareUpdateManagement;
            _visibilityManager = visibilityManager;
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
                                                            Type = s.Type ?? string.Empty
                                                        }));
                return await Task.FromResult(response);
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


    }
}

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.otasoftwareupdate;

namespace net.atos.daf.ct2.otasoftwareupateservice
{

    public class OTASoftwareUpdateManagementService : OTASoftwareUpdateService.OTASoftwareUpdateServiceBase
    {
        private readonly ILog _logger;
        private readonly IOTASoftwareUpdateManager _otaSoftwareUpdateManagement;

        public OTASoftwareUpdateManagementService(IOTASoftwareUpdateManager otaSoftwareUpdateManagement)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _otaSoftwareUpdateManagement = otaSoftwareUpdateManagement;
        }

        public async override Task<VehicleSoftwareStatusResponce> GetVehicleSoftwareStatus(NoRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleSoftwareStatusList = await _otaSoftwareUpdateManagement.GetVehicleSoftwareStatus();

                var response = new VehicleSoftwareStatusResponce
                {
                    Message = "Successfully fetch records for Vehicle Software Status",
                    Code = ResponceCode.Success
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
                return await Task.FromResult(new VehicleSoftwareStatusResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponceCode.InternalServerError
                });
            }
        }




    }
}

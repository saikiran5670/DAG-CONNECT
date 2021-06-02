using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        #region Driver Time management Report
        /// <summary>
        /// Fetch Multiple Drivers activity data
        /// </summary>
        /// <param name="request"> Filters for driver activity with VIN and Driver ID </param>
        /// <param name="context">GRPC Context</param>
        /// <returns>Driver activity by type column</returns>
        public override async Task<DriverActivityResponse> GetDriversActivity(ActivityFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetDriversActivity for multiple drivers.");
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter();
                objActivityFilter.VIN.Append(request.VINs);
                objActivityFilter.StartDateTime = request.StartDateTime;
                objActivityFilter.EndDateTime = request.EndDateTime;

                var result = await _reportManager.GetDriversActivity(objActivityFilter);
                DriverActivityResponse response = new DriverActivityResponse();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.DriverActivities.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverActivity>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverActivityResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriversActivity get failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Fetch Single driver activity data
        /// </summary>
        /// <param name="request"> Filters for driver activity with VIN and Driver ID </param>
        /// <param name="context">GRPC Context</param>
        /// <returns>Driver activity by type column</returns>
        public override async Task<DriverActivityResponse> GetDriverActivity(ActivityFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetDriverActivity for single driver.");
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter();
                objActivityFilter.VIN.Append(request.VINs);
                objActivityFilter.StartDateTime = request.StartDateTime;
                objActivityFilter.EndDateTime = request.EndDateTime;

                var result = await _reportManager.GetDriverActivity(objActivityFilter);
                DriverActivityResponse response = new DriverActivityResponse();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.DriverActivities.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverActivity>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverActivityResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriverActivity get failed due to - " + ex.Message
                });
            }
        }

        #endregion
    }
}

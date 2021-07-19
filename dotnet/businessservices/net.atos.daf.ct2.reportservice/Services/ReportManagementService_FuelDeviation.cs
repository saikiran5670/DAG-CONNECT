using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        #region Fuel Deviation Report Table Details 
        public override async Task<FuelDeviationResponse> GetFilteredFuelDeviation(FuelDeviationFilterRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _reportManager
                                            .GetFilteredFuelDeviation(
                                                                        new FuelDeviationFilter
                                                                        {
                                                                            StartDateTime = request.StartDateTime,
                                                                            EndDateTime = request.EndDateTime,
                                                                            VINs = request.VINs
                                                                        }
                                                                     );
                var response = new FuelDeviationResponse();
                if (result.Any())
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.FuelDeviationDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FuelDeviationDetails>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = ReportConstants.NORESULTFOUND_MSG;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FuelDeviationResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFilteredFuelDeviation get failed due to - " + ex.Message
                });
            }
        }
        #endregion

        #region Fuel Deviation Report Chartss  
        public override async Task<FuelDeviationChartResponse> GetFuelDeviationCharts(FuelDeviationFilterRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _reportManager
                                            .GetFuelDeviationCharts(
                                                                        new FuelDeviationFilter
                                                                        {
                                                                            StartDateTime = request.StartDateTime,
                                                                            EndDateTime = request.EndDateTime,
                                                                            VINs = request.VINs
                                                                        }
                                                                     );
                var response = new FuelDeviationChartResponse();
                if (result.Any())
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.FuelDeviationchart.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FuelDeviationCharts>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = ReportConstants.NORESULTFOUND_MSG;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FuelDeviationChartResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFilteredFuelDeviation get failed due to - " + ex.Message
                });
            }
            #endregion
        }
    }
}
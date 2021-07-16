using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        public override async Task<FuelBenchmarkResponse> GetFuelBenchmarkByVehicleGroup(FuelBenchmarkRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFuelBenchmarkByVehicleGroup report per Vehicle");
                ReportComponent.entity.FuelBenchmarkFilter objFleetFilter = new ReportComponent.entity.FuelBenchmarkFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFuelBenchmarkDetails(objFleetFilter);
                FuelBenchmarkResponse response = new FuelBenchmarkResponse();
                if (result != null)
                {
                    response.FuelBenchmarkDetails = _mapper.MapFuelBenchmarktoModel(result);
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
                return await Task.FromResult(new FuelBenchmarkResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFuelBenchmarkByVehicleGroup get failed due to - " + ex.Message
                });
            }
        }

        public override async Task<FuelBenchmarkResponse> GetFuelBenchmarkByTimePeriod(FuelBenchmarkRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFuelBenchmarkByTimePeriod report by time period");
                ReportComponent.entity.FuelBenchmarkFilter objFleetFilter = new ReportComponent.entity.FuelBenchmarkFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFuelBenchmarkDetails(objFleetFilter);
                FuelBenchmarkResponse response = new FuelBenchmarkResponse();
                if (result != null)
                {
                    response.FuelBenchmarkDetails = _mapper.MapFuelBenchmarktoModel(result);
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
                return await Task.FromResult(new FuelBenchmarkResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFuelBenchmarkByVehicleGroup get failed due to - " + ex.Message
                });
            }
        }
    }
}

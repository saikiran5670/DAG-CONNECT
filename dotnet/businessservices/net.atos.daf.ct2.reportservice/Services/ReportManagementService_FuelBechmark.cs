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
        //public override async Task<FuelBenchmarkResponse> GetFuelBenchmarks(FuelBenchmarkRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        ReportComponent.entity.FuelBenchmark objFuelBenchmark = new ReportComponent.entity.FuelBenchmark
        //        {
        //            StartDate = request.StartDateTime,
        //            EndDate = request.EndDateTime,
        //            VIN = request.VINs.ToList()
        //        };
        //        var result = await _reportManager.GetFuelBenchmarks(objFuelBenchmark);
        //        FuelBenchmarkResponse response = new FuelBenchmarkResponse();
        //        if (result.Any())
        //        {
        //            string res = JsonConvert.SerializeObject(result);
        //            response.FuelBenchmarkDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FuelBenchmarkDetails>>(res));
        //            response.Code = Responsecode.Success;
        //            response.Message = Responsecode.Success.ToString();
        //        }
        //        else
        //        {
        //            response.Code = Responsecode.NotFound;
        //            response.Message = ReportConstants.NORESULTFOUND_MSG;
        //        }
        //        return await Task.FromResult(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(null, ex);
        //        return await Task.FromResult(new FuelBenchmarkResponse
        //        {
        //            Code = Responsecode.Failed,
        //            Message = "Fuel benchmark details get failed due to - " + ex.Message
        //        });
        //    }

        //}

        public override async Task<FuelBenchmarkResponse> GetFuelBenchmarkByVehicleGroup(FuelBenchmarkRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetFuelDetailsByVehicle report per Vehicle");
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
                    response.FuelBenchmarkDetails = new FuelBenchmarkDetails();
                    // var vehicleRanking = await _reportRepository.GetFuelBenchmarkRanking(fuelBenchmarkFilter);
                    response.FuelBenchmarkDetails.NumberOfActiveVehicles = result.NumberOfActiveVehicles;
                    response.FuelBenchmarkDetails.NumberOfTotalVehicles = result.NumberOfTotalVehicles;
                    response.FuelBenchmarkDetails.TotalMileage = result.TotalMileage;
                    response.FuelBenchmarkDetails.TotalFuelConsumed = result.TotalFuelConsumed;
                    response.FuelBenchmarkDetails.AverageFuelConsumption = result.AverageFuelConsumption;
                    // response.FuelBenchmarkDetails.Ranking = new Ranking();

                    foreach (var item in result.Ranking)
                    {
                        Ranking objRanking = new Ranking();
                        objRanking.VIN = item.VIN;
                        objRanking.FuelConsumption = item.FuelConsumption;
                        objRanking.VehicleName = item.VehicleName;
                        response.FuelBenchmarkDetails.Ranking.Add(objRanking);
                    }

                    //  response.FuelBenchmarkDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections..MapField<FuelBenchmarkDetails>>(serialResult));
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
                return await Task.FromResult(new FuelBenchmarkResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFuelBenchmarkByVehicleGroup get failed due to - " + ex.Message
                });
            }
        }
    }
}

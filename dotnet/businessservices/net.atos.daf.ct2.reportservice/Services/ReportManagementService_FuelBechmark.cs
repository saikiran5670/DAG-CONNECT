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
                    response.Code = Responsecode.Success;
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
                _logger.Error($"{nameof(GetFuelBenchmarkByVehicleGroup)}: With Error:-", ex);
                return await Task.FromResult(new FuelBenchmarkResponse
                {
                    Code = Responsecode.Failed,
                    Message = ReportConstants.INTERNAL_SERVER_MSG
                });
            }
        }

        public override async Task<FuelBenchmarkResponse> GetFuelBenchmarkByTimePeriod(FuelBenchmarkTimePeriodRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFuelBenchmarkByTimePeriod report by time period");
                if (request.VINs.Count() == 0)
                {
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                    var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                    var vehicleDeatilsWithAccountVisibility =
                               await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                    if (vehicleDeatilsWithAccountVisibility.Count() > 0)
                    {
                        var vinList = await _reportManager
                                           .GetVinsFromTripStatistics(vehicleDeatilsWithAccountVisibility
                                                                          .Select(s => s.Vin).Distinct());
                        if (vinList.Count() > 0)
                        {
                            //var vins = vinList.Where(x => x.StartTimeStamp >= request.StartDateTime && x.EndTimeStamp <= request.EndDateTime).Select(x => x.Vin);
                            //for out of memory exception we are getting array of end time stamp.If it falls in range condition added
                            var vins = vinList.Where(x => x.EndTimeStamp.Contains(request.EndDateTime)).Select(x => x.Vin);

                            foreach (var item in vins)
                            {
                                request.VINs.Add(item);
                            }
                        }
                    }

                }
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
                    response.Code = Responsecode.Success;
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
                _logger.Error($"{nameof(GetFuelBenchmarkByTimePeriod)}: With Error:-", ex);
                return await Task.FromResult(new FuelBenchmarkResponse
                {
                    Code = Responsecode.Failed,
                    Message = ReportConstants.INTERNAL_SERVER_MSG
                });
            }
        }

        public override async Task<AssociatedVehicleResponse> GetAssociatedVehiclGroup(VehicleListRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);
                AssociatedVehicleResponse response = new AssociatedVehicleResponse();

                if (vehicleDetailsAccountVisibilty.Any())
                {

                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicle.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );
                    response.Message = Responsecode.Success.ToString();
                    response.Code = Responsecode.Success;
                }

                _logger.Info("Get method in report parameter called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAssociatedVehiclGroup)}: With Error:-", ex);
                return await Task.FromResult(new AssociatedVehicleResponse
                {
                    Code = Responsecode.Failed,
                    Message = ReportConstants.INTERNAL_SERVER_MSG
                });
            }
        }
    }
}

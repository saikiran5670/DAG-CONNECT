using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;

using ReportComponent = net.atos.daf.ct2.reports;
using ProtobufCollection = Google.Protobuf.Collections;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        /// <summary>
        /// Fetch Fleet Fuel Utilization by Vehicle data according to filter range
        /// </summary>
        /// <param name="request">Fleet Fuel report filter object</param>
        /// <param name="context"> GRPC context</param>
        /// <returns>Result will be list of Trips with average consumption group by vehicle</returns>
        public override async Task<FleetFuelDetailsResponse> GetFleetFuelDetailsByVehicle(FleetFuelFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetFuelDetailsByVehicle report per Vehicle");
                ReportComponent.entity.FleetFuelFilter objFleetFilter = new ReportComponent.entity.FleetFuelFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFleetFuelDetailsByVehicle(objFleetFilter);
                FleetFuelDetailsResponse response = new FleetFuelDetailsResponse();
                if (result?.Count > 0)
                {
                    string serialResult = JsonConvert.SerializeObject(result);
                    response.FleetFuelDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetFuelDetails>>(serialResult));
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
                return await Task.FromResult(new FleetFuelDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetFuelDetailsByVehicle get failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Fetch Fleet Fuel Utilization by Driver data according to filter range
        /// </summary>
        /// <param name="request">Fleet Fuel report filter object</param>
        /// <param name="context"> GRPC context</param>
        /// <returns>Result will be list of Trips with average consumption group by vehicle</returns>
        public override async Task<FleetFuelDetailsResponse> GetFleetFuelDetailsByDriver(FleetFuelFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetFuelDetailsByDriver report per Vehicle");
                ReportComponent.entity.FleetFuelFilter objFleetFilter = new ReportComponent.entity.FleetFuelFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFleetFuelDetailsByDriver(objFleetFilter);
                FleetFuelDetailsResponse response = new FleetFuelDetailsResponse();
                if (result?.Count > 0)
                {
                    string serialResult = JsonConvert.SerializeObject(result);
                    response.FleetFuelDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetFuelDetails>>(serialResult));
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
                return await Task.FromResult(new FleetFuelDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetFuelDetailsByDriver get failed due to - " + ex.Message
                });
            }
        }

        public override async Task<FleetFuelGraphsResponse> GetFleetFuelDetailsForVehicleGraphs(FleetFuelFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetFuelDetailsByfor graphs report per Vehicle");
                ReportComponent.entity.FleetFuelFilter objFleetFilter = new ReportComponent.entity.FleetFuelFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFleetFuelDetailsForVehicleGraphs(objFleetFilter);
                FleetFuelGraphsResponse response = new FleetFuelGraphsResponse();
                if (result?.Count > 0)
                {
                    string serialResult = JsonConvert.SerializeObject(result);
                    response.FleetfuelGraph.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetFuelGraphs>>(serialResult));
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
                throw;
            }
        }
        public override async Task<FleetFuelDetailsResponse> GetFleetFuelTripDetailsByVehicle(FleetFuelFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetFuelDetailsByVehicle report per Vehicle");
                ReportComponent.entity.FleetFuelFilter objFleetFilter = new ReportComponent.entity.FleetFuelFilter
                {
                    VINs = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFleetFuelTripDetailsByVehicle(objFleetFilter);
                FleetFuelDetailsResponse response = new FleetFuelDetailsResponse();
                if (result?.Count > 0)
                {
                    string serialResult = JsonConvert.SerializeObject(result);
                    response.FleetFuelDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetFuelDetails>>(serialResult));
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
                return await Task.FromResult(new FleetFuelDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetFuelDetailsByVehicle get failed due to - " + ex.Message
                });
            }
        }
    }
}

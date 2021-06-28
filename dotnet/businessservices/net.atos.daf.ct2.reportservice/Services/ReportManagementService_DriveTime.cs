using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using VisibleEntity = net.atos.daf.ct2.visibility.entity;
using ReportComponent = net.atos.daf.ct2.reports;
using ProtobufCollection = Google.Protobuf.Collections;

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
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter
                {
                    VIN = request.VINs.ToList<string>(),
                    DriverId = request.DriverIds.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };

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
        public override async Task<DriverActivityResponse> GetDriverActivity(SingleDriverActivityFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetDriverActivity for single driver.");
                ReportComponent.entity.DriverActivityFilter objActivityFilter = new ReportComponent.entity.DriverActivityFilter { VIN = new List<string>(), DriverId = new List<string>() };
                objActivityFilter.VIN.Add(request.VIN);
                objActivityFilter.DriverId.Add(request.DriverId);
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
        public override async Task<DriverListAndVehicleDetailsResponse> GetDriverActivityParameters(IdRequestForDriverActivity request, ServerCallContext context)
        {
            ///1. Call GetVehicleByAccountVisibility from vesibility to pull the list of VIN
            ///2. Pull the drivers details based on VIN
            ///3. Fill the DriverActivityParameters object and return it.
            DriverListAndVehicleDetailsResponse response = new DriverListAndVehicleDetailsResponse();
            try
            {
                var vehicleDeatilsWithAccountVisibility =
                                   await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() > 0)
                {
                    List<string> vinList = vehicleDeatilsWithAccountVisibility.Select(s => s.Vin).Distinct().ToList();
                    //string VINs = "'" + string.Join("','", vinList) + "'";
                    var lstDriver = await _reportManager.GetDriversByVIN(request.StartDateTime, request.EndDateTime, vinList);
                    if (lstDriver.Count() > 0)
                    {
                        string lstVehicle = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
                        response.VehicleDetailsWithAccountVisibiltyList.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleDetailsWithAccountVisibilty>>(lstVehicle));
                        string resDrivers = JsonConvert.SerializeObject(lstDriver);
                        response.DriverList.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleFromDriverTimeDetails>>(resDrivers));
                        response.Code = Responsecode.Success;
                        response.Message = Responsecode.Success.ToString();
                    }
                    else
                    {
                        VehicleFromDriverTimeDetails vehicleFromDriverTimeDetails = new VehicleFromDriverTimeDetails();
                        response.DriverList.Add(vehicleFromDriverTimeDetails);
                        string lstVehicle = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
                        response.VehicleDetailsWithAccountVisibiltyList.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleDetailsWithAccountVisibilty>>(lstVehicle));
                        //VehicleDetailsWithAccountVisibilty vehicleDetailsWithAccountVisibilty = new VehicleDetailsWithAccountVisibilty();
                        //response.VehicleDetailsWithAccountVisibiltyList.Add(vehicleDetailsWithAccountVisibilty);
                        response.Code = Responsecode.NotFound;
                        response.Message = Responsecode.NotFound.ToString();
                    }
                }
                else
                {
                    VehicleFromDriverTimeDetails vehicleFromDriverTimeDetails = new VehicleFromDriverTimeDetails();
                    response.DriverList.Add(vehicleFromDriverTimeDetails);
                    VehicleDetailsWithAccountVisibilty vehicleDetailsWithAccountVisibilty = new VehicleDetailsWithAccountVisibilty();
                    response.VehicleDetailsWithAccountVisibiltyList.Add(vehicleDetailsWithAccountVisibilty);
                    response.Code = Responsecode.NotFound;
                    response.Message = Responsecode.NotFound.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverListAndVehicleDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetDriverActivityParameters failed due to - " + ex.Message
                });
            }
            return await Task.FromResult(response);
        }

        private async Task<Tuple<ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty>, List<string>>> GetVisibleVINDetails(int accountId, int organizationId)
        {
            IEnumerable<VisibleEntity.VehicleDetailsAccountVisibilty> vehicleDeatilsWithAccountVisibility = await _visibilityManager.GetVehicleByAccountVisibility(accountId, organizationId);

            string lstVehicle = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
            ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty> lstVehiclesWithVisiblity = JsonConvert.DeserializeObject<ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty>>(lstVehicle);
            List<string> vinList = vehicleDeatilsWithAccountVisibility.Select(s => s.Vin).Distinct().ToList();
            return await Task.FromResult(new Tuple<ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty>, List<string>>(lstVehiclesWithVisiblity, vinList));
        }

        //private async Task<(ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty>, List<string>)> GetVisibleVINDetails1(int AccountId, int OrganizationId)
        //{
        //    IEnumerable<VisibleEntity.VehicleDetailsAccountVisibilty> _vehicleDeatilsWithAccountVisibility = await _visibilityManager.GetVehicleByAccountVisibility(AccountId, OrganizationId);

        //    string lstVehicle = JsonConvert.SerializeObject(_vehicleDeatilsWithAccountVisibility);
        //    ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty> lstVehiclesWithVisiblity = JsonConvert.DeserializeObject<ProtobufCollection.RepeatedField<VehicleDetailsWithAccountVisibilty>>(lstVehicle);
        //    List<string> vinList = _vehicleDeatilsWithAccountVisibility.Select(s => s.Vin).Distinct().ToList();
        //    return await Task.FromResult((lstVehiclesWithVisiblity, vinList));
        //}

        #endregion
    }
}

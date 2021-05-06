using Grpc.Core;
using log4net;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.poigeofence;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class CorridorManagementService: CorridorService.CorridorServiceBase
    {

        private ILog _logger;
        private readonly ICorridorManger _corridorManger;
        public CorridorManagementService(ICorridorManger corridorManger)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorManger = corridorManger;

        }

        public override async Task<CorridorResponseList> GetCorridorList(CorridorRequest request, ServerCallContext context)
        {
            try
            {
                CorridorResponseList objCorridorResponseList = new CorridorResponseList();
                net.atos.daf.ct2.poigeofence.entity.CorridorRequest obj = new poigeofence.entity.CorridorRequest();
                obj.OrganizationId = request.OrganizationId;
                obj.CorridorId = request.CorridorId;
                var data = await _corridorManger.GetCorridorList(obj);
                #region CorridorGridView
                foreach (var item in data.GridView)
                {
                    CorridorResponse objCorridorResponse = new CorridorResponse();
                    objCorridorResponse.Id = item.Id;
                    objCorridorResponse.OrganizationId = item.OrganizationId;
                    objCorridorResponse.CorridoreName = CheckNull(item.CorridoreName);
                    objCorridorResponse.StartPoint = CheckNull(item.StartPoint);
                    objCorridorResponse.StartLat = item.StartLat;
                    objCorridorResponse.StartLong = item.StartLong;
                    objCorridorResponse.EndPoint = CheckNull(item.EndPoint);
                    objCorridorResponse.EndLat = item.EndLat;
                    objCorridorResponse.EndLong = item.EndLong;
                    objCorridorResponse.Distance = item.Distance;
                    objCorridorResponse.Width = item.Width;
                    objCorridorResponse.CreatedAt = item.CreatedAt;
                    objCorridorResponse.CreatedBy = item.CreatedBy;
                    objCorridorResponse.ModifiedAt = item.ModifiedAt;
                    objCorridorResponse.ModifiedBy = item.ModifiedBy;
                    for (int i = 0; i < item.ViaAddressDetails.Count; i++)
                    {
                        ViaAddressDetail objViaAddressDetail = new ViaAddressDetail();
                        objViaAddressDetail.CorridorViaStopId = item.ViaAddressDetails[i].CorridorViaStopId;
                        objViaAddressDetail.CorridorViaStopName = CheckNull(item.ViaAddressDetails[i].CorridorViaStopName);
                        objViaAddressDetail.Latitude = item.ViaAddressDetails[i].Latitude;
                        objViaAddressDetail.Longitude = item.ViaAddressDetails[i].Longitude;
                        objCorridorResponse.ViaAddressDetail.Add(objViaAddressDetail);
                    }
                    objCorridorResponseList.CorridorList.Add(objCorridorResponse);
                }
                #endregion
                objCorridorResponseList.Message = "CorridorList data retrieved";
                objCorridorResponseList.Code = Responsecode.Success;
                _logger.Info("GetCorridorList method in CorridorManagement service called.");
                return await Task.FromResult(objCorridorResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CorridorResponseList
                {
                    Code = Responsecode.Failed,
                    Message = $"Exception while retrieving data from GetCorridorList : {ex.Message}"
                });
            }
        }

        string CheckNull(string value)
        {
            return string.IsNullOrEmpty(value) == true ? string.Empty : value;
        }

        public override async Task<RouteCorridorAddResponse> AddRouteCorridor(RouteCorridorAddRequest request, ServerCallContext context)
        {
            RouteCorridorAddResponse response = new RouteCorridorAddResponse();
            try
            {
                _logger.Info("Add Corridor .");
                poigeofence.entity.RouteCorridor obj = new poigeofence.entity.RouteCorridor();
                obj.OrganizationId = request.OrganizationId;
                obj.CorridorType = Convert.ToChar(request.CorridorType);
                obj.CorridorLabel = request.CorridorLabel;
                obj.StartAddress = request.StartAddress;
                obj.StartLatitude = request.StartLatitude;
                obj.StartLongitude = request.StartLongitude;
                obj.EndAddress = request.EndAddress;
                obj.EndLatitude = request.EndLatitude;
                obj.EndLongitude = request.EndLongitude;
                obj.Width = request.Width;
                obj.Distance = request.Distance;
                obj.Trailer = Convert.ToChar(request.Trailer);
                obj.TransportData = request.IsTransportData;
                obj.TrafficFlow = request.IsTrafficFlow;


                obj.Explosive = request.IsExplosive;
                obj.Gas = request.IsGas;
                obj.Flammable = request.IsFlammable;
                obj.Combustible = request.IsCombustible;
                obj.organic = request.Isorganic;
                obj.poision = request.Ispoision;
                obj.RadioActive = request.IsRadioActive;
                obj.Corrosive = request.IsCorrosive;
                obj.PoisonousInhalation = request.IsPoisonousInhalation;


                obj.WaterHarm = request.IsWaterHarm;
                obj.Other = request.IsOther;
                obj.TollRoad = Convert.ToChar(request.TollRoad);
                obj.Mortorway = Convert.ToChar(request.Mortorway);
                obj.BoatFerries = Convert.ToChar(request.BoatFerries);
                obj.RailFerries = Convert.ToChar(request.RailFerries);
                obj.Tunnels = Convert.ToChar(request.Tunnels);
                obj.DirtRoad = Convert.ToChar(request.DirtRoad);
                obj.VehicleSizeHeight = request.VehicleSizeHeight;


                obj.VehicleSizeWidth = request.VehicleSizeWidth;
                obj.VehicleSizeLength = request.VehicleSizeLength;
                obj.VehicleSizeLimitedWeight = request.VehicleSizeLimitedWeight;
                obj.VehicleSizeWeightPerAxle = request.VehicleSizeWeightPerAxle;
                obj.ViaRoutDetails = new List<poigeofence.entity.ViaRoute>();

                if (request != null && request.ViaAddressDetails != null)
                {
                    foreach (var item in request.ViaAddressDetails)
                    {
                        var trans = new poigeofence.entity.ViaRoute();
                        trans.ViaStopName = item.ViaName;
                        trans.Latitude = item.Longitude;
                        trans.Longitude = item.Longitude;
                        obj.ViaRoutDetails.Add(trans);
                       
                    }
                }

                var result = await _corridorManger.AddRouteCorridor(obj);
                if (result.Id == -1)
                {
                    response.Message = "Corridor Name is " + obj.CorridorLabel + " already exists ";
                    response.Code = Responsecode.Conflict;
                    response.CorridorID = result.Id;

                }
                else if (result != null && result.Id > 0)
                {
                    response.Message = "Added successfully";
                    response.Code = Responsecode.Success;
                    response.CorridorID = result.Id;
                }
                else
                {
                    response.Message = "Add Route Corridor Fail";
                    response.Code = Responsecode.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
            }
            return await Task.FromResult(response);
        }
    }
}

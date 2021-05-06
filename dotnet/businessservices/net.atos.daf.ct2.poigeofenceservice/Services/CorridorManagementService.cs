﻿using Grpc.Core;
using log4net;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofenceservice.entity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class CorridorManagementService : CorridorService.CorridorServiceBase
    {

        private ILog _logger;
        private readonly ICorridorManger _corridorManger;
        private readonly CorridorMapper _corridorMapper;
        public CorridorManagementService(ICorridorManger corridorManger)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorManger = corridorManger;
            _corridorMapper = new CorridorMapper();

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

                #region CorridorEditView
                if (data.EditView != null && data.EditView.Count > 0)
                {
                    foreach (var item in data.EditView)
                    {
                        CorridorEditViewResponse objCorridorEditViewResponse = new CorridorEditViewResponse();
                        objCorridorEditViewResponse.Id = item.Id;
                        objCorridorEditViewResponse.OrganizationId = item.OrganizationId;
                        objCorridorEditViewResponse.CorridoreName = CheckNull(item.CorridoreName);
                        objCorridorEditViewResponse.StartPoint = CheckNull(item.StartPoint);
                        objCorridorEditViewResponse.StartLat = item.StartLat;
                        objCorridorEditViewResponse.StartLong = item.StartLong;
                        objCorridorEditViewResponse.EndPoint = CheckNull(item.EndPoint);
                        objCorridorEditViewResponse.EndLat = item.EndLat;
                        objCorridorEditViewResponse.EndLong = item.EndLong;
                        objCorridorEditViewResponse.Distance = item.Distance;
                        objCorridorEditViewResponse.Width = item.Width;
                        objCorridorEditViewResponse.CreatedAt = item.CreatedAt;
                        objCorridorEditViewResponse.CreatedBy = item.CreatedBy;
                        objCorridorEditViewResponse.ModifiedAt = item.ModifiedAt;
                        objCorridorEditViewResponse.ModifiedBy = item.ModifiedBy;
                        for (int i = 0; i < item.ViaAddressDetails.Count; i++)
                        {
                            ViaAddressDetail objViaAddressDetail = new ViaAddressDetail();
                            objViaAddressDetail.CorridorViaStopId = item.ViaAddressDetails[i].CorridorViaStopId;
                            objViaAddressDetail.CorridorViaStopName = CheckNull(item.ViaAddressDetails[i].CorridorViaStopName);
                            objViaAddressDetail.Latitude = item.ViaAddressDetails[i].Latitude;
                            objViaAddressDetail.Longitude = item.ViaAddressDetails[i].Longitude;
                            objCorridorEditViewResponse.ViaAddressDetail.Add(objViaAddressDetail);
                        }
                        objCorridorEditViewResponse.CorridorProperties = new CorridorProperties();
                        objCorridorEditViewResponse.CorridorProperties.CorridorPropertiesId = item.CorridorPropertiesId;
                        objCorridorEditViewResponse.CorridorProperties.IsTransportData = item.IsTransportData;
                        objCorridorEditViewResponse.CorridorProperties.IsTrafficFlow = item.IsTrafficFlow;
                        objCorridorEditViewResponse.CorridorProperties.CreatedAtForCP = item.CreatedAtForCP;
                        objCorridorEditViewResponse.CorridorProperties.ModifiedAtForCP = item.ModifiedAtForCP;

                        objCorridorEditViewResponse.CorridorProperties.Attribute = new corridorservice.Attribute();
                        objCorridorEditViewResponse.CorridorProperties.Attribute.NoOfTrailers = item.NoOfTrailers;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsExplosive = item.IsExplosive;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsGas = item.IsGas;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsCombustible = item.IsCombustible;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsOrganic = item.IsOrganic;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsPoision = item.IsPoision;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsRadioActive = item.IsRadioActive;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsCorrosive = item.IsCorrosive;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsPoisonousInhalation = item.IsPoisonousInhalation;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsWaterHarm = item.IsWaterHarm;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsOther = item.IsOther;
                        objCorridorEditViewResponse.CorridorProperties.Attribute.IsFlammable = item.IsFlammable;

                        objCorridorEditViewResponse.CorridorProperties.Exclusion = new Exclusion();
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.TollRoadType = CheckNull(item.TollRoadType);
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.Mortorway = CheckNull(item.Mortorway);
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.BoatFerriesType = CheckNull(item.BoatFerriesType);
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.RailFerriesType = CheckNull(item.RailFerriesType);
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.TunnelsType = CheckNull(item.TunnelsType);
                        objCorridorEditViewResponse.CorridorProperties.Exclusion.DirtRoadType = CheckNull(item.DirtRoadType);

                        objCorridorEditViewResponse.CorridorProperties.VehicleSize = new VehicleSize();
                        objCorridorEditViewResponse.CorridorProperties.VehicleSize.VehicleHeight = item.VehicleHeight;
                        objCorridorEditViewResponse.CorridorProperties.VehicleSize.VehicleWidth = item.VehicleWidth;
                        objCorridorEditViewResponse.CorridorProperties.VehicleSize.VehicleLength = item.VehicleLength;
                        objCorridorEditViewResponse.CorridorProperties.VehicleSize.VehicleLimitedWeight = item.VehicleLimitedWeight;
                        objCorridorEditViewResponse.CorridorProperties.VehicleSize.VehicleWeightPerAxle = item.VehicleWeightPerAxle;

                        objCorridorResponseList.CorridorEditViewList.Add(objCorridorEditViewResponse);
                    }
                }
                #endregion

                #region CorridorGridView
                else if (data.GridView != null && data.GridView.Count > 0)
                {
                    foreach (var item in data.GridView)
                    {
                        CorridorGridViewResponse objCorridorGridViewResponse = new CorridorGridViewResponse();
                        objCorridorGridViewResponse.Id = item.Id;
                        objCorridorGridViewResponse.OrganizationId = item.OrganizationId;
                        objCorridorGridViewResponse.CorridoreName = CheckNull(item.CorridoreName);
                        objCorridorGridViewResponse.StartPoint = CheckNull(item.StartPoint);
                        objCorridorGridViewResponse.StartLat = item.StartLat;
                        objCorridorGridViewResponse.StartLong = item.StartLong;
                        objCorridorGridViewResponse.EndPoint = CheckNull(item.EndPoint);
                        objCorridorGridViewResponse.EndLat = item.EndLat;
                        objCorridorGridViewResponse.EndLong = item.EndLong;
                        objCorridorGridViewResponse.Distance = item.Distance;
                        objCorridorGridViewResponse.Width = item.Width;
                        objCorridorGridViewResponse.CreatedAt = item.CreatedAt;
                        objCorridorGridViewResponse.CreatedBy = item.CreatedBy;
                        objCorridorGridViewResponse.ModifiedAt = item.ModifiedAt;
                        objCorridorGridViewResponse.ModifiedBy = item.ModifiedBy;
                        for (int i = 0; i < item.ViaAddressDetails.Count; i++)
                        {
                            ViaAddressDetail objViaAddressDetail = new ViaAddressDetail();
                            objViaAddressDetail.CorridorViaStopId = item.ViaAddressDetails[i].CorridorViaStopId;
                            objViaAddressDetail.CorridorViaStopName = CheckNull(item.ViaAddressDetails[i].CorridorViaStopName);
                            objViaAddressDetail.Latitude = item.ViaAddressDetails[i].Latitude;
                            objViaAddressDetail.Longitude = item.ViaAddressDetails[i].Longitude;
                            objCorridorGridViewResponse.ViaAddressDetail.Add(objViaAddressDetail);
                        }
                        objCorridorResponseList.CorridorGridViewList.Add(objCorridorGridViewResponse);
                    }
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
        public override async Task<DeleteCorridorResponse> DeleteCorridor(DeleteCorridorRequest request, ServerCallContext context)
        {
            DeleteCorridorResponse response = new DeleteCorridorResponse();
            try
            {
                _logger.Info("Delete Corridor .");


                var result = await _corridorManger.DeleteCorridor(request.CorridorID);
                if (result.Id >= 0)
                {
                    response.Message = "Delete successfully";
                    response.Code = Responsecode.Success;
                    response.CorridorID = request.CorridorID;

                }
                else if (result.Id == -1)
                {
                    response.Message = "You can not delete the corridor, it is associated with alert ";
                    response.Code = Responsecode.Failed;
                }
                else
                {
                    response.Message = "Corridor Not found";
                    response.Code = Responsecode.NotFound;
                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }


        public override async Task<ExistingTripCorridorResponse> AddExistingTripCorridor(ExistingTripCorridorRequest request, ServerCallContext context)
        {
            var response = new ExistingTripCorridorResponse();
            try
            {
                _logger.Info("Add Corridor .");
               var existingTripEntity= _corridorMapper.ToExistingTripCorridorEntity(request);
              

                var result = await _corridorManger.AddExistingTripCorridor(existingTripEntity);
                if (result.Id == -1)
                {
                    response.Message = "Corridor Name is " + existingTripEntity.CorridorLabel + " already exists ";
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
                    response.Message = "Add Existing Trip Corridor Fail";
                    response.Code = Responsecode.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }

    }
}
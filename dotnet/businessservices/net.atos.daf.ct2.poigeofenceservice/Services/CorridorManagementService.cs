using Grpc.Core;
using log4net;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.poigeofence;
using System;
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
                foreach (var item in data)
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
                    objCorridorResponseList.CorridorList.Add(objCorridorResponse);
                }
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
                obj.Trailer = Convert.ToChar(request.Trailer);
                obj.TransportData = request.TransportData;
                obj.TrafficFlow = request.TrafficFlow;


                obj.Explosive = request.Explosive;
                obj.Gas = request.Gas;
                obj.Flammable = request.Flammable;
                obj.Combustible = request.Combustible;
                obj.organic = request.Organic;
                obj.poision = request.Poision;
                obj.RadioActive = request.RadioActive;
                obj.Corrosive = request.Corrosive;
                obj.PoisonousInhalation = request.PoisonousInhalation;


                obj.WaterHarm = request.WaterHarm;
                obj.Other = request.Other;
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

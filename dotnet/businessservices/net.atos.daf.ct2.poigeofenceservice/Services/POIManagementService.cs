using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofenceservice.entity;
using net.atos.daf.ct2.poiservice;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class POIManagementService : POIService.POIServiceBase
    {
        private ILog _logger;
        private readonly IPoiManager _poiManager;
        private readonly Mapper _mapper;
        public POIManagementService(IPoiManager poiManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiManager = poiManager;
            _mapper = new Mapper();
        }

        public override async Task<POIEntityResponseList> GetAllGobalPOI(net.atos.daf.ct2.poiservice.POIEntityRequest request, ServerCallContext context)
        {
            try
            {
                POIEntityResponseList objPOIEntityResponseList = new POIEntityResponseList();
                net.atos.daf.ct2.poigeofence.entity.POIEntityRequest obj = new poigeofence.entity.POIEntityRequest();
                obj.CategoryId = request.CategoryId;
                obj.SubCategoryId = request.SubCategoryId;
                var data = await _poiManager.GetAllGobalPOI(obj);
                foreach (var item in data)
                {
                    net.atos.daf.ct2.poiservice.POIEntityResponse objPOIEntityResponse = new net.atos.daf.ct2.poiservice.POIEntityResponse();
                    objPOIEntityResponse.Category = item.Category == null ? string.Empty : item.Category;
                    objPOIEntityResponse.City = item.City == null ? string.Empty : item.City;
                    objPOIEntityResponse.Latitude = item.Latitude;
                    objPOIEntityResponse.Longitude = item.Longitude;
                    objPOIEntityResponse.POIName = item.POIName == null ? string.Empty : item.POIName;
                    objPOIEntityResponseList.POIList.Add(objPOIEntityResponse);
                }
                objPOIEntityResponseList.Message = "GlobalPOI data retrieved";
                objPOIEntityResponseList.Code = Responsecode.Success;
                _logger.Info("GetAllGobalPOI method in POIManagement service called.");
                return await Task.FromResult(objPOIEntityResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIEntityResponseList
                {
                    Code = Responsecode.Failed,
                    Message = $"Exception while retrieving data from GetAllGobalPOI : {ex.Message}"
                });
            }
        }
        public override async Task<POIResponseList> GetAllPOI(POIRequest request, ServerCallContext context)
        {
            try
            {
                POIResponseList objPOIEntityResponseList = new POIResponseList();
                POIResponse objPOIEntityResponse = new POIResponse();
                POI obj = new POI();
                obj.Type = "POI";
                obj.OrganizationId = request.OrganizationId;
                var result = await _poiManager.GetAllPOI(obj);
                _logger.Info("GetAllPOI method in POI service called.");
                //foreach (var item in result)
                //{
                //    objPOIEntityResponse.Category = item.category == null ? string.Empty : item.category;
                //    objPOIEntityResponse.City = item.City == null ? string.Empty : item.city;
                //    objPOIEntityResponse.Latitude = item.latitude;
                //    objPOIEntityResponse.Longitude = item.longitude;
                //    objPOIEntityResponse.PoiName = item.poiName == null ? string.Empty : item.poiName;
                //    objPOIEntityResponseList.POIList.Add(objPOIEntityResponse);
                //}
                return objPOIEntityResponseList;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw ex;
            }
        }
        public override async Task<POIResponse> CreatePOI(POIRequest request, ServerCallContext context)
        {
            POIResponse response = new POIResponse();
            try
            {
                _logger.Info("Create POI.");
                POI poi = new POI();
                poi = _mapper.ToPOIEntity(request);
                poi = await _poiManager.CreatePOI(poi);
                if (poi.Id > 0)
                {
                    return await Task.FromResult(new POIResponse
                    {
                        Message = "POI is created with id:- " + poi.Id,
                        Code = Responsecode.Success
                    });
                }
                else
                {
                    return await Task.FromResult(new POIResponse
                    {
                        Message = "POI Creation Failed",
                        Code = Responsecode.Failed
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIResponse
                {
                    Code = Responsecode.Failed,
                    Message = "POI Creation Failed due to - " + ex.Message,
                });
            }
        }
        public override async Task<POIResponse> UpdatePOI(POIRequest request, ServerCallContext context)
        {
            POIResponse response = new POIResponse();
            try
            {
                _logger.Info("Update POI.");
                POI poi = new POI();
                poi = _mapper.ToPOIEntity(request);
                bool result = await _poiManager.UpdatePOI(poi);
                if (result)
                {
                    response.Message = "POI updated for id:- " + poi.Id;
                    response.Code = Responsecode.Success;
                }
                else
                {
                    response.Message = "POI is not created";
                    response.Code = Responsecode.Failed;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIResponse
                {
                    Code = Responsecode.Failed,
                    Message = "POI Creation Failed due to - " + ex.Message,
                });
            }
            return response;
        }
        public override async Task<POIResponse> DeletePOI(POIRequest request, ServerCallContext context)
        {
            POIResponse response = new POIResponse();
            try
            {
                _logger.Info("Delete POI.");
                
                bool result = await _poiManager.DeletePOI(request.Id);
                if (result)
                {
                    response.Message = "Deleted";
                    response.Code = Responsecode.Success;
                }
                else
                {
                    response.Message = "Not Deleted";
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

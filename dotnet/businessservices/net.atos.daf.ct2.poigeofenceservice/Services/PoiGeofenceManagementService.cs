using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofenceservice.entity;


namespace net.atos.daf.ct2.poigeofenceservice
{
    public class PoiGeofenceManagementService:PoiGeofenceService.PoiGeofenceServiceBase
    {
        private ILog _logger;
        private readonly IPoiManager _poiManager;
        private readonly IGeofenceManager geofenceManager;
        private readonly Mapper _mapper;
        public PoiGeofenceManagementService(IPoiManager poiManager,IGeofenceManager _geofenceManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiManager = poiManager;
            geofenceManager = _geofenceManager;
            _mapper = new Mapper();
        }

        public override async Task<POIEntityResponceList> GetAllPOI(POIEntityRequest request, ServerCallContext context)
        {
            try
            {
                POIEntityResponceList objPOIEntityResponceList = new POIEntityResponceList();
                POIEntityResponce objPOIEntityResponce = new POIEntityResponce();
                net.atos.daf.ct2.poigeofence.entity.POIEntityRequest obj = new poigeofence.entity.POIEntityRequest();
                obj.category_id = request.CategoryId;
                obj.organization_id = request.OrganizationId;
                obj.roleIdlevel = request.RoleIdlevel;
                obj.sub_category_id = request.SubCategoryId;
                var data = await _poiManager.GetAllPOI(obj);
                _logger.Info("GetAllPOI method in POI service called.");
                foreach (var item in data)
                {
                    objPOIEntityResponce.Category = item.category == null? string.Empty:item.category;
                    objPOIEntityResponce.City = item.city == null ? string.Empty : item.city;
                    objPOIEntityResponce.Latitude = item.latitude;
                    objPOIEntityResponce.Longitude = item.longitude;
                    objPOIEntityResponce.PoiName = item.poiName == null ? string.Empty :item.poiName;
                    objPOIEntityResponceList.POIList.Add(objPOIEntityResponce);
                }
                return objPOIEntityResponceList;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw ex;
            }
        }
        public override async Task<GeofenceDeleteResponse> DeleteGeofence(DeleteRequest request, ServerCallContext context)
        {
            GeofenceDeleteResponse response = new GeofenceDeleteResponse();
            try
            {
                _logger.Info("Delete Geofence .");
                List<int> lstGeofenceId = new List<int>();
                foreach (var item in request.GeofenceId)
                {
                    lstGeofenceId.Add(item);
                }
                bool result = await geofenceManager.DeleteGeofence(lstGeofenceId, request.OrganizationId);
                if (result)
                {
                    response.Message = "Deleted";
                    response.Code = Responcecode.Success;
                }
                if (!result)
                {
                    response.Message = "Not Deleted";
                    response.Code = Responcecode.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
            }
            return await Task.FromResult(response);
        }
        public override async Task<GeofenceEntityResponceList> GetAllGeofence(GeofenceEntityRequest request, ServerCallContext context)
        {
            GeofenceEntityResponceList response = new GeofenceEntityResponceList();
            try
            {
                _logger.Info("Get Geofence .");
                net.atos.daf.ct2.poigeofence.entity.GeofenceEntityRequest objGeofenceRequest = new poigeofence.entity.GeofenceEntityRequest();
                objGeofenceRequest.organization_id = request.OrganizationId;
                objGeofenceRequest.category_id = request.CategoryId;
                objGeofenceRequest.sub_category_id = request.SubCategoryId;
                var result = await geofenceManager.GetAllGeofence(objGeofenceRequest);
                foreach (net.atos.daf.ct2.poigeofence.entity.GeofenceEntityResponce entity in result)
                {
                    response.GeofenceList.Add(_mapper.ToGeofenceList(entity));
                }
                response.Code = Responcecode.Success;                
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }
    }
}

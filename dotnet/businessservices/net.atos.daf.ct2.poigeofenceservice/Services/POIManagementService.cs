using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
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
                net.atos.daf.ct2.poiservice.POIEntityResponse objPOIEntityResponse = new net.atos.daf.ct2.poiservice.POIEntityResponse();
                net.atos.daf.ct2.poigeofence.entity.POIEntityRequest obj = new poigeofence.entity.POIEntityRequest();
                obj.CategoryId = request.CategoryId;
                obj.SubCategoryId = request.SubCategoryId;
                var data = await _poiManager.GetAllGobalPOI(obj);
                _logger.Info("GetAllPOI method in POI service called.");
                foreach (var item in data)
                {
                    objPOIEntityResponse.Category = item.Category == null ? string.Empty : item.Category;
                    objPOIEntityResponse.City = item.City == null ? string.Empty : item.City;
                    objPOIEntityResponse.Latitude = item.Latitude;
                    objPOIEntityResponse.Longitude = item.Longitude;
                    objPOIEntityResponse.POIName = item.POIName == null ? string.Empty : item.POIName;
                    objPOIEntityResponseList.POIList.Add(objPOIEntityResponse);
                }
                objPOIEntityResponseList.Message = "POI data retrieved";
                objPOIEntityResponseList.Code = Responsecode.Success;
                _logger.Info("GetAllGobalPOI method in POI service called.");
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
    }
}

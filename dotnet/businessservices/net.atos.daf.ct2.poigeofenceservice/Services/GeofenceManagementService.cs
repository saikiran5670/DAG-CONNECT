using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.poigeofenceservice.entity;


namespace net.atos.daf.ct2.geofenceservice
{
    public class GeofenceManagementService : GeofenceService.GeofenceServiceBase
    {
        private ILog _logger;
        private readonly IGeofenceManager _geofenceManager;
        private readonly Mapper _mapper;
        public GeofenceManagementService(IGeofenceManager geofenceManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _geofenceManager = geofenceManager;
            _mapper = new Mapper();
        }

        #region Geofence

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
                bool result = await _geofenceManager.DeleteGeofence(lstGeofenceId, request.OrganizationId);
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

        public override async Task<GeofenceResponse> CreatePolygonGeofence(GeofenceRequest request, ServerCallContext context)
        {
            GeofenceResponse response = new GeofenceResponse();
            try
            {
                _logger.Info("Create Geofence.");
                Geofence geofence = new Geofence();
                geofence = _mapper.ToGeofenceEntity(request);
                geofence = await _geofenceManager.CreatePolygonGeofence(geofence);
                return await Task.FromResult(new GeofenceResponse
                {
                    Message = "Geofence created with id:- " + geofence.Id,
                    Code = Responcecode.Success,
                    GeofenceRequest = _mapper.ToGeofenceRequest(geofence)
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GeofenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Geofence Creation Faile due to - " + ex.Message,
                });
            }
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
                var result = await _geofenceManager.GetAllGeofence(objGeofenceRequest);
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

        public override async Task<CircularGeofenceResponse> CreateCircularGeofence(CircularGeofenceRequest request, ServerCallContext context)
        {
            CircularGeofenceResponse response = new CircularGeofenceResponse();
            try
            {
                _logger.Info("Create Geofence.");
                List<Geofence> geofence = new List<Geofence>();
                foreach (GeofenceRequest item in request.GeofenceRequest)
                {
                    geofence.Add(_mapper.ToGeofenceEntity(item));
                }
                geofence = await _geofenceManager.CreateCircularGeofence(geofence);
                foreach (var item in geofence)
                {
                    response.GeofenceRequest.Add(_mapper.ToGeofenceRequest(item));
                }
                response.Message = "Circular Geofence created with selected POI";
                response.Code = Responcecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CircularGeofenceResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Circular Geofence Creation Failed due to - " + ex.Message,
                });
            }
        }

        #endregion
    }
}

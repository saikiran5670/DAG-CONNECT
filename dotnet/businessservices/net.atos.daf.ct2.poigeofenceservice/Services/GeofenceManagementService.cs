using System;
using System.Collections.Generic;
using System.Linq;
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
                GeofenceDeleteEntity objGeofenceDeleteEntity = new GeofenceDeleteEntity();
                objGeofenceDeleteEntity.OrganizationId = request.OrganizationId;
                objGeofenceDeleteEntity.GeofenceId = lstGeofenceId;
                bool result = await _geofenceManager.DeleteGeofence(objGeofenceDeleteEntity);
                if (result)
                {
                    response.Message = "Deleted";
                    response.Code = Responsecode.Success;
                }
                if (!result)
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
        public override async Task<GeofenceResponse> CreatePolygonGeofence(GeofenceRequest request, ServerCallContext context)
        {
            GeofenceResponse response = new GeofenceResponse();
            try
            {
                _logger.Info("Create Geofence.");
                Geofence geofence = new Geofence();
                response.GeofenceRequest = new GeofenceRequest();
                geofence = _mapper.ToGeofenceEntity(request);
                geofence = await _geofenceManager.CreatePolygonGeofence(geofence);
                // check for exists
                response.GeofenceRequest.Exists = false;
                if (geofence.Exists)
                {
                    response.GeofenceRequest.Exists = true;
                    response.Message = "Duplicate Geofence Name";
                    response.Code = Responsecode.Conflict;
                    return response;
                }
                if (geofence == null)
                {
                    response.Message = "Geofence Response is null";
                    response.Code = Responsecode.NotFound;
                    return response;
                }
                return await Task.FromResult(new GeofenceResponse
                {
                    Message = "Geofence created with id:- " + geofence.Id,
                    Code = Responsecode.Success,
                    GeofenceRequest = _mapper.ToGeofenceRequest(geofence)
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GeofenceResponse
                {
                    Code = Responsecode.Failed,
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
                if (result != null)
                {
                    foreach (net.atos.daf.ct2.poigeofence.entity.GeofenceEntityResponce entity in result)
                    {
                        response.GeofenceList.Add(_mapper.ToGeofenceList(entity));
                    }
                }
                response.Code = Responsecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }
        public override async Task<GetGeofenceResponse> GetGeofenceByGeofenceID(IdRequest request, ServerCallContext context)
        {
            GetGeofenceResponse response = new GetGeofenceResponse();
            try
            {
                _logger.Info("Get GetGeofenceByGeofenceID .");
                var result = await _geofenceManager.GetGeofenceByGeofenceID(request.OrganizationId, request.GeofenceId);
                foreach (net.atos.daf.ct2.poigeofence.entity.Geofence entity in result)
                {
                    response.GeofenceName = entity.Name;
                    response.Id = entity.Id;
                    response.OrganizationId = entity.OrganizationId;
                    if (entity.CategoryName != null)
                    {
                        response.CategoryName = entity.CategoryName;
                    }
                    if (entity.SubCategoryName != null)
                    {
                        response.SubCategoryName = entity.SubCategoryName;
                    }
                    response.Address = entity.Address;
                    response.City = entity.City;
                    response.Country = entity.Country;
                    response.State = entity.State;
                    response.Distance = entity.Distance;
                    response.Latitude = entity.Latitude;
                    response.Longitude = entity.Longitude;
                    response.ModifiedAt = entity.ModifiedAt;
                    response.ModifiedBy = entity.ModifiedBy;
                    response.CreatedAt = entity.CreatedAt;
                    response.CreatedBy = entity.CreatedBy;
                    response.Zipcode = entity.Zipcode;
                }
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
                if (geofence[0].Exists)
                {
                    response.Message = "Duplicate Geofence Name";
                    response.Code = Responsecode.Conflict;
                    return response;
                }

                foreach (var item in geofence)
                {
                    response.GeofenceRequest.Add(_mapper.ToGeofenceRequest(item));
                }
                response.Message = "Circular Geofence created with selected POI";
                response.Code = Responsecode.Success;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CircularGeofenceResponse
                {
                    Code = Responsecode.Failed,
                    Message = "Circular Geofence Creation Failed due to - " + ex.Message,
                });
            }
        }

        public override async Task<GeofencePolygonUpdateResponce> UpdatePolygonGeofence(GeofencePolygonUpdateRequest request, ServerCallContext context)
        {
            GeofencePolygonUpdateResponce response = new GeofencePolygonUpdateResponce();
            try
            {
                _logger.Info("Update Geofence.");
                Geofence geofence = new Geofence();
                response.GeofencePolygonUpdateRequest = new GeofencePolygonUpdateRequest();
                geofence = _mapper.ToGeofenceUpdateEntity(request);
                geofence = await _geofenceManager.UpdatePolygonGeofence(geofence);
                // check for exists
                response.GeofencePolygonUpdateRequest.Exists = false;
                if (geofence.Exists)
                {
                    response.GeofencePolygonUpdateRequest.Exists = true;
                    response.Message = "Duplicate Geofence Name";
                    response.Code = Responsecode.Conflict;
                    return response;
                }
                if (geofence == null)
                {
                    response.Message = "Geofence Response is null";
                    response.Code = Responsecode.NotFound;
                    return response;
                }
                return await Task.FromResult(new GeofencePolygonUpdateResponce
                {
                    Message = "Geofence created with id:- " + geofence.Id,
                    Code = Responsecode.Success,
                    GeofencePolygonUpdateRequest = _mapper.ToGeofenceUpdateRequest(geofence)
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GeofencePolygonUpdateResponce
                {
                    Code = Responsecode.Failed,
                    Message = "Geofence Creation Failed due to - " + ex.Message,
                });
            }
        }

        public override async Task<GeofenceResponse> BulkImportGeofence(BulkGeofenceRequest requests, ServerCallContext context)
        {
            try
            {
                var geofence = new List<Geofence>();
                foreach (var item in requests.GeofenceRequest)
                    geofence.Add(_mapper.ToGeofenceEntity(item));

                var geofenceList = await _geofenceManager.BulkImportGeofence(geofence);
                var failCount = geofenceList.Where(w => w.IsFailed).Count();
                return await Task.FromResult(new GeofenceResponse
                {
                    Code = Responsecode.Success,
                    Message = failCount > 0 ? $"Bulk Geofence imported with failed count : {failCount}."
                                                                                : $"Bulk Geofence imported successfuly.",
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw ex;                
            }
        }

        public override async Task<GeofenceCircularUpdateResponce> UpdateCircularGeofence(GeofenceCircularUpdateRequest request, ServerCallContext context)
        {
            GeofenceCircularUpdateResponce response = new GeofenceCircularUpdateResponce();
            try
            {
                _logger.Info("Update Geofence.");
                Geofence geofence = new Geofence();
                response.GeofenceCircularUpdateRequest = new GeofenceCircularUpdateRequest();
                geofence = _mapper.ToGeofenceUpdateEntity(request);
                geofence = await _geofenceManager.UpdateCircularGeofence(geofence);
                // check for exists
                response.GeofenceCircularUpdateRequest.Exists = false;
                if (geofence.Exists)
                {
                    response.GeofenceCircularUpdateRequest.Exists = true;
                    response.Message = "Duplicate Geofence Name";
                    response.Code = Responsecode.Conflict;
                    return response;
                }
                if (geofence == null)
                {
                    response.Message = "Geofence Response is null";
                    response.Code = Responsecode.NotFound;
                    return response;
                }
                return await Task.FromResult(new GeofenceCircularUpdateResponce
                {
                    Message = "Geofence created with id:- " + geofence.Id,
                    Code = Responsecode.Success,
                    GeofenceCircularUpdateRequest = _mapper.ToCircularGeofenceUpdateRequest(geofence)
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GeofenceCircularUpdateResponce
                {
                    Code = Responsecode.Failed,
                    Message = "Geofence Creation Failed due to - " + ex.Message,
                });
            }
        }

        #endregion
    }
}

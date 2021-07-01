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
        private readonly ILog _logger;
        private readonly IPoiManager _poiManager;
        private readonly Mapper _mapper;
        public POIManagementService(IPoiManager poiManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _poiManager = poiManager;
            _mapper = new Mapper();
        }

        public override async Task<POIResponseList> GetAllGobalPOI(net.atos.daf.ct2.poiservice.POIEntityRequest request, ServerCallContext context)
        {
            try
            {
                POIResponseList objPOIResponseList = new POIResponseList();
                net.atos.daf.ct2.poigeofence.entity.POIEntityRequest obj = new poigeofence.entity.POIEntityRequest();
                obj.CategoryId = request.CategoryId;
                obj.SubCategoryId = request.SubCategoryId != null ? Convert.ToInt32(request.SubCategoryId) : 0;
                var data = await _poiManager.GetAllGobalPOI(obj);
                foreach (var item in data)
                {
                    net.atos.daf.ct2.poiservice.POIData objPOI = new net.atos.daf.ct2.poiservice.POIData();
                    objPOI.Id = item.Id;
                    //objPOI.OrganizationId = item.OrganizationId;
                    //objPOI.CategoryId = item.CategoryId;
                    //objPOI.SubCategoryId = item.SubCategoryId;
                    objPOI.Name = item.Name;// == null ? string.Empty : item.Name;
                    objPOI.Address = item.Address;// == null ? string.Empty : item.Name;
                    objPOI.City = item.City ?? string.Empty;
                    objPOI.CategoryName = item.CategoryName ?? string.Empty;
                    //objPOI.Country = item.Country;
                    //objPOI.Zipcode = item.Zipcode;
                    objPOI.Latitude = item.Latitude;
                    objPOI.Longitude = item.Longitude;
                    //objPOI.Distance = item.Distance;
                    //objPOI.State = item.State;
                    //objPOI.CreatedAt = item.CreatedAt;
                    //objPOI.CreatedBy = item.CreatedBy;
                    objPOIResponseList.POIList.Add(objPOI);
                }
                objPOIResponseList.Message = "GlobalPOI data retrieved";
                objPOIResponseList.Code = Responsecode.Success;
                _logger.Info("GetAllGobalPOI method in POIManagement service called.");
                return await Task.FromResult(objPOIResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIResponseList
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
                _logger.Info("GetAllPOI method in POIManagement service called.");
                POIResponseList objPOIResponseList = new POIResponseList();
                POI poi = new POI();
                //obj.OrganizationId = request.OrganizationId;
                //obj.State = "NONE";// if none then Active & inactive poi will be fetch
                //obj.Type = "POI";
                poi = _mapper.ToPOIEntity(request);
                var result = await _poiManager.GetAllPOI(poi);
                foreach (var item in result)
                {
                    objPOIResponseList.POIList.Add(_mapper.ToPOIResponseData(item));
                }
                objPOIResponseList.Message = "Succeed";
                objPOIResponseList.Code = Responsecode.Success;
                return await Task.FromResult(objPOIResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }
        public override async Task<POIResponse> CreatePOI(POIRequest request, ServerCallContext context)
        {
            POIResponse response = new POIResponse();
            try
            {
                _logger.Info("Create POI.");
                POI poi = new POI();
                request.Type = "POI";
                request.State = "Active";
                poi = _mapper.ToPOIEntity(request);
                poi = await _poiManager.CreatePOI(poi);
                if (poi.Id > 0)
                {
                    return await Task.FromResult(new POIResponse
                    {
                        POIData = _mapper.ToPOIResponseData(poi),
                        Message = "POI is created with id:- " + poi.Id,
                        Code = Responsecode.Success,
                    });
                }
                else if (poi.Id == -1)
                {
                    return await Task.FromResult(new POIResponse
                    {
                        Message = "Duplicate POI name " + poi.Name,
                        Code = Responsecode.Conflict
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
                request.Type = "POI";
                poi = _mapper.ToPOIEntity(request);
                poi = await _poiManager.UpdatePOI(poi);
                if (poi.Id > 0)
                {
                    response.POIData = _mapper.ToPOIResponseData(poi);
                    response.Message = "POI updated for id:- " + poi.Id;
                    response.Code = Responsecode.Success;
                }
                else if (poi.Id == -1)
                {
                    return await Task.FromResult(new POIResponse
                    {
                        Message = "Duplicate POI name " + poi.Name,
                        Code = Responsecode.Conflict
                    });
                }
                else
                {
                    response.Message = "POI is not Updated";
                    response.Code = Responsecode.Failed;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIResponse
                {
                    Code = Responsecode.Failed,
                    Message = "POI Updation Failed due to - " + ex.Message,
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
        public override async Task<POIResponse> DeletePOIBulk(POIDeleteBulkRequest request, ServerCallContext context)
        {
            POIResponse response = new POIResponse();
            try
            {
                _logger.Info("Delete POI.");

                List<int> poiIds = new List<int>();
                foreach (var item in request.Id)
                {
                    poiIds.Add(item);
                }
                bool result = await _poiManager.DeletePOI(poiIds);
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

        public override async Task<POIResponseList> DownloadPOIForExcel(DownloadPOIRequest request, ServerCallContext context)
        {
            try
            {
                POIResponseList objPOIResponseList = new POIResponseList();
                POI obj = new POI();
                obj.OrganizationId = request.OrganizationId;
                var result = await _poiManager.GetAllPOI(obj);
                foreach (var item in result)
                {
                    POIData objPOIData = new POIData();
                    objPOIData.Id = item.Id;
                    objPOIData.Name = item.Name ?? string.Empty;
                    objPOIData.Latitude = item.Latitude;
                    objPOIData.Longitude = item.Longitude;
                    objPOIData.CategoryName = item.CategoryName ?? string.Empty;
                    objPOIData.SubCategoryName = item.SubCategoryName ?? string.Empty;
                    objPOIData.Address = item.Address ?? string.Empty;
                    objPOIData.Zipcode = item.Zipcode ?? string.Empty;
                    objPOIData.City = item.City ?? string.Empty;
                    objPOIData.Country = item.Country ?? string.Empty;
                    objPOIResponseList.POIList.Add(objPOIData);
                }
                objPOIResponseList.Message = "POI data for Excel retrieved";
                objPOIResponseList.Code = Responsecode.Success;
                _logger.Info("DownloadPOIForExcel method in POIManagement service called.");
                return await Task.FromResult(objPOIResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }


        public override async Task<POIUploadResponse> UploadPOIExcel(POIUploadRequest request, ServerCallContext context)
        {

            try
            {
                var response = new POIUploadResponse();
                var poiList = new List<POI>();
                var uploadPoiData = _mapper.ToUploadPOIRequest(request);
                var packageUploaded = await _poiManager.UploadPOI(uploadPoiData);
                response = _mapper.ToPOIUploadResponseData(packageUploaded);
                response.POIExcelList.Add(request.POIList);
                response.Code = Responsecode.Success;
                response.Message = "Poi Uploaded successfully.";
                _logger.Info("UploadPOIExcel method in POIManagement service called.");
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new POIUploadResponse
                {
                    Code = Responsecode.Failed,
                    Message = "POI Creation Failed due to - " + ex.Message,
                });
            }
        }

        public override async Task<TripResponce> GetAllTripDetails(TripRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetAllTripDetails.");
                TripResponce response = new TripResponce();
                TripEntityRequest objTripEntityRequest = new TripEntityRequest();
                objTripEntityRequest.VIN = request.VIN;
                objTripEntityRequest.StartDateTime = request.StartDateTime;
                objTripEntityRequest.EndDateTime = request.EndDateTime;

                var result = await _poiManager.GetAllTripDetails(objTripEntityRequest);
                if (result.Count > 0)
                {
                    foreach (net.atos.daf.ct2.poigeofence.entity.TripEntityResponce entity in result)
                    {
                        response.TripData.Add(_mapper.ToTripResponce(entity));
                    }
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new TripResponce
                {
                    Code = Responsecode.Failed,
                    Message = "GetAllTripDetails get faile due to - " + ex.Message
                });
            }
        }

        public override async Task<AddTripAddressResponse> UpdateTripAddress(AddTripAddressRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Update tripdetail.trip_statistics start position and end position.");
                AddTripAddressResponse response = new AddTripAddressResponse();
                var tripDetails = new TripAddressDetails() { Id = request.Id, StartAddress = request.StartAddress, EndAddress = request.EndAddress };
                response.TripAddressDetails = new AddTripAddressRequest();
                var result = await _poiManager.UpdateTripArddress(tripDetails);
                response.TripAddressDetails.Id = result.Id;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AddTripAddressResponse
                {
                    Responsecode = Responsecode.Failed,
                    Message = "Update tripdetail.trip_statistics start position and end position get faile due to - " + ex.Message
                });
            }
        }


    }
}

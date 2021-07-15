using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.poigeofence;
//using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poiservice;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class Mapper
    {
        public net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce ToGeofenceList(net.atos.daf.ct2.poigeofence.entity.GeofenceEntityResponce request)
        {
            net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce objResponse = new net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce();
            if (request.Category != null)
            {
                objResponse.CategoryName = request.Category;
            }
            if (request.SubCategory != null)
            {
                objResponse.SubCategoryName = request.SubCategory;
            }
            if (request.GeofenceName != null)
            {
                objResponse.GeofenceName = request.GeofenceName;
            }
            if (request.Type != null)
            {
                objResponse.Type = request.Type;
            }
            objResponse.GeofenceId = request.GeofenceID;
            objResponse.CategoryId = request.CategoryID;
            objResponse.SubCategoryId = request.SubcategoryId;
            return objResponse;
        }

        public Geofence ToGeofenceEntity(net.atos.daf.ct2.geofenceservice.GeofenceRequest geofenceRequest)
        {
            Geofence geofence = new Geofence();
            geofence.Id = Convert.ToInt32(geofenceRequest.Id);
            geofence.OrganizationId = Convert.ToInt32(geofenceRequest.OrganizationId);
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = geofenceRequest.Name;
            if (!string.IsNullOrEmpty(geofenceRequest.Type))
            {
                char type = Convert.ToChar(geofenceRequest.Type);
                if (type == 'C' || type == 'c')
                {
                    geofence.Type = ((char)LandmarkType.CircularGeofence).ToString();
                }
                else
                {
                    geofence.Type = ((char)LandmarkType.PolygonGeofence).ToString();
                }
            }
            geofence.Address = geofenceRequest.Address;
            geofence.City = geofenceRequest.City;
            geofence.Country = geofenceRequest.Country;
            geofence.Zipcode = geofenceRequest.Zipcode;
            geofence.Latitude = geofenceRequest.Latitude;
            geofence.Longitude = geofenceRequest.Longitude;
            geofence.Distance = geofenceRequest.Distance;
            //geofence.State = Convert.ToChar(geofenceRequest.State);
            geofence.Width = geofenceRequest.Width;
            geofence.Nodes = new List<Nodes>();
            foreach (var item in geofenceRequest.Nodes)
            {
                if (item != null)
                {
                    geofence.Nodes.Add(ToNodesEntity(item));
                }
            }
            geofence.CreatedBy = geofenceRequest.CreatedBy;
            return geofence;
        }

        public Nodes ToNodesEntity(NodeRequest nodeRequest)
        {
            Nodes nodes = new Nodes();
            if (nodeRequest.Id > 0)
                nodes.Id = Convert.ToInt32(nodeRequest.Id);
            nodes.LandmarkId = nodeRequest.LandmarkId;
            nodes.SeqNo = nodeRequest.SeqNo;
            nodes.Latitude = nodeRequest.Latitude;
            nodes.Longitude = nodeRequest.Longitude;
            nodes.State = nodeRequest.State;
            nodes.Address = nodeRequest.Address;
            nodes.TripId = nodeRequest.TripId;
            return nodes;
        }
        public POI ToPOIEntity(net.atos.daf.ct2.poiservice.POIRequest poiRequest)
        {
            POI poi = new POI();
            poi.Id = poiRequest.Id;
            poi.OrganizationId = poiRequest.OrganizationId != null ? poiRequest.OrganizationId : 0;
            poi.CategoryId = poiRequest.CategoryId;
            poi.SubCategoryId = poiRequest.SubCategoryId;
            poi.Name = poiRequest.Name;
            poi.Type = poiRequest.Type;
            poi.Address = poiRequest.Address;
            poi.City = poiRequest.City;
            poi.Country = poiRequest.Country;
            poi.Zipcode = poiRequest.Zipcode;
            poi.Latitude = Convert.ToDouble(poiRequest.Latitude);
            poi.Longitude = Convert.ToDouble(poiRequest.Longitude);
            poi.Distance = Convert.ToDouble(poiRequest.Distance);
            poi.State = poiRequest.State;
            poi.CreatedBy = poiRequest.CreatedBy;
            poi.CreatedAt = poiRequest.CreatedAt;
            return poi;
        }
        public net.atos.daf.ct2.poiservice.POIData ToPOIResponseData(POI poiEntity)
        {
            net.atos.daf.ct2.poiservice.POIData poi = new net.atos.daf.ct2.poiservice.POIData();
            poi.Id = poiEntity.Id;
            poi.OrganizationId = poiEntity.OrganizationId != null ? Convert.ToInt32(poiEntity.OrganizationId) : 0;
            poi.CategoryId = poiEntity.CategoryId;
            poi.CategoryName = CheckNull(poiEntity.CategoryName);
            poi.SubCategoryId = poiEntity.SubCategoryId != null ? Convert.ToInt32(poiEntity.SubCategoryId) : 0;
            poi.SubCategoryName = CheckNull(poiEntity.SubCategoryName);
            poi.Name = CheckNull(poiEntity.Name);
            poi.Type = poiEntity.Type;
            poi.Address = CheckNull(poiEntity.Address);
            poi.City = CheckNull(poiEntity.City);
            poi.Country = CheckNull(poiEntity.Country);
            poi.Zipcode = CheckNull(poiEntity.Zipcode);
            poi.Latitude = poiEntity.Latitude;
            poi.Longitude = poiEntity.Longitude;
            poi.Distance = poiEntity.Distance;
            poi.State = poiEntity.State;
            poi.CreatedBy = poiEntity.CreatedBy;
            poi.CreatedAt = poiEntity.CreatedAt;
            poi.Icon = poiEntity.Icon != null ? ByteString.CopyFrom(poiEntity.Icon) : ByteString.Empty;
            return poi;
        }

        string CheckNull(string param)
        {
            return param ?? string.Empty;
        }
        public net.atos.daf.ct2.geofenceservice.GeofenceRequest ToGeofenceRequest(Geofence geofenceRequest)
        {
            GeofenceRequest geofence = new GeofenceRequest();
            geofence.Id = geofenceRequest.Id;
            geofence.OrganizationId = geofenceRequest.OrganizationId;
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = !string.IsNullOrEmpty(geofenceRequest.Name) ? geofenceRequest.Name : string.Empty;
            if (!string.IsNullOrEmpty(geofenceRequest.Type))
            {
                char type = Convert.ToChar(geofenceRequest.Type);
                if (type == 'C' || type == 'c')
                {
                    geofence.Type = ((char)LandmarkType.CircularGeofence).ToString();
                }
                else
                {
                    geofence.Type = ((char)LandmarkType.PolygonGeofence).ToString();
                }
            }
            geofence.Address = !string.IsNullOrEmpty(geofenceRequest.Address) ? geofenceRequest.Address : string.Empty;
            geofence.City = !string.IsNullOrEmpty(geofenceRequest.City) ? geofenceRequest.City : string.Empty;
            geofence.Country = !string.IsNullOrEmpty(geofenceRequest.Country) ? geofenceRequest.Country : string.Empty;
            geofence.Zipcode = !string.IsNullOrEmpty(geofenceRequest.Zipcode) ? geofenceRequest.Zipcode : string.Empty;
            geofence.Latitude = geofenceRequest.Latitude;
            geofence.Longitude = geofenceRequest.Longitude;
            geofence.Distance = geofenceRequest.Distance;
            geofence.State = !string.IsNullOrEmpty(geofenceRequest.State) ? geofenceRequest.State : string.Empty;
            geofence.Width = geofenceRequest.Width;
            //geofence.NodeRequest = new List<NodeRequest>();
            foreach (var item in geofenceRequest.Nodes)
            {
                if (item != null)
                {
                    geofence.Nodes.Add(ToNodesRequest(item));
                }
            }
            geofence.CreatedBy = geofenceRequest.CreatedBy;
            geofence.Message = geofenceRequest.Message;
            geofence.Exists = geofenceRequest.Exists;
            geofence.CategoryName = geofenceRequest.CategoryName;
            geofence.SubCategoryName = geofenceRequest.SubCategoryName;
            return geofence;
        }


        public NodeRequest ToNodesRequest(Nodes nodeRequest)
        {
            NodeRequest nodes = new NodeRequest();
            if (nodeRequest.Id > 0)
                nodes.Id = Convert.ToInt32(nodeRequest.Id);
            nodes.LandmarkId = nodeRequest.LandmarkId;
            nodes.SeqNo = nodeRequest.SeqNo;
            nodes.Latitude = nodeRequest.Latitude;
            nodes.Longitude = nodeRequest.Longitude;
            nodes.State = nodeRequest.State;
            nodes.Message = nodeRequest.Message;
            nodes.Address = !string.IsNullOrEmpty(nodeRequest.Address) ? nodeRequest.Address : string.Empty;
            nodes.TripId = !string.IsNullOrEmpty(nodeRequest.TripId) ? nodeRequest.TripId : string.Empty;
            return nodes;
        }

        public Geofence ToGeofenceUpdateEntity(GeofencePolygonUpdateRequest geofenceRequest)
        {
            Geofence geofence = new Geofence();
            geofence.Id = Convert.ToInt32(geofenceRequest.Id);
            geofence.OrganizationId = Convert.ToInt32(geofenceRequest.OrganizationId);
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = geofenceRequest.Name;
            geofence.ModifiedBy = geofenceRequest.ModifiedBy;
            return geofence;
        }

        public GeofencePolygonUpdateRequest ToGeofenceUpdateRequest(Geofence geofenceRequest)
        {
            GeofencePolygonUpdateRequest geofence = new GeofencePolygonUpdateRequest();
            geofence.Id = Convert.ToInt32(geofenceRequest.Id);
            geofence.OrganizationId = Convert.ToInt32(geofenceRequest.OrganizationId);
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = geofenceRequest.Name;
            geofence.ModifiedBy = geofenceRequest.ModifiedBy;
            return geofence;
        }

        public Geofence ToGeofenceUpdateEntity(GeofenceCircularUpdateRequest geofenceRequest)
        {
            Geofence geofence = new Geofence();
            geofence.Id = Convert.ToInt32(geofenceRequest.Id);
            geofence.OrganizationId = Convert.ToInt32(geofenceRequest.OrganizationId);
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = geofenceRequest.Name;
            geofence.ModifiedBy = geofenceRequest.ModifiedBy;
            geofence.Distance = geofenceRequest.Distance;
            return geofence;
        }
        public GeofenceCircularUpdateRequest ToCircularGeofenceUpdateRequest(Geofence geofenceRequest)
        {
            GeofenceCircularUpdateRequest geofence = new GeofenceCircularUpdateRequest();
            geofence.Id = Convert.ToInt32(geofenceRequest.Id);
            geofence.OrganizationId = Convert.ToInt32(geofenceRequest.OrganizationId);
            geofence.CategoryId = geofenceRequest.CategoryId;
            geofence.SubCategoryId = geofenceRequest.SubCategoryId;
            geofence.Name = geofenceRequest.Name;
            geofence.ModifiedBy = geofenceRequest.ModifiedBy;
            geofence.Distance = geofenceRequest.Distance;
            return geofence;
        }

        public UploadPOIExcel ToUploadPOIRequest(POIUploadRequest poiUploadRequest)
        {
            var uploadPoiExcelData = new UploadPOIExcel() { PoiExcelList = new List<POI>() };
            uploadPoiExcelData.PoiExcelList.AddRange(poiUploadRequest.POIList.Select(x => new POI()
            {
                OrganizationId = x.OrganizationId,
                CategoryId = x.CategoryId,
                SubCategoryId = x.SubCategoryId,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                Country = x.Country,
                Zipcode = x.Zipcode,
                Type = "POI",
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Distance = x.Distance,
                // TripId = x.TripId,
                State = x.State,
                CreatedBy = x.CreatedBy
            }).ToList());
            return uploadPoiExcelData;


        }

        public POIUploadResponse ToPOIUploadResponseData(UploadPOIExcel uploadPOIExcel)
        {
            var poiResponse = new POIUploadResponse();
            poiResponse.PoiDuplicateList.AddRange(uploadPOIExcel.PoiDuplicateList
                                     .Select(x => new POIRequest()
                                     {
                                         Id = x.Id,
                                         OrganizationId = x.OrganizationId,
                                         CategoryId = x.CategoryId,
                                         SubCategoryId = x.SubCategoryId,
                                         Name = x.Name,
                                         Address = x.Address,
                                         City = x.City,
                                         Country = x.Country,
                                         Zipcode = x.Zipcode,
                                         Type = x.Type,
                                         Latitude = x.Latitude,
                                         Longitude = x.Longitude,
                                         Distance = x.Distance,
                                         //TripId = x.TripId,
                                         State = x.State,
                                         CreatedBy = x.CreatedBy
                                     }).ToList());
            poiResponse.PoiUploadedList.AddRange(uploadPOIExcel.PoiUploadedList
                                    .Select(x => new POIRequest()
                                    {
                                        Id = x.Id,
                                        OrganizationId = x.OrganizationId,
                                        CategoryId = x.CategoryId,
                                        SubCategoryId = x.SubCategoryId,
                                        Name = x.Name,
                                        Address = x.Address,
                                        City = x.City,
                                        Country = x.Country,
                                        Zipcode = x.Zipcode,
                                        Type = x.Type,
                                        Latitude = x.Latitude,
                                        Longitude = x.Longitude,
                                        Distance = x.Distance,
                                        //  TripId = x.TripId,
                                        State = x.State,
                                        CreatedBy = x.CreatedBy
                                    }).ToList());
            return poiResponse;
        }

        public net.atos.daf.ct2.poiservice.TripData ToTripResponce(net.atos.daf.ct2.poigeofence.entity.TripEntityResponce entity)
        {
            try
            {
                net.atos.daf.ct2.poiservice.TripData response = new TripData();
                response.Id = entity.Id;
                if (entity.StartAddress != null)
                {
                    response.StartAddress = entity.StartAddress;
                }
                response.TripId = entity.TripId;
                response.StartPositionlattitude = entity.StartPositionlattitude;
                response.StartPositionLongitude = entity.StartPositionLongitude;
                response.EndPositionLattitude = entity.EndPositionLattitude;
                response.EndPositionLongitude = entity.EndPositionLongitude;
                response.StartTimeStamp = entity.StartTimeStamp;
                response.EndTimeStamp = entity.EndTimeStamp;

                if (entity.VIN != null)
                {
                    response.VIN = entity.VIN;
                }
                response.Distance = entity.Distance;
                if (entity.DriverFirstName != null)
                {
                    response.DriverFirstName = entity.DriverFirstName;
                }
                if (entity.DriverLastName != null)
                {
                    response.DriverLastName = entity.DriverLastName;
                }
                if (entity.DriverId1 != null)
                {
                    response.DriverId1 = entity.DriverId1;
                }
                if (entity.DriverId2 != null)
                {
                    response.DriverId2 = entity.DriverId2;
                }
                if (entity.EndAddress != null)
                {
                    response.EndAddress = entity.EndAddress;
                }
                if (entity.StartAddress != null)
                {
                    response.StartAddress = entity.StartAddress;
                }
                if (entity.LiveFleetPosition != null)
                {
                    if (entity.LiveFleetPosition.Count() > 0)
                    {
                        foreach (var item in entity.LiveFleetPosition)
                        {
                            net.atos.daf.ct2.poiservice.LiveFleetPosition liveFleetPosition = new poiservice.LiveFleetPosition();
                            liveFleetPosition.GpsAltitude = item.GpsAltitude;
                            liveFleetPosition.GpsHeading = item.GpsHeading;
                            liveFleetPosition.GpsLatitude = item.GpsLatitude;
                            liveFleetPosition.GpsLongitude = item.GpsLongitude;
                            liveFleetPosition.Id = item.Id;
                            response.LiveFleetPosition.Add(liveFleetPosition);
                        }
                    }
                }
                return response;
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
}

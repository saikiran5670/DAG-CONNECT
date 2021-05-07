using Google.Protobuf;
using net.atos.daf.ct2.poiservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class Mapper
    {
        public POIRequest ToPOIRequest(POI poi)
        {
            POIRequest poiRequest = new POIRequest();
            poiRequest.Id = poi.Id;
            poiRequest.OrganizationId = poi.OrganizationId;
            poiRequest.CategoryId = poi.CategoryId;
            poiRequest.SubCategoryId = poi.SubCategoryId;
            poiRequest.Name = poi.Name;
            //poiRequest.Type = poi.Type;
            poiRequest.Address = poi.Address;
            poiRequest.City = poi.City;
            poiRequest.Country = poi.Country;
            poiRequest.Zipcode = poi.Zipcode;
            poiRequest.Latitude = poi.Latitude;
            poiRequest.Longitude = poi.Longitude;
            poiRequest.State = poi.State;
            poiRequest.CreatedBy = poi.CreatedBy;
            return poiRequest;
        }
        public net.atos.daf.ct2.portalservice.Entity.POI.POIResponse ToPOIEntity(POIData poiResponseData)
        {
            net.atos.daf.ct2.portalservice.Entity.POI.POIResponse poi = new net.atos.daf.ct2.portalservice.Entity.POI.POIResponse();
            poi.Id = poiResponseData.Id;
            poi.OrganizationId = poiResponseData.OrganizationId != null ? poiResponseData.OrganizationId.Value : 0;
            poi.CategoryId = poiResponseData.CategoryId;
            poi.SubCategoryId = poiResponseData.SubCategoryId != null ? poiResponseData.SubCategoryId.Value : 0; 
            poi.Name = poiResponseData.Name;
            poi.SubCategoryName = poiResponseData.SubCategoryName;
            poi.CategoryName = poiResponseData.CategoryName;
            //poi.Type = poiResponseData.Type;
            poi.Address = poiResponseData.Address;
            poi.City = poiResponseData.City;
            poi.Country = poiResponseData.Country;
            poi.Zipcode = poiResponseData.Zipcode;
            poi.Latitude = Convert.ToDouble(poiResponseData.Latitude);
            poi.Longitude = Convert.ToDouble(poiResponseData.Longitude);
            //poi.Distance = Convert.ToDouble(poiResponseData.Distance);
            poi.State = poiResponseData.State;
            poi.CreatedAt = poiResponseData.CreatedAt;
            poi.Icon = poiResponseData.Icon != null ? poiResponseData.Icon.ToByteArray() : new Byte[] { };
            return poi;
        }


        public POIUploadRequest ToUploadRequest(List<POI> poiList)
        {

            var packageRequest = new POIUploadRequest();
            packageRequest.POIList.AddRange(poiList.Select(x => new POIRequest()
            {
                OrganizationId = x.OrganizationId,
                CategoryId = x.CategoryId,
                SubCategoryId = x.SubCategoryId,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                Country = x.Country,
                Zipcode = x.Zipcode,
              //  Type = x.Type,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
               // Distance = x.Distance,
              //  TripId = x.TripId,
                State = x.State,
                CreatedBy = x.CreatedBy
            }).ToList());
            return packageRequest;

        }



        public LandmarkType Maplandmarktype(string type)
        {
            var landmarktype = LandmarkType.None;
            switch (type)
            {
                case "C":
                    landmarktype = LandmarkType.CircularGeofence;
                    break;
                case "R":
                    landmarktype = LandmarkType.RouteCorridor;
                    break;
                case "N":
                    landmarktype = LandmarkType.None;
                    break;
                case "P":
                    landmarktype = LandmarkType.POI;
                    break;
                case "O":
                    landmarktype = LandmarkType.PolygonGeofence;
                    break;
                case "E":
                    landmarktype = LandmarkType.ExistingTripCorridor;
                    break;
               

            }
            return landmarktype;
        }
    }
}

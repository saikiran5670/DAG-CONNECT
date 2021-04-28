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
            return poiRequest;
        }
        public net.atos.daf.ct2.portalservice.Entity.POI.POIResponse ToPOIEntity(POIData poiResponseData)
        {
            net.atos.daf.ct2.portalservice.Entity.POI.POIResponse poi = new net.atos.daf.ct2.portalservice.Entity.POI.POIResponse();
            poi.Id = poiResponseData.Id;
            poi.OrganizationId = poiResponseData.OrganizationId != null ? poiResponseData.OrganizationId.Value : 0;
            poi.CategoryId = poiResponseData.CategoryId;
            poi.SubCategoryId = poiResponseData.SubCategoryId;
            poi.Name = poiResponseData.Name;
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
            return poi;
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
                    landmarktype = LandmarkType.Corridor;
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
                case "U":
                    landmarktype = LandmarkType.Route;
                    break;
               

            }
            return landmarktype;
        }
    }
}

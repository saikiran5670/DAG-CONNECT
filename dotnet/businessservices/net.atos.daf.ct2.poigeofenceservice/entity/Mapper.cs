using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.poigeofence;
//using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class Mapper
    {
        public net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce ToGeofenceList(net.atos.daf.ct2.poigeofence.entity.GeofenceEntityResponce request)
        {
            net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce objResponse = new net.atos.daf.ct2.geofenceservice.GeofenceEntityResponce();
            objResponse.CategoryName = request.category;
            objResponse.SubCategoryName = request.subCategory;
            objResponse.GeofenceName = request.geofenceName;
            objResponse.GeofenceId = request.geofenceID;
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
            geofence.TripId = geofenceRequest.TripId;
            geofence.Nodes = new List<Nodes>();
            foreach (var item in geofenceRequest.NodeRequest)
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
            return nodes;
        }
        public POI ToPOIEntity(net.atos.daf.ct2.poiservice.POIRequest poiRequest)
        {
            POI poi = new POI();
            poi.Id = Convert.ToInt32(poiRequest.Id);
            poi.OrganizationId = Convert.ToInt32(poiRequest.OrganizationId);
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
            poi.TripId = poiRequest.TripId;
            poi.CreatedBy = poiRequest.CreatedBy;
            return poi;
        }
    }
}

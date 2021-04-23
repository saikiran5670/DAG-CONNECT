using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class Mapper
    {
        public net.atos.daf.ct2.poigeofenceservice.GeofenceEntityResponce ToGeofenceList(net.atos.daf.ct2.poigeofence.entity.GeofenceEntityResponce request)
        {
            net.atos.daf.ct2.poigeofenceservice.GeofenceEntityResponce objResponse = new net.atos.daf.ct2.poigeofenceservice.GeofenceEntityResponce();
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
                    geofence.Type = Convert.ToChar(LandmarkType.CircularGeofence);
                }
                else
                {
                    geofence.Type = Convert.ToChar(LandmarkType.PolygonGeofence);
                }
            }
            geofence.Address = geofenceRequest.Address;
            geofence.City = geofenceRequest.City;
            geofence.Country = geofenceRequest.Country;
            geofence.Zipcode = geofenceRequest.Zipcode;
            geofence.Latitude = Convert.ToDecimal(geofenceRequest.Latitude);
            geofence.Longitude = Convert.ToDecimal(geofenceRequest.Longitude);
            geofence.Distance = Convert.ToDecimal(geofenceRequest.Distance);
            //geofence.State = Convert.ToChar(geofenceRequest.State);
            geofence.TripId = geofenceRequest.TripId;
            foreach (var item in geofenceRequest.NodeRequest)
            {
                if (item != null)
                    geofence.Nodes.Add(ToNodesEntity(item));
            }
            geofence.CreatedBy = geofenceRequest.CreatedBy;
            return geofence;
        }

        public Nodes ToNodesEntity(NodeRequest nodeRequest)
        {
            Nodes nodes = new Nodes();
            nodes.Id = Convert.ToInt32(nodeRequest.Id);
            nodes.LandmarkId = nodeRequest.LandmarkId;
            nodes.SeqNo = nodeRequest.SeqNo;
            nodes.Latitude = Convert.ToDecimal(nodeRequest.Latitude);
            nodes.Longitude = Convert.ToDecimal(nodeRequest.Longitude);
            return nodes;
        }
    }
}

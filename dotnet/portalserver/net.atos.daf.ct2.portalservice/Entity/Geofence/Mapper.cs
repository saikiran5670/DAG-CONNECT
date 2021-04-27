﻿using net.atos.daf.ct2.geofenceservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Geofence
{
    public class Mapper
    {
        public GeofenceRequest ToGeofenceRequest(Geofence geofence)
        {
            GeofenceRequest geofenceRequest = new GeofenceRequest();
            geofenceRequest.Id = geofence.Id;
            geofenceRequest.OrganizationId = geofence.OrganizationId;
            geofenceRequest.CategoryId = geofence.CategoryId;
            geofenceRequest.SubCategoryId = geofence.SubCategoryId;
            geofenceRequest.Name = geofence.Name;
            geofenceRequest.Type = geofence.Type;
            geofenceRequest.Address = geofence.Address;
            geofenceRequest.City = geofence.City;
            geofenceRequest.Country = geofence.Country;
            geofenceRequest.Zipcode = geofence.Zipcode;
            geofenceRequest.Latitude = geofence.Latitude;
            geofenceRequest.Longitude = geofence.Longitude;
            geofenceRequest.Distance = geofence.Distance;
            geofenceRequest.TripId = geofence.TripId;
            geofenceRequest.CreatedBy = geofence.CreatedBy;
            foreach (var item in geofence.Nodes)
            {
                geofenceRequest.NodeRequest.Add(ToNodeRequest(item));
            }
            return geofenceRequest;
        }

        public NodeRequest ToNodeRequest(Nodes nodes)
        {
            NodeRequest nodeRequest = new NodeRequest();
            nodeRequest.Id = nodes.Id;
            nodeRequest.LandmarkId = nodes.LandmarkId;
            nodeRequest.SeqNo = nodes.SeqNo;
            nodeRequest.Latitude = nodes.Latitude;
            nodeRequest.Longitude = nodes.Longitude;
            return nodeRequest;
        }

        public GeofenceRequest ToCircularGeofenceRequest(CircularGeofence geofence)
        {
            GeofenceRequest geofenceRequest = new GeofenceRequest();
            geofenceRequest.Id = geofence.Id;
            geofenceRequest.OrganizationId = geofence.OrganizationId;
            geofenceRequest.CategoryId = geofence.CategoryId;
            geofenceRequest.SubCategoryId = geofence.SubCategoryId;
            geofenceRequest.Name = geofence.Name;
            geofenceRequest.Type = geofence.Type;
            geofenceRequest.Address = geofence.Address;
            geofenceRequest.City = geofence.City;
            geofenceRequest.Country = geofence.Country;
            geofenceRequest.Zipcode = geofence.Zipcode;
            geofenceRequest.Latitude = geofence.Latitude;
            geofenceRequest.Longitude = geofence.Longitude;
            geofenceRequest.Distance = geofence.Distance;
            geofenceRequest.TripId = geofence.TripId;
            geofenceRequest.CreatedBy = geofence.CreatedBy;
            return geofenceRequest;
        }

        public GeofencePolygonUpdateRequest ToGeofenceUpdateRequest(GeofenceUpdateEntity geofence)
        {
            GeofencePolygonUpdateRequest geofenceRequest = new GeofencePolygonUpdateRequest();
            geofenceRequest.Id = geofence.Id;
            geofenceRequest.CategoryId = geofence.CategoryId;
            geofenceRequest.SubCategoryId = geofence.SubCategoryId;
            geofenceRequest.Name = geofence.Name;
            geofenceRequest.ModifiedBy = geofence.ModifiedBy;
            return geofenceRequest;
        }

        public GeofenceCircularUpdateRequest ToCircularGeofenceUpdateRequest(GeofenceUpdateEntity geofence)
        {
            GeofenceCircularUpdateRequest geofenceRequest = new GeofenceCircularUpdateRequest();
            geofenceRequest.Id = geofence.Id;
            geofenceRequest.CategoryId = geofence.CategoryId;
            geofenceRequest.SubCategoryId = geofence.SubCategoryId;
            geofenceRequest.Name = geofence.Name;
            geofenceRequest.ModifiedBy = geofence.ModifiedBy;
            return geofenceRequest;
        }
    }
}

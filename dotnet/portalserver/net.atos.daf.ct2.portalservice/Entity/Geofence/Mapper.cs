using net.atos.daf.ct2.geofenceservice;

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
            geofenceRequest.Width = geofence.Width;
            geofenceRequest.CreatedBy = geofence.CreatedBy;
            foreach (var item in geofence.Nodes)
            {
                geofenceRequest.Nodes.Add(ToNodeRequest(item));
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
            nodeRequest.Address = nodes.Address;
            nodeRequest.TripId = nodes.TripId;
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
            geofenceRequest.Width = geofence.Width;
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
            geofenceRequest.Distance = geofence.Distance;
            geofenceRequest.OrganizationId = geofence.OrganizationId;
            return geofenceRequest;
        }
    }
}

using net.atos.daf.ct2.geofenceservice;
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
            geofenceRequest.Name = geofence.Name;
            geofenceRequest.Type = geofence.Type;
            geofenceRequest.Address = geofence.Address;
            geofenceRequest.City = geofence.City;
            geofenceRequest.Country = geofence.Country;
            geofenceRequest.Zipcode = geofence.Zipcode;
            return geofenceRequest;
        }
    }
}

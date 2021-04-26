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
            poiRequest.Name = poi.Name;
            poiRequest.Type = poi.Type;
            poiRequest.Address = poi.Address;
            poiRequest.City = poi.City;
            poiRequest.Country = poi.Country;
            poiRequest.Zipcode = poi.Zipcode;
            return poiRequest;
        }
    }
}

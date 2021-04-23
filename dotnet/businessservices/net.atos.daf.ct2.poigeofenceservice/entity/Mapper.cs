using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofence = net.atos.daf.ct2.poigeofence;
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
    }
}

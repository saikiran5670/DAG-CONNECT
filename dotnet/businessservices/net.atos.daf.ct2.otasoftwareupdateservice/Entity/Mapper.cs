using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Entity
{
    public class Mapper
    {
        public httpclientservice.VehiclesStatusOverviewRequest MapVehiclesStatusOverviewRequest(string language,
                                                                                                string retention ,
                                                                                                IEnumerable<string> vins)
        {
            var returnObj = new httpclientservice.VehiclesStatusOverviewRequest();
            returnObj.Language = language;
            returnObj.Retention = retention;
            foreach (var item in vins)
            {
                returnObj.Vins.Add(item);
            }
            return returnObj;
        }
    }
}

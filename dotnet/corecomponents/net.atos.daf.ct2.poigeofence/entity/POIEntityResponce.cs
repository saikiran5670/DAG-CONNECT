using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class POIEntityResponse
    {
        public string POIName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
    }

    public class POIEntityRequest
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}

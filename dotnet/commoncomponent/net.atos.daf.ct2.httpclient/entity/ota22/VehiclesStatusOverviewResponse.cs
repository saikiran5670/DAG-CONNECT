using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace net.atos.daf.ct2.httpclient.entity
{
    public class VehiclesStatusOverview
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public string vin { get; set; }
        public string status { get; set; }
        public string description { get; set; }
    }

    public class VehiclesStatusOverviewResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public VehiclesStatusOverview VehiclesStatusOverview { get; set; }
    }
}

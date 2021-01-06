using System;
using Newtonsoft.Json;
using System.Net;

namespace net.atos.daf.ct2.exceptionhandling.entity
{
    public class ExceptionDetails
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

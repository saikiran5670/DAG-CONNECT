using System.Net;

namespace net.atos.daf.ct2.identity.entity
{
    public class Response
    {
        public Response()
        { }
        public Response(HttpStatusCode statusCode, object result = null)
        {
            StatusCode = statusCode;
            Result = result;
        }
        public HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
    }

    public class IdentityResponse
    {
        public string Error { get; set; }
        public string Error_Description { get; set; }
    }
}
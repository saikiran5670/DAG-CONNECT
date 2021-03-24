using System.Net.Http;

namespace net.atos.daf.ct2.identity.entity
{
    public class Response
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
    }

    public class IdentityResponse
    {
        public string Error { get; set; }
        public string Error_Description { get; set; }
    }
}
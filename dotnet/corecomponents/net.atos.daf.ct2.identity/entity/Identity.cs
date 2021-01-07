using System.Net.Http;

namespace net.atos.daf.ct2.identity.entity
{
    public class Identity
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
    }
}
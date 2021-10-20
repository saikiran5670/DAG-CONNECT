using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.vehicledataservice.Entity
{
    public class VehicleNamelistRequest
    {
        public string Since { get; set; }
        public string Token { get; set; }
        public string Org { get; set; }
    }
}
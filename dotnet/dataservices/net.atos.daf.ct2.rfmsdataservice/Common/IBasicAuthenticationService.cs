using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Common
{
    public interface IBasicAuthenticationService
    {
        Task<string> ValidateToken(string token);
    }
}

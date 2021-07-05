using System.Threading.Tasks;

namespace net.atos.daf.ct2.ecoscoredataservice
{
    public interface IBasicAuthenticationService
    {
        Task<string> ValidateTokenGuid(string token);
    }
}

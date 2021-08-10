using System.Threading.Tasks;

namespace net.atos.daf.ct2.accountdataservice
{
    public interface IBasicAuthenticationService
    {
        Task<string> ValidateTokenGuid(string token);
    }
}

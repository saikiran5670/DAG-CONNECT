using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
namespace net.atos.daf.ct2.identity
{
    public interface IAccountAuthenticator
    {
        Task<Response> AccessToken(Identity user);
        string getURL(Identity user);
    }
}
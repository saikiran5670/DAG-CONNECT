using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
namespace net.atos.daf.ct2.identity
{
    public interface IAutheticator
    {
          Task<Response> AccessToken(Account user);
          string getURL(Account user);
    }
}
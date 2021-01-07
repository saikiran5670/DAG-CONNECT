using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
namespace net.atos.daf.ct2.identity
{
    public interface IAccountManager
    {
        Task<Response> CreateUser(Identity user);
        Task<Response> UpdateUser(Identity user);
        Task<Response> DeleteUser(Identity user); 
        Task<Response> ChangeUserPassword(Identity user);

    }
}

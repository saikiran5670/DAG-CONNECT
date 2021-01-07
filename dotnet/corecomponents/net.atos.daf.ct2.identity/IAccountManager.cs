using System.Threading.Tasks;
using net.atos.daf.ct2.identity.entity;
using net.atos.daf.ct2.account.entity;
namespace net.atos.daf.ct2.identity
{
    public interface IAccountManager
    {
        Task<Response> CreateUser(Account user);
        Task<Response> UpdateUser(Account user);
        Task<Response> DeleteUser(Account user); 
        Task<Response> ChangeUserPassword(Account user);

    }
}

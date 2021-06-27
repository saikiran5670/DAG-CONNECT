using net.atos.daf.ct2.notificationengine.entity;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.notificationengine
{
    public interface ITripAlertManager
    {
        Task<TripAlert> CreateTripAlert(TripAlert tripAlert);
    }
}

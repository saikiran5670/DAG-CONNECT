using pushnotificationcorecomponent.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pushnotificationcorecomponent.Repository
{
    public interface ITripAlertRepository
    {
        Task<TripAlert> CreateTripAlert(TripAlert tripAlert);
    }
}

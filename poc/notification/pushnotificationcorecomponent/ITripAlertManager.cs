using pushnotificationcorecomponent.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pushnotificationcorecomponent
{
   public interface ITripAlertManager
    {
        Task<TripAlert> CreateTripAlert(TripAlert tripAlert);
    }
}

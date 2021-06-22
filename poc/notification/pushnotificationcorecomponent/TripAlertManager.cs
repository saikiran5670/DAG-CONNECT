using pushnotificationcorecomponent.Entity;
using pushnotificationcorecomponent.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pushnotificationcorecomponent
{
    public class TripAlertManager:ITripAlertManager
    {
        private readonly ITripAlertRepository _tripAlertRepository;
        public TripAlertManager(ITripAlertRepository tripAlertRepository)
        {
            _tripAlertRepository = tripAlertRepository;
        }
        public async Task<TripAlert> CreateTripAlert(TripAlert tripAlert)
        {
            return await _tripAlertRepository.CreateTripAlert(tripAlert);
        }
    }
}

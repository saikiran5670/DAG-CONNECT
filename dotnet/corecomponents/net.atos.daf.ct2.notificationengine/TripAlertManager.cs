using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.notificationengine.repository;

namespace net.atos.daf.ct2.notificationengine
{
    public class TripAlertManager : ITripAlertManager
    {
        private readonly ITripAlertRepository _tripAlertRepository;
        public TripAlertManager(ITripAlertRepository tripAlertRepository)
        {
            _tripAlertRepository = tripAlertRepository;
        }
        public async Task<TripAlert> CreateTripAlert(TripAlert tripAlert) => await _tripAlertRepository.CreateTripAlert(tripAlert);
    }
}

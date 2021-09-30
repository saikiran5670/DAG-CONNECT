using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public class OtaSoftwareNotification : IOtaSoftwareNotification
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public OtaSoftwareNotification(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataMartdataAccess = dataMartdataAccess;
            _dataAccess = dataAccess;

        }

        public async Task<TripAlertOtaConfigParam> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam)
        {
            return tripAlertOtaConfigParam;

        }

    }
}

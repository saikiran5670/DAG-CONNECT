﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.otasoftwareupdate.entity;

namespace net.atos.daf.ct2.otasoftwareupdate.repository
{
    public class OTASoftwareUpdateRepository : IOTASoftwareUpdateRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public OTASoftwareUpdateRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            this._dataAccess = dataAccess;
            this._dataMartdataAccess = dataMartdataAccess;

        }

        #region Get GetVehicleSoftwareStatus List
        public async Task<IEnumerable<VehicleSoftwareStatus>> GetVehicleSoftwareStatus()
        {
            try
            {
                var queryAlert = @"SELECT id as Id, type as Type, enum as Enum, parent_enum as ParentEnum, key as Key, feature_id as FeatureId
                                    FROM translation.enumtranslation
                                    where type ='S'";
                return await _dataAccess.QueryAsync<VehicleSoftwareStatus>(queryAlert);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.map.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.map.repository
{
    public class MapRepository : IMapRepository
    {
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MapRepository(IDataMartDataAccess dataMartDataAccess)
        {
            _dataMartDataAccess = dataMartDataAccess;
        }


        public async Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses)
        {
            try
            {
                var addresses = new List<LookupAddress>();
                foreach (var lookupAddress in lookupAddresses)
                {

                    var queryStatement = @"select   id, longitude, latitude, address, created_at, modified_at                                 
                                   from master.geolocationaddress 
                                   where 1=1";

                    var parameter = new DynamicParameters();
                    if (lookupAddress.Latitude > 0)
                    {
                        parameter.Add("@latitude", lookupAddress.Latitude);
                    }
                    if (lookupAddress.Longitude > 0)
                    {
                        parameter.Add("@longitude", lookupAddress.Longitude);
                    }
                    if (lookupAddress.Id > 0)
                    {
                        parameter.Add("@id", lookupAddress.Id);
                    }

                    var result = await _dataMartDataAccess.QueryAsync<LookupAddress>(queryStatement, parameter);
                    var d = Between(18.50248, 18.50246, 18.5025, false);


                    addresses.Add(result as LookupAddress);
                }
                return addresses;
            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

        public bool Between(double num, double lower, double upper, bool inclusive)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
        public async Task<List<LookupAddress>> AddLookupAddress(List<LookupAddress> lookupAddresses)
        {
            var geolocationaddresses = new List<LookupAddress>();
            try
            {
                var addresses = new List<LookupAddress>();
                foreach (var lookupAddress in lookupAddresses)
                {

                    var query = @"Insert INTO master.geolocationaddress(
	                                                            longitude, latitude, address, created_at) 
	                                                            VALUES (@longitude, @latitude, @address,@created_at) RETURNING id";


                    var parameter = new DynamicParameters();
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    if (lookupAddress.Latitude > 0)
                    {
                        parameter.Add("@latitude", lookupAddress.Latitude);
                    }
                    if (lookupAddress.Longitude > 0)
                    {
                        parameter.Add("@longitude", lookupAddress.Longitude);
                    }
                    if (lookupAddress.Address != string.Empty)
                    {
                        parameter.Add("@address", lookupAddress.Address);
                    }

                    var result = await _dataMartDataAccess.ExecuteScalarAsync<int>(query, parameter);
                    lookupAddress.Id = result;
                    geolocationaddresses.Add(lookupAddress);


                }
                return geolocationaddresses; // have to change this  per logic
            }

            catch (Exception ex)
            {

                log.Error(ex.ToString());
                return geolocationaddresses;
            }


        }






    }


}

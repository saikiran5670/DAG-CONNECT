
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.map.entity;
using net.atos.daf.ct2.utilities;
using System;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.map.repository
{
    public class MapRepository : IMapRepository
    {
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private readonly MapHelper _mapHelper;
        private static readonly log4net.ILog _log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MapRepository(IDataMartDataAccess dataMartDataAccess)
        {
            _dataMartDataAccess = dataMartDataAccess;
            _mapHelper = new MapHelper();
        }
        public async Task<LookupAddress> GetMapAddress(LookupAddress lookupAddress)
        {
            try
            {

                DynamicParameters parameter = new DynamicParameters();
                //MapLatLngRange addressRange = _mapHelper.GetLatLonRange(lookupAddress.Latitude, lookupAddress.Longitude);
                //Consider Co-ordinates decimal upto 4 places, that means accurancy of 11 meter or less 
                //There would be only 9 address under 11 metere range 
                //an accuracy of 4 decimal places is accurate to 11.1 meters (+/- 5.55 m) 
                string queryStatement = @"select   id, longitude, latitude, address, created_at, modified_at                                 
                                            from master.geolocationaddress 
                                            where TRUNC(CAST(latitude as numeric),4)= TRUNC(CAST(@latitude as numeric),4) 
                                            and TRUNC(CAST(longitude as numeric),4)= TRUNC(CAST(@longitude as numeric),4) ";

                parameter.Add("@latitude", lookupAddress.Latitude);
                parameter.Add("@longitude", lookupAddress.Longitude);

                var result = await _dataMartDataAccess.QueryFirstOrDefaultAsync<LookupAddress>(queryStatement, parameter);
                lookupAddress = result as LookupAddress;
                return lookupAddress;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return lookupAddress;

            }

        }
        public async Task<LookupAddress> AddMapAddress(LookupAddress lookupAddress)
        {
            try
            {

                string query = @"Insert INTO master.geolocationaddress(
	                                                            longitude, latitude, address, created_at) 
	                                                            VALUES (@longitude, @latitude, @address,@created_at) RETURNING id,address,longitude, latitude, address, created_at";


                DynamicParameters parameter = new DynamicParameters();
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

                var result = await _dataMartDataAccess.QueryFirstAsync<LookupAddress>(query, parameter);
                lookupAddress.Id = result.Id;
                lookupAddress.Address = result.Address;
                return lookupAddress;
            }

            catch (Exception ex)
            {

                _log.Error(ex.ToString());
                return lookupAddress;
            }


        }


    }


}

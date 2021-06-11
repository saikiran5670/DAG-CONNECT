
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
                MapLatLngRange addressRange = _mapHelper.GetLatLonRange(lookupAddress.Latitude, lookupAddress.Longitude);
                string queryStatement = @"select   id, longitude, latitude, address, created_at, modified_at                                 
                                   from master.geolocationaddress 
                                   where 1=1 and latitude =any(@latitudeRange) or  longitude =any(@longitudeRange)";
                parameter.Add("@latitudeRange", addressRange.Latitude);
                parameter.Add("@longitudeRange", addressRange.Longitude);

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

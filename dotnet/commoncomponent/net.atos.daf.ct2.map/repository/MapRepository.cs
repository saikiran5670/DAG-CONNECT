
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.map.entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                    addresses.Add(result as LookupAddress);
                }
                return addresses;
            }

            catch (Exception ex)
            {
                throw ex;
            }


        }      
        public async Task<bool> AddLookupAddress(List<LookupAddress> lookupAddresses)
        {
            try
            {
                var addresses = new List<LookupAddress>();
                foreach (var lookupAddress in lookupAddresses)
                {
                    
                    var query = @"update  master.geolocationaddress  set address=@address where  longitude =@longitude and latitude=@latitude  RETURNING id";
                   


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

                    var result = await _dataMartDataAccess.QueryAsync<int>(query, parameter);
                     
                }
                return true; // have to change this  per logic
            }

            catch (Exception ex)
            {

                log.Error(ex.ToString());
                return false;
            }


        }






    }


}

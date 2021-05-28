
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.map.entity;
using System;
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


        public async Task<string> GetLookupAddress(LookupAddress lookupAddress)
        {
            try
            {
                var queryStatement = @"select id,latitude
                                   ,longitude
                                   ,address                                   
                                   from master.vehicle 
                                   where 1=1";
                var parameter = new DynamicParameters();


                if (lookupAddress.Latitude > 0)
                {
                    parameter.Add("@latitude", lookupAddress.Latitude);
                    queryStatement = queryStatement + " and id LIKE @id";

                }
                if (lookupAddress.longitude > 0)
                {
                    parameter.Add("@vin", lookupAddress.Longitude);
                    queryStatement = queryStatement + " and vin LIKE @id";

                }
                var rfmsVehicles = new RfmsVehicles();
                dynamic result = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                return rfmsVehicles;

            }

            catch (Exception ex)
            {
                throw ex;
            }


        }





    }


}

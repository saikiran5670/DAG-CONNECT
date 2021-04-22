using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class PoiRepository : IPoiRepository
    {
        private readonly IDataAccess _dataAccess;

        private static readonly log4net.ILog log =
      log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PoiRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest)
        {
            log.Info("Subscribe Subscription method called in repository");
            //POIEntityRequest objPOIEntityRequestList = new POIEntityRequest();
            try
            {
                string query = string.Empty;
                query = @"select 
                           l.id
                           ,l.name
                           ,l.latitude
                           ,l.longitude
                           ,c.name as category
                           ,l.city from MASTER.LANDMARK l
                     LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id
                     WHERE 1=1";
                var parameter = new DynamicParameters();
                if (objPOIEntityRequest.organization_id > 0)
                {
                    parameter.Add("@organization_id", objPOIEntityRequest.organization_id);
                    query = $"{query} and l.organization_id=@organization_id ";

                    if (objPOIEntityRequest.category_id >0)
                    {
                        parameter.Add("@category_id", objPOIEntityRequest.category_id);
                        query = $"{query} and l.category_id=@category_id";
                    }

                    else if (objPOIEntityRequest.sub_category_id > 0)
                    {
                        parameter.Add("@sub_category_id", objPOIEntityRequest.sub_category_id);
                        query = $"{query} and l.sub_category_id=@sub_category_id";
                    }
                }
                var data = await _dataAccess.QueryAsync<POIEntityResponce>(query, parameter);
                List<POIEntityResponce> objPOIEntityResponceList = new List<POIEntityResponce>();
                return objPOIEntityResponceList = data.Cast<POIEntityResponce>().ToList();
                //Handel Null Exception
            }
            catch (System.Exception)
            {
            }
            return null;
        }
    }
}

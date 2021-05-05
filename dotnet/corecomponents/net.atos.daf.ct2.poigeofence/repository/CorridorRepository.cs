using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using net.atos.daf.ct2.poigeofence.entity;
using Dapper;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
   public class CorridorRepository : ICorridorRepository
    {
        private readonly IDataAccess _dataAccess;

        public CorridorRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;


        }

        public async Task<List<CorridorResponse>> GetCorridorList(CorridorRequest objCorridorRequest)
        {
            List<CorridorResponse> objCorridorResponseList = new List<CorridorResponse>();
            try
            {
                string query = string.Empty;
                query = @"select l.id 
                                ,l.organization_id as OrganizationId
	                            ,l.name as CorridoreName
	                            ,l.address as StartPoint
	                            ,l.latitude as StartLat
	                            ,l.longitude as StartLong
	                            ,n.address as EndPoint
	                            ,n.latitude as EndLat
	                            ,n.longitude as EndLong
	                            ,l.distance as Distance
	                            ,l.distance as Width
	                            ,l.created_at as Created_At
	                            ,l.created_by as CreatedBy
	                            ,l.modified_at as ModifiedAt
	                            ,l.modified_by as ModifiedBy
                        FROM       master.landmark l
                        INNER JOIN master.nodes n on l.id= n.landmark_id
                        WHERE      l.type IN ('E','R')
                        AND        l.organization_id = @organization_id";

                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
               
                if (objCorridorRequest.CorridorId > 0)
                {
                    parameter.Add("@id", objCorridorRequest.CorridorId);
                    query = $"{query} AND l.id = @id";
                }

                var data = await _dataAccess.QueryAsync<CorridorResponse>(query, parameter);
                return objCorridorResponseList = data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

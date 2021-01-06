using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.visibility.entity;
using net.atos.daf.ct2.data;
using Dapper;
//using NpgsqlTypes;

namespace net.atos.daf.ct2.visibility.repository
{
    public class VisibilityRepository : IVisibilityRepository
    {
        private readonly IDataAccess dataAccess;
        public VisibilityRepository(IDataAccess _dataAccess)
        {
           
            dataAccess = _dataAccess;
        }
        public IEnumerable<FeatureSet> GetFeatureSet(int userid, int orgid )
        {
            var featureSet = new List<FeatureSet>();            
            var func = "dafconnectmaster.getuserrolefeatures";
            var result = dataAccess.Query<Feature>(
                            sql: func,
                            param: new { useridinput = userid,orgidinput = orgid },
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 900) as List<Feature>;

            var parentFeature = result.Where(fe => fe.ParentFeatureId == 0).ToList();
            foreach(var feature in parentFeature)
                {
                        if (feature != null)
                        {
                            var _featureSet = new FeatureSet();
                            _featureSet.FeatureSetID = feature.RoleFeatureId;
                            _featureSet.FeatureSetName = feature.FeatureDescription;
                            // get child features
                            var childFeatures = result.Where(fe => fe.ParentFeatureId == _featureSet.FeatureSetID).ToList();
                            if(childFeatures != null)
                            {
                                _featureSet.Features=new List<Feature>();
                                _featureSet.Features.AddRange(childFeatures);                                
                            }
                            featureSet.Add(_featureSet);
                        }
                }
            return featureSet;
        }
    }
}
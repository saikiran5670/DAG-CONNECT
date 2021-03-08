using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.features.repository
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly IDataAccess dataAccess;
        public FeatureRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        #region Feature Set 

        public async Task<int> AddFeatureSet(FeatureSet featureSet)
        {
            try
            {
                var FeatureSetQueryStatement = @"INSERT INTO master.featureset(
                                                            name, description, is_active,
                                                            is_custom_feature_set)
                                                            VALUES (@name, @description, @is_active, 
                                                            @is_custom_feature_set) RETURNING id";

                            var parameter = new DynamicParameters();
                            parameter.Add("@name", featureSet.Name);
                            parameter.Add("@is_active", true);
                            parameter.Add("@is_custom_feature_set", featureSet.is_custom_feature_set);
                            parameter.Add("@description", featureSet.description);
                            int InsertedFeatureSetId = await dataAccess.ExecuteScalarAsync<int>(FeatureSetQueryStatement, parameter);

                            if (featureSet.Features != null)
                            {
                                foreach (var item in featureSet.Features)
                                {
                                    var parameterfeature =AddFeatureSetFeature (InsertedFeatureSetId,item.Id);
                                }               
                            }
                            
            return InsertedFeatureSetId;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public int AddFeatureSetFeature(int featuresetid, int FeatureID)
        {
                var parameterfeature = new DynamicParameters();
                parameterfeature.Add("@feature_set_id", featuresetid);
                parameterfeature.Add("@feature_id", FeatureID);
                int resultAddFeatureSet =  dataAccess.Execute(@"INSERT INTO master.featuresetfeature(
                                                                feature_set_id, feature_id)
                                                                VALUES (@feature_set_id, @feature_id)",parameterfeature);
                return resultAddFeatureSet;
        }

        public async Task<int> UpdateFeatureSet(FeatureSet featureSet)
        {

            var FSQueryStatement = @" UPDATE dafconnectmaster.featureset 
                                 SET  featuresetdescription = @featuresetdescription
                                ,updateddate = @updateddate
                                ,updatedby = @updatedby
                                WHERE featuresetid = @featuresetid
                                RETURNING featuresetid;";
            var parameter = new DynamicParameters();
            parameter.Add("@featuresetid", featureSet.FeatureSetID);
            // parameter.Add("@featuresetdescription", featureSet.FeatureSetName);
            parameter.Add("@updateddate", DateTime.Now);
            //parameter.Add("@updatedby", featureSet.Updatedby);
            int UpdatedFeatureSetId = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);

            if (featureSet.Features != null)
            {
                var FSFQueryStatement = @"UPDATE dafconnectmaster.featuresetfeature 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE featuresetid = @featuresetid
                                    RETURNING featuresetid;";

                var FSFparameter = new DynamicParameters();
                FSFparameter.Add("@featuresetid", featureSet.FeatureSetID);
                FSFparameter.Add("@updateddate", DateTime.Now);
                //FSFparameter.Add("@updatedby", featureSet.Updatedby);
                FSFparameter.Add("@isactive", false);
                await dataAccess.ExecuteScalarAsync<int>(FSFQueryStatement, FSFparameter);

                foreach (var item in featureSet.Features)
                {
                    // item.FeatureSetID = featureSet.FeatureSetID;
                    item.Createddate = DateTime.Now;
                    item.is_active = true;
                }

                int resultUpdateSet = await dataAccess.ExecuteAsync("INSERT INTO dafconnectmaster.featuresetfeature (featuresetid,rolefeatureid,isactive,createddate,createdby,isrolefeatureenabled) VALUES (@FeatureSetID,@RoleFeatureId,@isactive,@createddate,@modifiedby,@IsRoleFeatureEnabled) RETURNING featuresetfeatureid", featureSet.Features);
                return resultUpdateSet;
            }
            return UpdatedFeatureSetId;
        }

        public async Task<int> DeleteFeatureSet(int FeatureSetId, int UserId)
        {
            var FSQueryStatement = @"UPDATE dafconnectmaster.featureset 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE featuresetid = @featuresetid
                                    RETURNING featuresetid;";
            var FSparameter = new DynamicParameters();
            FSparameter.Add("@featuresetid", FeatureSetId);
            FSparameter.Add("@updateddate", DateTime.Now);
            FSparameter.Add("@updatedby", UserId);
            FSparameter.Add("@isactive", false);
            await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, FSparameter);

            var FSFQueryStatement = @"UPDATE dafconnectmaster.featuresetfeature 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE featuresetid = @featuresetid
                                    RETURNING featuresetid;";

            int resultDeleteFeature = await dataAccess.ExecuteScalarAsync<int>(FSFQueryStatement, FSparameter);

            return resultDeleteFeature;
        }

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active)
        {

            var QueryStatement = @" SELECT id,
                                     name,
                                     description,
                                     is_active,
                                     is_custom_feature_set
	                                FROM master.featureset
                                    Where is_active= @Active
                                    and (id=@featuresetid OR @featuresetid=0)";

            var parameter = new DynamicParameters();
            parameter.Add("@featuresetid", FeatureSetId);
            parameter.Add("@Active", Active);
            IEnumerable<FeatureSet> FeatureSetDetails = await dataAccess.QueryAsync<FeatureSet>(QueryStatement, parameter);
            return FeatureSetDetails;

        }

        public async Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, char? Featuretype)
        {

            var QueryStatement = @"SELECT f.id, f.name, 
                                f.description, f.type, f.is_active,r.id as roleid  ,   r.organization_id                        
                                FROM master.feature f
								 join master.featuresetfeature fsf
								on fsf.feature_id= f.id
								 join master.Role r
								on r.feature_set_id = fsf.feature_set_id";
            
            
            var parameter = new DynamicParameters();
            if (RoleId > 0)
            {
                parameter.Add("@RoleId", RoleId);
                QueryStatement = QueryStatement + " and r.id  = @RoleId";

            }
            // organization id filter
            if (Organizationid > 0)
            {
                parameter.Add("@organization_id", Organizationid);
                QueryStatement = QueryStatement + " and r.organization_id  = @organization_id";

            }
            if (Featuretype != 0)
            {
                 parameter.Add("@type", Featuretype);
                QueryStatement = QueryStatement + " and f.type  = @type";

            }
            if(RoleId == 0  && Organizationid ==0)
            {
                 QueryStatement = @"SELECT f.id, f.name, 
                                f.description, f.type, f.is_active                       
                                FROM master.feature f where 1=1";
               if (Featuretype != 0)
                {
                    parameter.Add("@type", Featuretype);
                    QueryStatement = QueryStatement + " and f.type  = @type";

                }

            }

           
           IEnumerable<Feature> FeatureSetDetails = await dataAccess.QueryAsync<Feature>(QueryStatement, parameter);
            return FeatureSetDetails;

        }

         public async Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int feature_set_id)
         {
             var QueryStatement = @"SELECT feature_id as id
                                   FROM master.featuresetfeature
                                    Where feature_set_id = @feature_set_id
                                    ";

            var parameter = new DynamicParameters();
            parameter.Add("@feature_set_id", feature_set_id);
            IEnumerable<Feature> FeatureSetDetails = await dataAccess.QueryAsync<Feature>(QueryStatement, parameter);
            return FeatureSetDetails;


         }

        public async Task<int> CheckFeatureSetExist(string FeatureSetName)
        {
            var QueryStatement = @"SELECT CASE WHEN featuresetid IS NULL THEN 0 ELSE featuresetid END
                                    FROM dafconnectmaster.featureset 
                                    WHERE isactive=true
                                    AND LOWER(featuresetdescription) = LOWER(@featuresetdescription)";
            var parameter = new DynamicParameters();
            parameter.Add("@featuresetdescription", FeatureSetName);
            int resultFeatureName = await dataAccess.QueryFirstAsync<int>(QueryStatement, parameter);
            return resultFeatureName;
        }

        public async Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId)
        {
            var lookup = new Dictionary<int, FeatureSet>();
            await dataAccess.QueryAsync<FeatureSet, Feature, FeatureSet>(@"
                     SELECT FS.featuresetid
                    ,FS.featuresetdescription as FeatureSetName
                    ,FSF.featuresetfeatureid
                    ,FSF.rolefeatureid
                    ,FSF.isrolefeatureenabled
                    ,RF.rolefeaturetypeid
                    ,RF.featuredescription
                    ,RF.parentfeatureid
                    ,RF.ismenu
                    ,RF.seqnum
                    FROM dafconnectmaster.featureset FS
                    LEFT JOIN dafconnectmaster.featuresetfeature FSF ON FS.featuresetid=FSF.featuresetid
                    LEFT JOIN dafconnectmaster.rolefeature RF ON FSF.rolefeatureid = RF.rolefeatureid 
                    WHERE (FS.featuresetid=@FeatureSetId Or @FeatureSetId=0) AND RF.isactive=true AND FSF.isactive=true AND FS.isactive=true;
                    ", (c, l) =>
            {
                FeatureSet featureSet;
                if (!lookup.TryGetValue(c.FeatureSetID, out featureSet))
                    lookup.Add(c.FeatureSetID, featureSet = c);
                if (featureSet.Features == null)
                    featureSet.Features = new List<Feature>();
                featureSet.Features.Add(l); /* Add locations to course */
                return featureSet;
            }, new { featuresetid = @FeatureSetId }, splitOn: "featuresetid,rolefeatureid");

            var FeatureSetFeatureDetails = lookup.Values;
            return FeatureSetFeatureDetails;

        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.features.repository
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly FeatureCoreMapper _featureCoreMapper;
        public FeatureRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
            _featureCoreMapper = new FeatureCoreMapper();
        }

        #region Feature Set 


        public async Task<int> AddFeatureSet(FeatureSet featureSet)
        {
            try
            {
                var featureSetQueryStatement = @"INSERT INTO master.featureset(
                                                             name, description, state, created_at, created_by, modified_at, modified_by)
                                                            VALUES (@name, @description, @state,@created_at, @created_by,@modified_at,@modified_by)
                                                             RETURNING id";

                var parameter = new DynamicParameters();
                parameter.Add("@name", featureSet.Name);
                parameter.Add("@description", featureSet.Description);
                parameter.Add("@state", "A");
                parameter.Add("@created_at", featureSet.Created_at);
                parameter.Add("@created_by", featureSet.Created_by);
                parameter.Add("@modified_at", featureSet.Modified_at);
                parameter.Add("@modified_by", featureSet.Modified_by);

                int insertedFeatureSetId = await _dataAccess.ExecuteScalarAsync<int>(featureSetQueryStatement, parameter);

                if (featureSet.Features != null)
                {
                    foreach (var item in featureSet.Features)
                    {
                        var parameterfeature = AddFeatureSetFeature(insertedFeatureSetId, item.Id);
                    }
                }

                return insertedFeatureSetId;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<int> GetMinimumLevel(List<Feature> features)
        {
            string featureid = string.Join(",", features.Select(p => p.Id.ToString()));
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@featureid", featureid);
            string query = @"select  min(level) from master.feature where id in (" + featureid + ")";
            var minlevel = await _dataAccess.ExecuteScalarAsync<int>(query, parameterfeature);
            return minlevel;

        }

        public int AddFeatureSetFeature(int featuresetid, int featureID)
        {
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@feature_set_id", featuresetid);
            parameterfeature.Add("@feature_id", featureID);
            int resultAddFeatureSet = _dataAccess.Execute(@"INSERT INTO master.featuresetfeature(
                                                                feature_set_id, feature_id)
                                                                VALUES (@feature_set_id, @feature_id)", parameterfeature);
            return resultAddFeatureSet;
        }

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, char state)
        {
            List<FeatureSet> featuress = new List<FeatureSet>();
            var queryStatement = @" SELECT id,
                                     name,
                                     description,
                                     state
	                                FROM master.featureset
                                    Where state= @state
                                    and (id=@featuresetid OR @featuresetid=0)";

            var parameter = new DynamicParameters();
            parameter.Add("@featuresetid", featureSetId);
            parameter.Add("@state", state);
            IEnumerable<FeatureSet> featureSetDetails = await _dataAccess.QueryAsync<FeatureSet>(queryStatement, parameter);

            foreach (dynamic record in featureSetDetails)
            {

                featuress.Add(_featureCoreMapper.Map(record));
            }

            return featuress;

        }

        public async Task<IEnumerable<Feature>> GetFeatures(int roleId, int organizationId, int featureId, int level, char? featureType, string langaugeCode)
        {
            try
            {
                var features = new List<Feature>();

                var queryStatement = @"SELECT f.id, f.name, 
                                 f.type, f.state,f.data_attribute_set_id,f.key,r.id as roleid, r.organization_id                        
                                FROM master.feature f
								 join master.featuresetfeature fsf
								on fsf.feature_id= f.id
								 join master.Role r
								on r.feature_set_id = fsf.feature_set_id 
                                Left join translation.translation t
                                on f.Key = t.name and t.code=@Code
                                where f.state IN ('A', 'I')";


                var parameter = new DynamicParameters();
                if (roleId > 0)
                {
                    parameter.Add("@RoleId", roleId);
                    queryStatement = queryStatement + " and r.id  = @RoleId";

                }
                // organization id filter
                if (organizationId > 0)
                {
                    parameter.Add("@organization_id", organizationId);
                    queryStatement = queryStatement + " and r.organization_id  = @organization_id";

                }

                if (roleId == 0 && organizationId == 0)
                {
                    queryStatement = @"SELECT f.id, f.name,t.value, f.type, f.state, f.data_attribute_set_id, f.key, f.level, f.state
	                                FROM master.feature f 
									Left join translation.translation t
                                    on f.Key = t.name and t.code=@Code
                                    where f.state IN ('A', 'I')";

                    if (featureId > 0)
                    {
                        parameter.Add("@id", featureId);
                        queryStatement = queryStatement + " and f.id  = @id";
                    }
                    if (level > 0)
                    {
                        parameter.Add("@level", level);
                        queryStatement = queryStatement + " and f.level  >= @level";
                    }

                }

                parameter.Add("@Code", langaugeCode);
                IEnumerable<Feature> featureSetDetails = await _dataAccess.QueryAsync<Feature>(queryStatement, parameter);

                foreach (dynamic record in featureSetDetails)
                {
                    features.Add(_featureCoreMapper.MapFeature(record));
                }

                return features;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes(string langaugeCode)
        {

            var queryStatement = @"SELECT d.id, d.name,t.value, d.description, d.type, d.key
	                                FROM master.dataattribute d
									Left join translation.translation t
									on d.Key = t.name and t.code=@Code";
            var parameter = new DynamicParameters();
            parameter.Add("@Code", langaugeCode);
            IEnumerable<DataAttribute> dataAttributeDetails = await _dataAccess.QueryAsync<DataAttribute>(queryStatement, parameter);
            return dataAttributeDetails;

        }
        public async Task<DataAttributeSet> GetDataAttributeset(int dataAttributeSetID)
        {
            try
            {
                List<DataAttributeSet> dataAttributeSets = new List<DataAttributeSet>();
                var queryStatement = @"SELECT id, name, description, is_exlusive, created_at, created_by, modified_at, modified_by, state
	                                FROM master.dataattributeset where id= @data_set_id";

                var parameter = new DynamicParameters();
                parameter.Add("@data_set_id", dataAttributeSetID);
                var dataAttributeSetDetails = await _dataAccess.QueryAsync<DataAttributeSet>(queryStatement, parameter);

                foreach (dynamic record in dataAttributeSetDetails)
                {

                    dataAttributeSets.Add(_featureCoreMapper.MapDataAttributeSet(record));
                }


                var dataAttributeQuery = @"SELECT dsa.data_attribute_id as Id
	                                    FROM master.dataattributeset ds Left Join 
	                                    master.dataattributesetattribute dsa
	                                    on ds.id = dsa.data_attribute_set_id
	                                    where ds.id= @data_set_id";
                var parameters = new DynamicParameters();
                parameters.Add("@data_set_id", dataAttributeSetID);
                var dataAttributes = await _dataAccess.QueryAsync<DataAttribute>(dataAttributeQuery, parameters);
                var dataatribute = dataAttributeSets.FirstOrDefault();
                dataatribute.DataAttributes = new List<DataAttribute>();
                dataatribute.DataAttributes.AddRange(dataAttributes);
                return dataatribute;
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int feature_set_id, string langaugeCode)
        {
            var feature = new List<Feature>();
            var queryStatement = @"Select f.id,f.name,t.value,f.type,f.state,f.data_attribute_set_id,f.key,f.level,fs.feature_set_id from master.feature f
	                                Left join master.featuresetfeature fS
	                                on f.id=fs.feature_id
                                    Left join translation.translation t
                                    on f.Key = t.name and t.code=@Code
                                    Where fs.feature_set_id = @feature_set_id
                                    ";

            var parameter = new DynamicParameters();
            parameter.Add("@Code", langaugeCode);
            parameter.Add("@feature_set_id", feature_set_id);
            IEnumerable<Feature> featureSetDetails = await _dataAccess.QueryAsync<Feature>(queryStatement, parameter);


            foreach (dynamic record in featureSetDetails)
            {

                feature.Add(_featureCoreMapper.MapFeatureSetDetails(record));
            }


            return feature;


        }

        public async Task<int> CheckFeatureSetExist(string featureSetName)
        {
            var queryStatement = @"SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.featureset 
                                    WHERE state='A'
                                    AND LOWER(description) = LOWER(@featuresetdescription)";
            var parameter = new DynamicParameters();
            parameter.Add("@featuresetdescription", featureSetName);
            int resultFeatureName = await _dataAccess.QueryFirstAsync<int>(queryStatement, parameter);
            return resultFeatureName;
        }

        public async Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId)
        {
            var lookup = new Dictionary<int, FeatureSet>();
            await _dataAccess.QueryAsync<FeatureSet, Feature, FeatureSet>(@"
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
                if (!lookup.TryGetValue(c.FeatureSetID, out FeatureSet featureSet))
                    lookup.Add(c.FeatureSetID, featureSet = c);
                if (featureSet.Features == null)
                    featureSet.Features = new List<Feature>();
                featureSet.Features.Add(l); /* Add locations to course */
                return featureSet;
            }, new { featuresetid = @FeatureSetId }, splitOn: "featuresetid,rolefeatureid");

            var featureSetFeatureDetails = lookup.Values;
            return featureSetFeatureDetails;

        }

        #endregion
        public async Task<bool> DeleteFeatureSet(int featureSetId)
        {
            try
            {

                var fsQueryStatement = @"UPDATE master.featureset 
                                    SET  
                                    state = @state
                                    WHERE id = @featuresetid
                                    RETURNING id;";
                var fsparameter = new DynamicParameters();
                fsparameter.Add("@featuresetid", featureSetId);
                fsparameter.Add("@state", 'D');
                int deleteFeatureSetId = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, fsparameter);
                if (deleteFeatureSetId > 0)
                {
                    // var parameterfeature = RemoveFeatureSetMapping(FeatureSetId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // public async Task<bool> RemoveFeatureSetMapping(int FeatureSetId , List<int> IDs)
        public async Task<IEnumerable<Featuresetfeature>> RemoveFeatureSetMapping(int featureSetId, List<int> ids)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var test = string.Empty;

                    if (ids != null)
                    {
                        test = string.Join("' , '", ids);
                    }

                    var fsfSelectQueryStatement = @"select feature_id  FROM master.featuresetfeature  
                                    WHERE feature_set_id=  @featuresetid and feature_id not in ( '" + test + "');";

                    var fsparameter = new DynamicParameters();
                    fsparameter.Add("@featuresetid", featureSetId);
                    fsparameter.Add("@IDs", test);

                    IEnumerable<Featuresetfeature> featureSetDetails = await _dataAccess.QueryAsync<Featuresetfeature>(fsfSelectQueryStatement, fsparameter);

                    foreach (var item in featureSetDetails)
                    {

                        var fsfDeleteQueryStatement = @"DELETE FROM master.featuresetfeature  
                                        WHERE feature_set_id = @featuresetid AND feature_id=@resultSelectFeature
                                        RETURNING feature_set_id;";

                        fsparameter.Add("@resultSelectFeature", item.Feature_id);
                        int resultDeleteFeature = await _dataAccess.ExecuteScalarAsync<int>(fsfDeleteQueryStatement, fsparameter);
                    }
                    transactionScope.Complete();
                    return featureSetDetails;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> CheckDataAttributeSetExist(int id)
        {
            try
            {
                var queryStatement = @"SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.dataattributeset 
                                    WHERE id=@DataAttributeSetID";
                var parameter = new DynamicParameters();
                parameter.Add("@DataAttributeSetID", id);
                int result = await _dataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet)
        {
            try
            {
                int dataAttributeName = await (Task<int>)CheckDataAttributeSetExist(dataAttributeSet.ID);
                if (dataAttributeName > 0)
                {
                    var updateDataAttributID = await (Task<DataAttributeSet>)UpdatedataattributeSet(dataAttributeSet);
                    return updateDataAttributID;

                }
                else
                {

                    var featureSetQueryStatement = @"INSERT INTO master.dataattributeset(
                                                             name, description, is_exlusive, created_at, created_by, modified_at, modified_by,state)
                                                            VALUES (@name, @description, @is_exlusive,@created_at,@created_by,@modified_at,@modified_by,'A') RETURNING id";

                    var parameter = new DynamicParameters();
                    // parameter.Add("@dataattributesetID", DataAttributeSetID);
                    parameter.Add("@name", dataAttributeSet.Name);
                    parameter.Add("@description", dataAttributeSet.Description);
                    parameter.Add("@is_exlusive", dataAttributeSet.Is_exlusive);
                    parameter.Add("@created_at", dataAttributeSet.Created_at);
                    parameter.Add("@created_by", dataAttributeSet.Created_by);
                    parameter.Add("@modified_at", dataAttributeSet.Modified_at);
                    parameter.Add("@modified_by", dataAttributeSet.Modified_by);

                    int insertedDataAttributeSetID = await _dataAccess.ExecuteScalarAsync<int>(featureSetQueryStatement, parameter);
                    if (insertedDataAttributeSetID > 0)
                    {

                        dataAttributeSet.ID = insertedDataAttributeSetID;
                    }
                    List<int> temp = new List<int>();
                    foreach (var item in dataAttributeSet.DataAttributes)
                    {
                        temp.Add(item.ID);

                    }

                    if (dataAttributeSet.DataAttributes != null)
                    {
                        foreach (var item in dataAttributeSet.DataAttributes)
                        {
                            var mapdataattribute = CreateDataAttributeSetMapping(insertedDataAttributeSetID, item.ID);

                        }

                    }



                    return dataAttributeSet;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<int> CreateDataattributeSetFeature(Feature feature, int insertedDataAttributeSetID)
        {
            try
            {
                int maxSetFeatureID = GetMaxFeatureID();  // Dataattribute set ID will start from 10000
                var parameter = new DynamicParameters();
                parameter.Add("@id", maxSetFeatureID);
                parameter.Add("@name", feature.Name);
                parameter.Add("@type", 'D');
                parameter.Add("@state", (char)feature.FeatureState);
                parameter.Add("@data_attribute_set_id", insertedDataAttributeSetID);
                parameter.Add("@key", feature.Description);
                parameter.Add("@level", feature.Level);
                int resultAddFeatureSet = await _dataAccess.ExecuteScalarAsync<int>(@"INSERT INTO master.feature(
	                                                 id, name, type, state, data_attribute_set_id, key,level)
	                                           VALUES (@id, @name, @type, @state, @data_attribute_set_id, @key,@level) RETURNING id", parameter);
                return resultAddFeatureSet;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public int CreateDataAttributeSetMapping(int dataAttributeSetId, int id)
        {
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@data_Attribute_set_id", dataAttributeSetId);
            parameterfeature.Add("@data_Attribute_id", id);
            int resultAddFeatureSet = _dataAccess.Execute(@"INSERT INTO master.dataattributesetattribute(
                                                                data_attribute_set_id, data_attribute_id)
                                                                VALUES (@data_Attribute_set_id, @data_Attribute_id)", parameterfeature);
            return resultAddFeatureSet;
        }

        public int GetMaxFeatureID()
        {
            var parameterfeature = new DynamicParameters();
            int maxFeatureID = _dataAccess.ExecuteScalar<int>(@"select max(id)+1 as ID from master.Feature", parameterfeature);
            if (maxFeatureID < 10000)
            {
                maxFeatureID = 10000;
                return maxFeatureID;
            }
            else
            {
                return maxFeatureID;
            }
        }
        public async Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var fsQueryStatement = @" UPDATE master.dataattributeset 
                                 SET 
                                 name =@name
                                ,description = @description
                                ,is_exlusive = @is_exlusive
                                ,created_at = @created_at
                                ,created_by = @created_by
                                ,modified_at = @modified_at
                                ,modified_by = @modified_by
                                WHERE id = @id
                                RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", dataAttributeSet.ID);
                    parameter.Add("@name", dataAttributeSet.Name);
                    parameter.Add("@description", dataAttributeSet.Description);
                    parameter.Add("@is_exlusive", dataAttributeSet.Is_exlusive);
                    parameter.Add("@created_at", dataAttributeSet.Created_at);
                    parameter.Add("@created_by", dataAttributeSet.Created_by);
                    parameter.Add("@modified_at", dataAttributeSet.Modified_at);
                    parameter.Add("@modified_by", dataAttributeSet.Modified_by);
                    int updatedDataAttributeSetId = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, parameter);
                    if (updatedDataAttributeSetId > 0)
                    {
                        dataAttributeSet.ID = updatedDataAttributeSetId;
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in dataAttributeSet.DataAttributes)
                    {
                        temp.Add(item.ID);

                    }

                    var removeFeatureID = await RemoveDataAttributeSetMapping(updatedDataAttributeSetId, temp);

                    if (dataAttributeSet.DataAttributes != null)
                    {
                        foreach (var item in dataAttributeSet.DataAttributes)
                        {
                            var parameterfeature = UpdateDataAttributeSetMapping(updatedDataAttributeSetId, item.ID);
                        }
                    }
                    Feature feature = new Feature();
                    int mapDataAttributeSetID = UpdatedataattributeSetFeature(feature, updatedDataAttributeSetId);

                    transactionScope.Complete();

                    return dataAttributeSet;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Feature> UpdateFeature(Feature feature)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var fsQueryStatement = @" UPDATE master.dataattributeset 
                                 SET 
                                 name =@name
                                ,description = @description
                                ,is_exlusive = @is_exlusive
                                ,created_at = @created_at
                                ,created_by = @created_by
                                ,modified_at = @modified_at
                                ,modified_by = @modified_by
                                WHERE id = @id
                                RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", feature.DataAttributeSets.ID);
                    parameter.Add("@name", feature.DataAttributeSets.Name);
                    parameter.Add("@description", feature.DataAttributeSets.Description);
                    parameter.Add("@is_exlusive", feature.DataAttributeSets.Is_exlusive);
                    parameter.Add("@created_at", feature.DataAttributeSets.Created_at);
                    parameter.Add("@created_by", feature.DataAttributeSets.Created_by);
                    parameter.Add("@modified_at", feature.DataAttributeSets.Modified_at);
                    parameter.Add("@modified_by", feature.DataAttributeSets.Modified_by);
                    int updatedDataAttributeSetId = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, parameter);
                    if (updatedDataAttributeSetId > 0)
                    {
                        feature.DataAttributeSets.ID = updatedDataAttributeSetId;
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in feature.DataAttributeSets.DataAttributes)
                    {
                        temp.Add(item.ID);

                    }

                    var removeFeatureID = await RemoveDataAttributeSetMapping(updatedDataAttributeSetId, temp);

                    if (feature.DataAttributeSets.DataAttributes != null)
                    {
                        foreach (var item in feature.DataAttributeSets.DataAttributes)
                        {
                            var parameterfeature = UpdateDataAttributeSetMapping(updatedDataAttributeSetId, item.ID);
                        }
                    }
                    //Feature features = new Feature();
                    int mapDataAttributeSetID = UpdatedataattributeSetFeature(feature, updatedDataAttributeSetId);

                    transactionScope.Complete();

                    return feature;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int UpdatedataattributeSetFeature(Feature feature, int updatedDataAttributeSetId)
        {
            int maxSetFeatureID = GetMaxFeatureID();  // Dataattribute set ID will start from 10000
            var parameter = new DynamicParameters();
            //parameter.Add("@id", maxSetFeatureID);
            parameter.Add("@name", feature.Name);
            parameter.Add("@id", feature.Id);
            parameter.Add("@data_attribute_set_id", updatedDataAttributeSetId);
            parameter.Add("@key", feature.Key);
            parameter.Add("@State", (char)feature.FeatureState);

            int resultUpdateDataAttributeFeature = _dataAccess.Execute(@"UPDATE master.feature
	                                                SET 
                                                        name= @name,                                                       
                                                        key= @key,    
                                                        state= @State
	                                                WHERE data_attribute_set_id = @data_attribute_set_id", parameter);
            return resultUpdateDataAttributeFeature;
        }
        public int UpdateDataAttributeSetMapping(int dataAttributeSetId, int id)
        {

            int mapdataAttributesetvalue = CheckDataAttributeSetMappingExist(dataAttributeSetId, id);

            if (mapdataAttributesetvalue == 0)
            {
                // insert
                var parameterfeature = CreateDataAttributeSetMapping(dataAttributeSetId, id);
            }

            return dataAttributeSetId;


        }

        public async Task<bool> DeleteDataAttribute(int dataAttributeSetID)
        {
            try
            {
                if (dataAttributeSetID != 0)
                {
                    //int MapDataAttributeSetID = RemoveDataAttributeSetMapping(dataAttributeSetID);
                    int mapDataAttributeSetIDFeature = RemoveDataAttributeSetMappingWithFeature(dataAttributeSetID);
                    var fsQueryStatement = @"update master.dataattributeset set state='D' where id = @dataAttributeSetID  RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@dataAttributeSetID", dataAttributeSetID);
                    int deleteDataAttributeSetId = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, parameter);

                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public int RemoveDataAttributeSetMapping( int dataAttributeSetID , List<int> IDs)
        public async Task<IEnumerable<DataAttributeSetAttribute>> RemoveDataAttributeSetMapping(int dataAttributeSetID, List<int> ids)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var test = string.Empty;

                    if (ids != null)
                    {
                        test = string.Join("' , '", ids);
                    }

                    var fsfSelectQueryStatement = @"select data_attribute_id  FROM master.dataattributesetattribute  
                                    WHERE data_attribute_set_id=  @dataAttributeSetID and data_attribute_id not in ( '" + test + "');";

                    var fsParameter = new DynamicParameters();
                    fsParameter.Add("@dataAttributeSetID", dataAttributeSetID);
                    fsParameter.Add("@IDs", test);

                    IEnumerable<DataAttributeSetAttribute> dataAttributeSetDetails = await _dataAccess.QueryAsync<DataAttributeSetAttribute>(fsfSelectQueryStatement, fsParameter);


                    foreach (var item in dataAttributeSetDetails)
                    {

                        var fsfDeleteQueryStatement = @"DELETE FROM master.dataattributesetattribute  
                                        WHERE data_attribute_set_id = @dataAttributeSetID AND data_attribute_id=@resultSelectFeature
                                        RETURNING data_attribute_id;";

                        fsParameter.Add("@resultSelectFeature", item.Data_attribute_id);
                        int resultDeleteFeature = await _dataAccess.ExecuteScalarAsync<int>(fsfDeleteQueryStatement, fsParameter);
                    }
                    transactionScope.Complete();
                    return dataAttributeSetDetails;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public int RemoveDataAttributeSetMappingWithFeature(int dataAttributeSetID)
        {
            if (dataAttributeSetID != 0)
            {
                var fsQueryStatement = @" update master.feature set state = 'I' where data_attribute_set_id = @dataAttributeSetID  RETURNING data_attribute_set_id;";
                var parameter = new DynamicParameters();
                parameter.Add("@dataAttributeSetID", dataAttributeSetID);
                int result = _dataAccess.ExecuteScalar<int>(fsQueryStatement, parameter);
                return result;
            }
            return dataAttributeSetID;
        }

        public async Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet)
        {
            try
            {

                var featureSetQueryStatement = @"INSERT INTO master.featureset(
                                                             name, description, state, created_at, created_by, modified_at, modified_by)
                                                            VALUES (@name, @description, @state,@created_at, @created_by,@modified_at,@modified_by)
                                                             RETURNING id";

                var parameter = new DynamicParameters();
                parameter.Add("@name", featureSet.Name);
                parameter.Add("@description", featureSet.Description);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", featureSet.Created_at);
                parameter.Add("@created_by", featureSet.Created_by);
                parameter.Add("@modified_at", featureSet.Modified_at);
                parameter.Add("@modified_by", featureSet.Modified_by);

                int insertedFeatureSetId = await _dataAccess.ExecuteScalarAsync<int>(featureSetQueryStatement, parameter);
                if (insertedFeatureSetId > 0)
                {
                    featureSet.FeatureSetID = insertedFeatureSetId;
                }
                List<int> temp = new List<int>();
                if (featureSet.Features != null)
                {
                    foreach (var item in featureSet.Features)
                    {
                        temp.Add(item.Id);

                    }


                    foreach (var item in featureSet.Features)
                    {
                        var parameterfeature = CreateFeatureSetMapping(insertedFeatureSetId, item.Id);
                    }
                }

                return featureSet;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public int CreateFeatureSetMapping(int featuresetid, int featureID)
        {
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@feature_set_id", featuresetid);
            parameterfeature.Add("@feature_id", featureID);
            int resultAddFeatureSet = _dataAccess.Execute(@"INSERT INTO master.featuresetfeature(
                                                                feature_set_id, feature_id)
                                                                VALUES (@feature_set_id, @feature_id)", parameterfeature);
            return resultAddFeatureSet;
        }

        //GetDataAttributeSetDetails

        public async Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetId)
        {
            try
            {
                var queryStatement = @"select id, name, description, is_exlusive, created_at, created_by, modified_at, modified_by
                                        FROM master.dataattributeset
                                        where id =@DataAttributeSetId";
                var parameter = new DynamicParameters();

                parameter.Add("@DataAttributeSetId", dataAttributeSetId);
                dynamic dataattributeset = await _dataAccess.QueryAsync<dynamic>(queryStatement, parameter);

                List<DataAttributeSet> dataattributesetList = new List<DataAttributeSet>();
                foreach (dynamic record in dataattributeset)
                {
                    dataattributesetList.Add(Map(record));
                }
                return dataattributesetList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private DataAttributeSet Map(dynamic record)
        {
            DataAttributeSet entity = new DataAttributeSet();
            entity.ID = record.id;
            entity.Name = record.name;
            entity.Description = record.description;
            entity.Is_exlusive = record.is_exlusive;
            entity.Created_at = record.created_at;
            entity.Created_by = record.created_by;
            entity.Modified_at = record.modified_at;
            entity.Modified_by = record.modified_by;
            return entity;
        }

        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var fsQueryStatement = @" UPDATE master.featureset 
                                 SET 
                                    name= @name,
                                    description= @description, 
                                    modified_at=@modified_at,
                                    modified_by=@modified_by
                                WHERE id = @id
                                RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", featureSet.FeatureSetID);
                    parameter.Add("@name", featureSet.Name);
                    parameter.Add("@description", featureSet.Description);
                    parameter.Add("@modified_at", featureSet.Modified_at);
                    parameter.Add("@modified_by", featureSet.Modified_by);
                    int updateFeatureSetID = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, parameter);
                    if (updateFeatureSetID > 0)
                    {
                        featureSet.FeatureSetID = updateFeatureSetID;
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in featureSet.Features)
                    {
                        temp.Add(item.Id);

                    }
                    var removeFeatureID = await RemoveFeatureSetMapping(updateFeatureSetID, temp);

                    if (featureSet.Features != null)
                    {
                        foreach (var item in featureSet.Features)
                        {
                            //if (featureSet.Is_Active == true)
                            //{
                            var parameterfeature = UpdateFeatureSetMapping(updateFeatureSetID, item.Id);
                            //}
                        }
                    }
                    transactionScope.Complete();
                    return featureSet;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateFeatureSetMapping(int updateFeatureSetID, int id)
        {
            int mapfeaturesetvalue = CheckFeatureSetMappingExist(updateFeatureSetID, id);

            if (mapfeaturesetvalue == 0)
            {
                // insert
                var parameterfeature = CreateFeatureSetMapping(updateFeatureSetID, id);
            }

            return await Task.FromResult(updateFeatureSetID);
        }

        public int CheckFeatureSetMappingExist(int updateFeatureSetID, int id)
        {
            try
            {
                var queryStatement = @" SELECT CASE WHEN feature_set_id IS NULL THEN 0 ELSE feature_set_id END
                                    FROM master.featuresetfeature
                                    WHERE feature_set_id=@UpdateFeatureSetID
                                    AND feature_id = @ID
                                     ";
                var parameter = new DynamicParameters();
                parameter.Add("@UpdateFeatureSetID", updateFeatureSetID);
                parameter.Add("@ID", id);
                int resultFeatureName = _dataAccess.ExecuteScalar<int>(queryStatement, parameter);
                return resultFeatureName;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int CheckDataAttributeSetMappingExist(int updateDataAttributeSetID, int id)
        {
            try
            {
                var queryStatement = @" SELECT CASE WHEN data_attribute_set_id IS NULL THEN 0 ELSE data_attribute_set_id END
                                    FROM master.dataattributesetattribute
                                    WHERE data_attribute_set_id=@UpdateDataAttributeSetID
                                    AND data_attribute_id = @ID
                                     ";
                var parameter = new DynamicParameters();
                parameter.Add("@UpdateDataAttributeSetID", updateDataAttributeSetID);
                parameter.Add("@ID", id);
                int result = _dataAccess.ExecuteScalar<int>(queryStatement, parameter);
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<int> DeleteFeature(int featureId)
        {
            try
            {
                if (featureId != 0)
                {
                    var fsQueryStatement = @"update master.feature set state= @state where id=@id  and type= 'D' RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", featureId);
                    parameter.Add("@state", 'D');
                    int deleteDataAttributeSetFeatureID = await _dataAccess.ExecuteScalarAsync<int>(fsQueryStatement, parameter);
                    return deleteDataAttributeSetFeatureID;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CheckFeatureNameExist(string featureName, int featureId)
        {
            var queryStatement = @"SELECT id
                                    FROM master.feature 
                                    WHERE state='A'
                                    AND LOWER(name) = LOWER(@roleName)";
            var parameter = new DynamicParameters();

            parameter.Add("@roleName", featureName.Trim());
            if (featureId > 0)
            {
                parameter.Add("@featureid", featureId);
                queryStatement = queryStatement + " and id != @featureid";
            }
            int resultRoleId = _dataAccess.ExecuteScalar<int>(queryStatement, parameter);
            return resultRoleId;

        }


        public async Task<int> ChangeFeatureState(int featureID, Char state)
        {
            var queryStatement = @"Update master.feature set state=@state
                                    where id=@featureid returning id";
            var parameter = new DynamicParameters();

            parameter.Add("@featureid", featureID);
            parameter.Add("@state", state);


            int resultfeatureid = await _dataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
            return resultfeatureid;

        }


    }
}

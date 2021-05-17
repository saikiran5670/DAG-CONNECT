using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.features.entity;
using System.Transactions;

namespace net.atos.daf.ct2.features.repository
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly IDataAccess dataAccess;
        private readonly FeatureCoreMapper _featureCoreMapper;
        public FeatureRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
            _featureCoreMapper = new FeatureCoreMapper();
        }

        #region Feature Set 


        public async Task<int> AddFeatureSet(FeatureSet featureSet)
        {
            try
            {
                var FeatureSetQueryStatement = @"INSERT INTO master.featureset(
                                                             name, description, state, created_at, created_by, modified_at, modified_by)
                                                            VALUES (@name, @description, @state,@created_at, @created_by,@modified_at,@modified_by)
                                                             RETURNING id";

                            var parameter = new DynamicParameters();
                            parameter.Add("@name", featureSet.Name);
                            parameter.Add("@description", featureSet.description);
                            parameter.Add("@state", "A");
                            parameter.Add("@created_at", featureSet.created_at);
                            parameter.Add("@created_by", featureSet.created_by);
                            parameter.Add("@modified_at", featureSet.modified_at);
                            parameter.Add("@modified_by", featureSet.modified_by);
                           
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

        public async Task<int> GetMinimumLevel(List<Feature> features)
        {
             string featureid= string.Join(",", features.Select(p => p.Id.ToString()));
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@featureid", featureid);
            string query = @"select  min(level) from master.feature where id in (" + featureid +")";
             var minlevel= await dataAccess.ExecuteScalarAsync<int>(query, parameterfeature);
            return minlevel;

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

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId, char state)
        {
            List<FeatureSet> featuress = new List<FeatureSet>();
            var QueryStatement = @" SELECT id,
                                     name,
                                     description,
                                     state
	                                FROM master.featureset
                                    Where state= @state
                                    and (id=@featuresetid OR @featuresetid=0)";

            var parameter = new DynamicParameters();
            parameter.Add("@featuresetid", FeatureSetId);
            parameter.Add("@state", state);
            IEnumerable<FeatureSet> FeatureSetDetails = await dataAccess.QueryAsync<FeatureSet>(QueryStatement, parameter);

            foreach (dynamic record in FeatureSetDetails)
            {

                featuress.Add(_featureCoreMapper.Map(record));
            }

            return featuress;

        }

        public async Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid,int FeatureId,int level, char? Featuretype,string Langaugecode)
        {
            var features = new List<Feature>();

            var QueryStatement = @"SELECT f.id, f.name, 
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
                 QueryStatement = @"SELECT f.id, f.name,t.value, f.type, f.state, f.data_attribute_set_id, f.key, f.level, f.state
	                                FROM master.feature f 
									Left join translation.translation t
                                    on f.Key = t.name and t.code=@Code
                                    where f.state IN ('A', 'I')";
               if (Featuretype != '0')
                {
                    parameter.Add("@type", Featuretype);
                    QueryStatement = QueryStatement + " and f.type  = @type";

                }
                if (FeatureId > 0)
                {
                    parameter.Add("@id", FeatureId);
                    QueryStatement = QueryStatement + " and f.id  = @id";
                }
                if (level > 0)
                {
                    parameter.Add("@level", level);
                    QueryStatement = QueryStatement + " and f.level  >= @level";
                }

            }

            parameter.Add("@Code", Langaugecode);
            IEnumerable<Feature> FeatureSetDetails = await dataAccess.QueryAsync<Feature>(QueryStatement, parameter);

            foreach(dynamic record in FeatureSetDetails)
            {
                features.Add(_featureCoreMapper.MapFeature(record));
            }

            return features;

        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes(string Langaugecode)
        {

            var QueryStatement = @"SELECT d.id, d.name,t.value, d.description, d.type, d.key
	                                FROM master.dataattribute d
									Left join translation.translation t
									on d.Key = t.name and t.code=@Code";
            var parameter = new DynamicParameters();
            parameter.Add("@Code", Langaugecode);
            IEnumerable<DataAttribute> DataAttributeDetails = await dataAccess.QueryAsync<DataAttribute>(QueryStatement,parameter);
            return DataAttributeDetails;

        }
        public async Task<DataAttributeSet> GetDataAttributeset(int DataAttributeSetID)
        {
            try
            {
                List<DataAttributeSet> dataAttributeSets = new List<DataAttributeSet>();
                var QueryStatement = @"SELECT id, name, description, is_exlusive, created_at, created_by, modified_at, modified_by, state
	                                FROM master.dataattributeset where id= @data_set_id";

                var parameter = new DynamicParameters();
                parameter.Add("@data_set_id", DataAttributeSetID);
                var DataAttributeSetDetails = await dataAccess.QueryAsync<DataAttributeSet>(QueryStatement, parameter);

                foreach (dynamic record in DataAttributeSetDetails)
                {

                    dataAttributeSets.Add(_featureCoreMapper.MapDataAttributeSet(record));
                }


                var Dataattributequery = @"SELECT dsa.data_attribute_id as Id
	                                    FROM master.dataattributeset ds Left Join 
	                                    master.dataattributesetattribute dsa
	                                    on ds.id = dsa.data_attribute_set_id
	                                    where ds.id= @data_set_id";
                var parameters = new DynamicParameters();
                parameters.Add("@data_set_id", DataAttributeSetID);
                var DataAttributeS = await dataAccess.QueryAsync<DataAttribute>(Dataattributequery, parameters);
                var dataatribute = dataAttributeSets.FirstOrDefault();
                dataatribute.DataAttributes = new List<DataAttribute>();
                dataatribute.DataAttributes.AddRange(DataAttributeS);
                return dataatribute;
            }
            catch (Exception)
            {

                throw;
            }

            

        }

        public async Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int feature_set_id,string Langaugecode)
         {
            var feature = new List<Feature>();
             var QueryStatement = @"Select f.id,f.name,t.value,f.type,f.state,f.data_attribute_set_id,f.key,f.level,fs.feature_set_id from master.feature f
	                                Left join master.featuresetfeature fS
	                                on f.id=fs.feature_id
                                    Left join translation.translation t
                                    on f.Key = t.name and t.code=@Code
                                    Where fs.feature_set_id = @feature_set_id
                                    ";

            var parameter = new DynamicParameters();
            parameter.Add("@Code", Langaugecode);
            parameter.Add("@feature_set_id", feature_set_id);
            IEnumerable<Feature> FeatureSetDetails = await dataAccess.QueryAsync<Feature>(QueryStatement, parameter);


            foreach (dynamic record in FeatureSetDetails)
            {

                feature.Add(_featureCoreMapper.MapFeatureSetDetails(record));
            }


            return feature;


         }

        public async Task<int> CheckFeatureSetExist(string FeatureSetName)
        {
            var QueryStatement = @"SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.featureset 
                                    WHERE state='A'
                                    AND LOWER(description) = LOWER(@featuresetdescription)";
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
        public async Task<bool> DeleteFeatureSet(int FeatureSetId)
        {
            try
            {

            var FSQueryStatement = @"UPDATE master.featureset 
                                    SET  
                                    state = @state
                                    WHERE id = @featuresetid
                                    RETURNING id;";
            var FSparameter = new DynamicParameters();
            FSparameter.Add("@featuresetid", FeatureSetId);
            FSparameter.Add("@state", 'D');
            int DeleteFeatureSetId = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, FSparameter);
                if (DeleteFeatureSetId > 0)
                {
                   // var parameterfeature = RemoveFeatureSetMapping(FeatureSetId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex ;
            }
        }
       // public async Task<bool> RemoveFeatureSetMapping(int FeatureSetId , List<int> IDs)
        public async Task<IEnumerable<featuresetfeature>> RemoveFeatureSetMapping(int FeatureSetId, List<int> IDs)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var test = string.Empty;

                    if (IDs != null)
                    {
                        test =  string.Join("' , '", IDs) ;
                    }

                    var FSFSelectQueryStatement = @"select feature_id  FROM master.featuresetfeature  
                                    WHERE feature_set_id=  @featuresetid and feature_id not in ( '" + test + "');";

                    var FSparameter = new DynamicParameters();
                    FSparameter.Add("@featuresetid", FeatureSetId);
                    FSparameter.Add("@IDs", test);



                    IEnumerable<featuresetfeature> FeatureSetDetails = await dataAccess.QueryAsync<featuresetfeature>(FSFSelectQueryStatement, FSparameter);
                    // await dataAccess.ExecuteScalarAsync<int>(FSFSelectQueryStatement, FSparameter);

                    foreach (var item in FeatureSetDetails)
                    {

                        var FSFDeleteQueryStatement = @"DELETE FROM master.featuresetfeature  
                                        WHERE feature_set_id = @featuresetid AND feature_id=@resultSelectFeature
                                        RETURNING feature_set_id;";

                        FSparameter.Add("@resultSelectFeature", item.feature_id);
                        int resultDeleteFeature = await dataAccess.ExecuteScalarAsync<int>(FSFDeleteQueryStatement, FSparameter);
                    }
                    transactionScope.Complete();
                    return FeatureSetDetails;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> CheckDataAttributeSetExist(int ID)
        {
            try
            {
                //var QueryStatement = @"SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                //                    FROM master.dataattributeset 
                //                    WHERE id=@DataAttributeSetID  RETURNING id;";
                var QueryStatement = @"SELECT CASE WHEN id IS NULL THEN 0 ELSE id END
                                    FROM master.dataattributeset 
                                    WHERE id=@DataAttributeSetID";
                var parameter = new DynamicParameters();
                parameter.Add("@DataAttributeSetID", ID);
                int result =await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet)
        {
            try
            {
                int DataattributeName = await (Task<int>) CheckDataAttributeSetExist(dataAttributeSet.ID);
                if (DataattributeName > 0)
                {
                   var UpdatedataattributID = await (Task<DataAttributeSet>) UpdatedataattributeSet(dataAttributeSet);
                   return UpdatedataattributID;

                }
                else
                {
                
                var FeatureSetQueryStatement = @"INSERT INTO master.dataattributeset(
                                                             name, description, is_exlusive, created_at, created_by, modified_at, modified_by,state)
                                                            VALUES (@name, @description, @is_exlusive,@created_at,@created_by,@modified_at,@modified_by,'A') RETURNING id";

                            var parameter = new DynamicParameters();
                           // parameter.Add("@dataattributesetID", DataAttributeSetID);
                            parameter.Add("@name", dataAttributeSet.Name);
                            parameter.Add("@description", dataAttributeSet.Description);
                            parameter.Add("@is_exlusive", dataAttributeSet.Is_exlusive);
                            parameter.Add("@created_at", dataAttributeSet.created_at);
                            parameter.Add("@created_by", dataAttributeSet.created_by);
                            parameter.Add("@modified_at", dataAttributeSet.modified_at);
                            parameter.Add("@modified_by", dataAttributeSet.modified_by);
                           
                            int InserteddataAttributeSetID = await dataAccess.ExecuteScalarAsync<int>(FeatureSetQueryStatement, parameter);
                    if (InserteddataAttributeSetID > 0)
                    {

                        dataAttributeSet.ID = InserteddataAttributeSetID;
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
                                var mapdataattribute = CreateDataAttributeSetMapping(InserteddataAttributeSetID,item.ID);

                            }
                      
                    }



                    return dataAttributeSet;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
         public async Task<int> CreateDataattributeSetFeature(Feature feature, int InserteddataAttributeSetID)
        {
            int MaxSetFeatureID = GetMaxFeatureID();  // Dataattribute set ID will start from 10000
            var parameter = new DynamicParameters();
                            parameter.Add("@id", MaxSetFeatureID);
                            parameter.Add("@name", feature.Name);
                            parameter.Add("@type", 'D');
                            parameter.Add("@state", 'A');
                            parameter.Add("@data_attribute_set_id", InserteddataAttributeSetID);
                            parameter.Add("@key", feature.Description);
                            parameter.Add("@level", feature.Level);
            int resultAddFeatureSet = await dataAccess.ExecuteScalarAsync<int>(@"INSERT INTO master.feature(
	                                                 id, name, type, state, data_attribute_set_id, key,level)
	                                           VALUES (@id, @name, @type, @state, @data_attribute_set_id, @key,@level) RETURNING id", parameter);
                                        return resultAddFeatureSet;
        }

         public int CreateDataAttributeSetMapping(int DataAttributeSetId, int ID)
        {
            int resultAddFeatureSet = 0;
            
                var parameterfeature = new DynamicParameters();
                parameterfeature.Add("@data_Attribute_set_id", DataAttributeSetId);
                parameterfeature.Add("@data_Attribute_id", ID);
                 resultAddFeatureSet = dataAccess.Execute(@"INSERT INTO master.dataattributesetattribute(
                                                                data_attribute_set_id, data_attribute_id)
                                                                VALUES (@data_Attribute_set_id, @data_Attribute_id)", parameterfeature);
            
            return resultAddFeatureSet;
        }

        public int GetMaxFeatureID ()
        {
            var parameterfeature = new DynamicParameters();
            int MaxFeatureID = dataAccess.ExecuteScalar<int>(@"select max(id)+1 as ID from master.Feature", parameterfeature);
            if (MaxFeatureID < 10000)
            {
                MaxFeatureID = 10000;
                return MaxFeatureID;
            }
            else
            {
               return MaxFeatureID;
            }
        }
        public async Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet )
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var FSQueryStatement = @" UPDATE master.dataattributeset 
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
                    parameter.Add("@created_at", dataAttributeSet.created_at);
                    parameter.Add("@created_by", dataAttributeSet.created_by);
                    parameter.Add("@modified_at", dataAttributeSet.modified_at);
                    parameter.Add("@modified_by", dataAttributeSet.modified_by);
                    int UpdatedDataAttributeSetId = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);
                    if (UpdatedDataAttributeSetId > 0)
                    {
                        dataAttributeSet.ID = UpdatedDataAttributeSetId;
                        // var mapdataattribute = RemoveDataAttributeSetMapping(UpdatedDataAttributeSetId);
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in dataAttributeSet.DataAttributes)
                    {
                        temp.Add(item.ID);

                    }

                    var removeFeatureID = await RemoveDataAttributeSetMapping(UpdatedDataAttributeSetId, temp);

                    if (dataAttributeSet.DataAttributes != null)
                    {
                        foreach (var item in dataAttributeSet.DataAttributes)
                        {
                            var parameterfeature = UpdateDataAttributeSetMapping(UpdatedDataAttributeSetId, item.ID);
                        }
                    }
                    Feature feature = new Feature();
                    int MapDataAttributeSetID = UpdatedataattributeSetFeature(feature, UpdatedDataAttributeSetId);

                    transactionScope.Complete();

                    return dataAttributeSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Feature> UpdateFeature(Feature feature)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var FSQueryStatement = @" UPDATE master.dataattributeset 
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
                    parameter.Add("@created_at", feature.DataAttributeSets.created_at);
                    parameter.Add("@created_by", feature.DataAttributeSets.created_by);
                    parameter.Add("@modified_at", feature.DataAttributeSets.modified_at);
                    parameter.Add("@modified_by", feature.DataAttributeSets.modified_by);
                    int UpdatedDataAttributeSetId = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);
                    if (UpdatedDataAttributeSetId > 0)
                    {
                        feature.DataAttributeSets.ID = UpdatedDataAttributeSetId;
                        // var mapdataattribute = RemoveDataAttributeSetMapping(UpdatedDataAttributeSetId);
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in feature.DataAttributeSets.DataAttributes)
                    {
                        temp.Add(item.ID);

                    }

                    var removeFeatureID = await RemoveDataAttributeSetMapping(UpdatedDataAttributeSetId, temp);

                    if (feature.DataAttributeSets.DataAttributes != null)
                    {
                        foreach (var item in feature.DataAttributeSets.DataAttributes)
                        {
                            var parameterfeature = UpdateDataAttributeSetMapping(UpdatedDataAttributeSetId, item.ID);
                        }
                    }
                    //Feature features = new Feature();
                    int MapDataAttributeSetID = UpdatedataattributeSetFeature(feature, UpdatedDataAttributeSetId);

                    transactionScope.Complete();

                    return feature;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int UpdatedataattributeSetFeature(Feature feature, int UpdatedDataAttributeSetId)
        {
            int MaxSetFeatureID = GetMaxFeatureID();  // Dataattribute set ID will start from 10000
            var parameter = new DynamicParameters();
            //parameter.Add("@id", MaxSetFeatureID);
            parameter.Add("@name", feature.Name);
            parameter.Add("@id", feature.Id);
            parameter.Add("@data_attribute_set_id", UpdatedDataAttributeSetId);
            parameter.Add("@key", feature.Key);
            parameter.Add("@level", feature.Level);
            parameter.Add("@State", (char)feature.FeatureState);

            int resultUpdateDataAttributeFeature = dataAccess.Execute(@"UPDATE master.feature
	                                                SET 
                                                        name= @name,                                                       
                                                        key= @key,
                                                        level= @level,      
                                                        state= @State
	                                                WHERE data_attribute_set_id = @data_attribute_set_id", parameter);
            return resultUpdateDataAttributeFeature;
        }
        public int UpdateDataAttributeSetMapping(int DataAttributeSetId, int ID)
        {

            int mapdataAttributesetvalue = CheckDataAttributeSetMappingExist(DataAttributeSetId, ID);

            if (mapdataAttributesetvalue == 0)
            {
                // insert
                var parameterfeature = CreateDataAttributeSetMapping(DataAttributeSetId, ID);
            }

            return DataAttributeSetId;

           
        }

        public async Task<bool> DeleteDataAttribute(int dataAttributeSetID)
        {
            try
            {
                if (dataAttributeSetID != 0)
                {
                    //int MapDataAttributeSetID = RemoveDataAttributeSetMapping(dataAttributeSetID);
                    int MapDataAttributeSetIDFeature = RemoveDataAttributeSetMappingWithFeature(dataAttributeSetID);
                    var FSQueryStatement = @"update master.dataattributeset set state='D' where id = @dataAttributeSetID  RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@dataAttributeSetID", dataAttributeSetID);
                    int DeleteDataAttributeSetId = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);
                   
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public int RemoveDataAttributeSetMapping( int dataAttributeSetID , List<int> IDs)
         public async Task<IEnumerable<DataAttributeSetAttribute>> RemoveDataAttributeSetMapping(int dataAttributeSetID, List<int> IDs)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var test = string.Empty;

                    if (IDs != null)
                    {
                        test = string.Join("' , '", IDs);
                    }

                    var FSFSelectQueryStatement = @"select data_attribute_id  FROM master.dataattributesetattribute  
                                    WHERE data_attribute_set_id=  @dataAttributeSetID and data_attribute_id not in ( '" + test + "');";

                    var FSparameter = new DynamicParameters();
                    FSparameter.Add("@dataAttributeSetID", dataAttributeSetID);
                    FSparameter.Add("@IDs", test);

                    IEnumerable<DataAttributeSetAttribute> DataAttributeSetDetails = await dataAccess.QueryAsync<DataAttributeSetAttribute>(FSFSelectQueryStatement, FSparameter);


                    foreach (var item in DataAttributeSetDetails)
                    {

                        var FSFDeleteQueryStatement = @"DELETE FROM master.dataattributesetattribute  
                                        WHERE data_attribute_set_id = @dataAttributeSetID AND data_attribute_id=@resultSelectFeature
                                        RETURNING data_attribute_id;";

                        FSparameter.Add("@resultSelectFeature", item.data_attribute_id);
                        int resultDeleteFeature = await dataAccess.ExecuteScalarAsync<int>(FSFDeleteQueryStatement, FSparameter);
                    }
                    transactionScope.Complete();
                    return DataAttributeSetDetails;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int RemoveDataAttributeSetMappingWithFeature(int dataAttributeSetID)
        {
            if (dataAttributeSetID != 0)
            {
                var FSQueryStatement = @" update master.feature set state = 'I' where data_attribute_set_id = @dataAttributeSetID  RETURNING data_attribute_set_id;";
                var parameter = new DynamicParameters();
                parameter.Add("@dataAttributeSetID", dataAttributeSetID);
                int result = dataAccess.ExecuteScalar<int>(FSQueryStatement, parameter);
                return result;
            }
            return dataAttributeSetID;
        }

        public async Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet)
        {
            try
            {
                
                var FeatureSetQueryStatement = @"INSERT INTO master.featureset(
                                                             name, description, state, created_at, created_by, modified_at, modified_by)
                                                            VALUES (@name, @description, @state,@created_at, @created_by,@modified_at,@modified_by)
                                                             RETURNING id";

                var parameter = new DynamicParameters();
                parameter.Add("@name", featureSet.Name);
                parameter.Add("@description", featureSet.description);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", featureSet.created_at);
                parameter.Add("@created_by", featureSet.created_by);
                parameter.Add("@modified_at", featureSet.modified_at);
                parameter.Add("@modified_by", featureSet.modified_by);

                int InsertedFeatureSetId = await dataAccess.ExecuteScalarAsync<int>(FeatureSetQueryStatement, parameter);
                if (InsertedFeatureSetId > 0)
                {
                    featureSet.FeatureSetID = InsertedFeatureSetId;
                }
                List<int> temp = new List<int>();
                foreach (var item in featureSet.Features)
                {
                    temp.Add(item.Id);

                }

                if (featureSet.Features != null)
                {
                    foreach (var item in featureSet.Features)
                    {
                        var parameterfeature = CreateFeatureSetMapping(InsertedFeatureSetId, item.Id);
                    }
                }

                return featureSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int CreateFeatureSetMapping(int featuresetid, int FeatureID)
        {
            var parameterfeature = new DynamicParameters();
            parameterfeature.Add("@feature_set_id", featuresetid);
            parameterfeature.Add("@feature_id", FeatureID);
            int resultAddFeatureSet = dataAccess.Execute(@"INSERT INTO master.featuresetfeature(
                                                                feature_set_id, feature_id)
                                                                VALUES (@feature_set_id, @feature_id)", parameterfeature);
            return resultAddFeatureSet;
        }

        //GetDataAttributeSetDetails

        public async Task <List<DataAttributeSet>> GetDataAttributeSetDetails(int DataAttributeSetId )
        {
            try
            {
                var QueryStatement = @"select id, name, description, is_exlusive, created_at, created_by, modified_at, modified_by
                                        FROM master.dataattributeset
                                        where id =@DataAttributeSetId";
                var parameter = new DynamicParameters();

                parameter.Add("@DataAttributeSetId", DataAttributeSetId);
                dynamic dataattributeset = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<DataAttributeSet> dataattributesetList = new List<DataAttributeSet>();
                foreach (dynamic record in dataattributeset)
                {
                    dataattributesetList.Add(Map(record));
                }
                return dataattributesetList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private DataAttributeSet Map(dynamic record)
        {
            DataAttributeSet entity = new DataAttributeSet();
            entity.ID = record.id;
            entity.Name = record.name;
            entity.Description = record.description;
            entity.Is_exlusive = record.is_exlusive;
            entity.created_at = record.created_at;
            entity.created_by = record.created_by;
            entity.modified_at = record.modified_at;
            entity.modified_by = record.modified_by;
            return entity;
        }

        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var FSQueryStatement = @" UPDATE master.featureset 
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
                    parameter.Add("@description", featureSet.description);                  
                    parameter.Add("@modified_at", featureSet.modified_at);
                    parameter.Add("@modified_by", featureSet.modified_by);
                    int UpdateFeatureSetID = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);
                    if (UpdateFeatureSetID > 0)
                    {
                        featureSet.FeatureSetID = UpdateFeatureSetID;
                    }

                    List<int> temp = new List<int>();
                    foreach (var item in featureSet.Features)
                    {
                        temp.Add(item.Id);

                    }
                    var removeFeatureID = await RemoveFeatureSetMapping(UpdateFeatureSetID, temp);

                    if (featureSet.Features != null)
                    {
                        foreach (var item in featureSet.Features)
                        {
                            //if (featureSet.Is_Active == true)
                            //{
                                var parameterfeature = UpdateFeatureSetMapping(UpdateFeatureSetID, item.Id);
                            //}
                        }
                    }
                    transactionScope.Complete();
                    return featureSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID)
        {
            int mapfeaturesetvalue = CheckFeatureSetMappingExist(UpdateFeatureSetID,ID);

            if (mapfeaturesetvalue == 0)
            {
                // insert
                var parameterfeature = CreateFeatureSetMapping(UpdateFeatureSetID, ID);
            }
           
            return await Task.FromResult(UpdateFeatureSetID);
        }

        public int CheckFeatureSetMappingExist(int UpdateFeatureSetID, int ID)
        {
            try
            {
                var QueryStatement = @" SELECT CASE WHEN feature_set_id IS NULL THEN 0 ELSE feature_set_id END
                                    FROM master.featuresetfeature
                                    WHERE feature_set_id=@UpdateFeatureSetID
                                    AND feature_id = @ID
                                     ";
                var parameter = new DynamicParameters();
                parameter.Add("@UpdateFeatureSetID", UpdateFeatureSetID);
                parameter.Add("@ID", ID);
                int resultFeatureName = dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
                return resultFeatureName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public int CheckDataAttributeSetMappingExist(int UpdateDataAttributeSetID, int ID)
        {
            try
            {
                var QueryStatement = @" SELECT CASE WHEN data_attribute_set_id IS NULL THEN 0 ELSE data_attribute_set_id END
                                    FROM master.dataattributesetattribute
                                    WHERE data_attribute_set_id=@UpdateDataAttributeSetID
                                    AND data_attribute_id = @ID
                                     ";
                var parameter = new DynamicParameters();
                parameter.Add("@UpdateDataAttributeSetID", UpdateDataAttributeSetID);
                parameter.Add("@ID", ID);
                int result = dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> DeleteFeature(int FeatureId)
        {
            try
            {
                if (FeatureId != 0)
                {
                    var FSQueryStatement = @"update master.feature set state= @state where id=@id  and type= 'D' RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", FeatureId);
                    parameter.Add("@state", 'D');
                    int DeleteDataAttributeSetFeatureID = await dataAccess.ExecuteScalarAsync<int>(FSQueryStatement, parameter);

                    //if(DeleteDataAttributeSetFeatureID >0)
                    //{
                    //    feature.Id = DeleteDataAttributeSetFeatureID;
                    //}
                    return DeleteDataAttributeSetFeatureID;
                }
                else
                {
                    return 0;
                }
           

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CheckFeatureNameExist(string FeatureName,int FeatureId)
        {
            var QueryStatement = @"SELECT id
                                    FROM master.feature 
                                    WHERE state='A'
                                    AND LOWER(name) = LOWER(@roleName)";
            var parameter = new DynamicParameters();
           
            parameter.Add("@roleName", FeatureName.Trim());
            if (FeatureId > 0)
            {
                parameter.Add("@featureid", FeatureId);
                QueryStatement = QueryStatement + " and id != @featureid";
            }
            int resultRoleId = dataAccess.ExecuteScalar<int>(QueryStatement, parameter);
            return resultRoleId;

        }


        public async  Task<int> ChangeFeatureState(int FeatureID,Char State)
        {
            var QueryStatement = @"Update master.feature set state=@state
                                    where id=@featureid returning id";
            var parameter = new DynamicParameters();

                parameter.Add("@featureid", FeatureID);
                parameter.Add("@state", State);
                
        
            int resultfeatureid = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultfeatureid;

        }


    }
}

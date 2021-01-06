using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.data;
using Dapper;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.role.repository
{
    public class FeatureRepository: IFeatureRepository
    {
        private readonly IDataAccess dataAccess;
        public FeatureRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        #region Feature Type
        public async Task<int> AddFeatureType(FeatureType featureType)
        {
            var QueryStatement = @"INSERT INTO dafconnectmaster.rolefeaturetype
                                               (featuretypedescription
                                               ,isactive
                                               ,createddate
                                               ,createdby) 
	                                    VALUES (@featuretypename
                                                ,@isactive
                                                ,@createddate
                                                ,@createdby) RETURNING rolefeaturetypeid";

            var parameter = new DynamicParameters();
            parameter.Add("@featuretypename", featureType.FeatureTypeDescription);
            parameter.Add("@isactive", true);
            parameter.Add("@createddate", DateTime.Now);
            parameter.Add("@createdby", featureType.createdby);
            int resultAddFeatureType = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultAddFeatureType;
        }

        public async Task<int> UpdateFeatureType(FeatureType featureType)
        {
            var QueryStatement = @" UPDATE dafconnectmaster.rolefeaturetype
	                                 SET featuretypedescription=@featuretypedescription,
		                             updatedby=@updatedby,
		                             updateddate=@updateddate  
	                                 WHERE rolefeaturetypeid = @rolefeaturetypeid
                                RETURNING rolefeaturetypeid;";

            var parameter = new DynamicParameters();
            parameter.Add("@rolefeaturetypeid", featureType.RoleFeatureTypeId);
            parameter.Add("@featuretypedescription", featureType.FeatureTypeDescription);
            parameter.Add("@updatedby", featureType.modifiedby);
            parameter.Add("@updateddate", DateTime.Now);
            int resultUpdateFeatureType = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultUpdateFeatureType;
        }

        public async Task<int> DeleteFeatureType(int FeatureTypeId, int Userid)
        {
            var QueryStatement = @" UPDATE dafconnectmaster.rolefeaturetype
	                                 SET isactive=false,
		                             updatedby=@userid,
		                             updateddate=now()  
	                                 WHERE rolefeaturetypeid = @rolefeaturetypeid
                                RETURNING rolefeaturetypeid;";

            var parameter = new DynamicParameters();
            parameter.Add("@rolefeaturetypeid", FeatureTypeId);
            parameter.Add("@userid", Userid);
            int resultDeleteFeatureType = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultDeleteFeatureType;
        }

        public async Task<IEnumerable<FeatureType>> GetFeatureType(int FeatureTypeId)
        {
            var QueryStatement = @" SELECT rolefeaturetypeid, 
                                            featuretypedescription,
                                            isactive,
                                            createddate,
                                            createdby,
                                            updateddate,
                                            updatedby                                            
                                            FROM dafconnectmaster.rolefeaturetype
                                            WHERE (rolefeaturetypeid=@featuretypeid or @featuretypeid=0)
                                            and isactive=true";

            var parameter = new DynamicParameters();
            parameter.Add("@featuretypeid", FeatureTypeId);
            IEnumerable<FeatureType> FeatureTypeDetails = await dataAccess.QueryAsync<FeatureType>(QueryStatement, parameter);
            return FeatureTypeDetails;
        }

        public async Task<int> CheckFeatureTypeExist(string FeatureType)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@featuretype", FeatureType);
            parameter.Add("@featuretypeid", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            int resultFeatureTypeId = await dataAccess.QuerySingleAsync<int>(@"CALL dafconnectmaster.checkfeaturetypeexist(@featuretype,@featuretypeid)", parameter);
            return resultFeatureTypeId;
        }

        #endregion

        #region Features
        public async Task<IEnumerable<Feature>> GetFeature(int RoleFeatureId)
        {

            var QueryStatement = @" SELECT e.rolefeatureid
                                        ,e.featuredescription
                                        ,e.roleFeatureTypeId
                                        ,m.featuredescription AS ParentFeatureName
                                        ,m.rolefeatureid AS parentFeatureId
                                        ,e.ismenu
                                        ,e.seqnum
                                        ,e.isactive
                                        ,e.createddate
                                        ,e.createdby
                                        ,e.updateddate
                                        ,e.updatedby
                                    FROM dafconnectmaster.rolefeature AS e 
                                    LEFT OUTER JOIN dafconnectmaster.rolefeature  AS m
                                    ON e.parentfeatureid =m.rolefeatureid 
                                    WHERE e.isactive=true 
                                    AND ((e.rolefeatureid=@rolefeatureid OR m.rolefeatureid=@rolefeatureid) OR @rolefeatureid=0);";

            var parameter = new DynamicParameters();
            parameter.Add("@rolefeatureid", RoleFeatureId);
            IEnumerable<Feature> FeatureDetails = await dataAccess.QueryAsync<Feature>(QueryStatement, parameter);
            return FeatureDetails;
        }

        public async Task<int> AddFeature(Feature feature)
        {

            var QueryStatement = @" INSERT INTO dafconnectmaster.rolefeature  
                                    (rolefeaturetypeid
                                    ,featuredescription
                                    ,parentfeatureid
                                    ,ismenu
                                    ,seqnum
                                    ,createddate
                                    ,createdby)
                            VALUES ( @RoleFeatureTypeId
                                    ,@FeatureDescription
                                    ,@ParentFeatureId
                                    ,@IsMenu
                                    ,@SeqNum
                                    ,@createddate
                                    ,@createdby) RETURNING rolefeatureid;";

            var parameter = new DynamicParameters();
            parameter.Add("@RoleFeatureTypeId", feature.RoleFeatureTypeId);
            parameter.Add("@FeatureDescription", feature.FeatureDescription);
            parameter.Add("@ParentFeatureId", feature.ParentFeatureId);
            parameter.Add("@IsMenu", feature.IsMenu);
            parameter.Add("@SeqNum", feature.SeqNum);
            parameter.Add("@createddate", DateTime.Now);
            parameter.Add("@createdby", feature.createdby);
            int resultAddFeature = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultAddFeature;
        }

        public async Task<int> UpdateFeature(Feature feature)
        {

            var QueryStatement = @" UPDATE dafconnectmaster.rolefeature 
                                 SET  rolefeaturetypeid = @rolefeaturetypeid
                                ,featuredescription = @featuredescription
                                ,parentfeatureid = @parentfeatureid 
                                ,ismenu = @ismenu
                                ,seqnum = @seqnum
                                ,updateddate = @updateddate
                                ,updatedby = @updatedby
                                WHERE rolefeatureid = @rolefeatureid
                                RETURNING rolefeatureid;";
            var parameter = new DynamicParameters();
            parameter.Add("@rolefeatureid", feature.RoleFeatureId);
            parameter.Add("@rolefeaturetypeid", feature.RoleFeatureTypeId);
            parameter.Add("@featuredescription", feature.FeatureDescription);
            parameter.Add("@parentfeatureid", feature.ParentFeatureId);
            parameter.Add("@ismenu", feature.IsMenu);
            parameter.Add("@seqnum", feature.SeqNum);
            parameter.Add("@updateddate", DateTime.Now);
            parameter.Add("@updatedby", feature.modifiedby);
            int resultUpdateFeature = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultUpdateFeature;
        }

        public async Task<int> DeleteFeature(int RoleFeatureId, int UserId)
        {
            var QueryStatement = @"UPDATE dafconnectmaster.rolefeature 
                                    SET isactive = @isactive
                                    ,updateddate = @updateddate
                                    ,updatedby = @updatedby
                                    WHERE rolefeatureid = @rolefeatureid
                                    RETURNING rolefeatureid;";
            var parameter = new DynamicParameters();
            parameter.Add("@rolefeatureid", RoleFeatureId);
            parameter.Add("@updateddate", DateTime.Now);
            parameter.Add("@updatedby", UserId);
            parameter.Add("@isactive", false);
            int resultDeleteFeature = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultDeleteFeature;
        }

        public async Task<int> CheckFeatureExist(string FeatureName)
        {
            var QueryStatement = @"SELECT CASE WHEN rolefeatureid IS NULL THEN 0 ELSE rolefeatureid END
                                    FROM dafconnectmaster.rolefeature 
                                    WHERE isactive=true
                                    AND LOWER(featuredescription) = LOWER(@featuredescription)";
            var parameter = new DynamicParameters();
            parameter.Add("@featuredescription", FeatureName);
            int resultFeatureName = await dataAccess.QueryFirstAsync<int>(QueryStatement, parameter);
            return resultFeatureName;
        }

        #endregion

        #region Feature Set 

        public async Task<int> AddFeatureSet(FeatureSet featureSet)
        {
            var FeatureSetQueryStatement = @"INSERT INTO dafconnectmaster.featureset
                                               (featuresetdescription
                                               ,isactive
                                               ,createddate
                                               ,createdby) 
	                                    VALUES (@featuresetdescription
                                                ,@isactive
                                                ,@createddate
                                                ,@createdby) RETURNING featuresetid";

            var parameter = new DynamicParameters();
            parameter.Add("@featuresetdescription", featureSet.FeatureSetName);
            parameter.Add("@isactive", true);
            parameter.Add("@createddate", DateTime.Now);
            parameter.Add("@createdby", featureSet.createdby);
            int InsertedFeatureSetId = await dataAccess.ExecuteScalarAsync<int>(FeatureSetQueryStatement, parameter);

            if (featureSet.Features != null)
            {
                foreach (var item in featureSet.Features)
                {
                    item.FeatureSetID = InsertedFeatureSetId;
                    item.createddate = DateTime.Now;
                    item.isactive = true;
                }

                int resultAddFeatureSet = await dataAccess.ExecuteAsync("INSERT INTO dafconnectmaster.featuresetfeature (featuresetid,rolefeatureid,isactive,createddate,createdby,isrolefeatureenabled) VALUES (@FeatureSetID,@RoleFeatureId,@isactive,@createddate,@createdby,@IsRoleFeatureEnabled) RETURNING featuresetfeatureid", featureSet.Features);
                return resultAddFeatureSet;
            }
            return InsertedFeatureSetId;
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
            parameter.Add("@featuresetdescription", featureSet.FeatureSetName);
            parameter.Add("@updateddate", DateTime.Now);
            parameter.Add("@updatedby", featureSet.modifiedby);
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
                FSFparameter.Add("@updatedby", featureSet.modifiedby);
                FSFparameter.Add("@isactive", false);
                await dataAccess.ExecuteScalarAsync<int>(FSFQueryStatement, FSFparameter);

                foreach (var item in featureSet.Features)
                {
                    item.FeatureSetID = featureSet.FeatureSetID;
                    item.createddate = DateTime.Now;
                    item.isactive = true;
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

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId)
        {

            var QueryStatement = @" SELECT featuresetid, 
                                    featuresetdescription as FeatureSetName,                                    
                                    isactive,
                                    createddate,
                                    createdby,
                                    updateddate,
                                    updatedby
                                    from dafconnectmaster.featureset
                                    where isactive=true
                                    and (featuresetid=@featuresetid OR @featuresetid=0)";

            var parameter = new DynamicParameters();
            parameter.Add("@featuresetid", FeatureSetId);
            IEnumerable<FeatureSet> FeatureSetDetails = await dataAccess.QueryAsync<FeatureSet>(QueryStatement, parameter);
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

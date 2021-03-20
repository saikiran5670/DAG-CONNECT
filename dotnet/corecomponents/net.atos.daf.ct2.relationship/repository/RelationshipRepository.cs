﻿using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.relationship.entity;
using net.atos.daf.ct2.relationship.ENUM;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.relationship.repository
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly IDataAccess _dataAccess;

        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RelationshipRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

        }

        public async Task<Relationship> CreateRelationship(Relationship relationship)
        {
            log.Info("Create Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var defaultLevelCode = 30;
                var defaultCode = "Owner";
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", relationship.OrganizationId);
                var query = @"SELECT id FROM master.organization where id=@org_id";
                int orgexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                if (orgexist > 0)
                {
                    parameter.Add("@OrganizationId", relationship.OrganizationId);
                    parameter.Add("@Name", relationship.Name);
                    parameter.Add("@Code", !string.IsNullOrEmpty(relationship.Code) ? relationship.Code : defaultCode);
                    parameter.Add("@Level", relationship.Level != 0 ? relationship.Level : defaultLevelCode);
                    parameter.Add("@Description", relationship.Description);
                    parameter.Add("@FeatureSetId", relationship.FeaturesetId);
                    parameter.Add("@Is_active", relationship.IsActive);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    string queryInsert = "insert into master.orgrelationship(organization_id, feature_set_id, name, description, code, is_active, level,created_at) " +
                                          "values(@OrganizationId,@FeatureSetId, @Name, @Description, @Code,@Is_active, @Level,@created_at) RETURNING id";
                    var orgid = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                    relationship.Id = orgid;
                }
            }
            catch (Exception ex)
            {
                log.Info("Create Organization Relationship method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(relationship));
                log.Error(ex.ToString());
                throw ex;
            }
            return relationship;
        }

        public async Task<Relationship> UpdateRelationship(Relationship relationship)
        {
            log.Info("Update Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();

                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", relationship.OrganizationId);
                var query = @"SELECT id FROM master.organization where id=@org_id";
                int orgexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                if (orgexist > 0)
                {
                    parameter.Add("@Id", relationship.Id);
                    parameter.Add("@OrganizationId", relationship.OrganizationId);
                    parameter.Add("@Name", relationship.Name);
                    parameter.Add("@Code", relationship.Code);
                    parameter.Add("@Level", relationship.Level);
                    parameter.Add("@Description", relationship.Description);
                    parameter.Add("@FeatureSetId", relationship.FeaturesetId);
                    parameter.Add("@Is_active", relationship.IsActive);

                    var queryUpdate = @"update master.orgrelationship set organization_id=@OrganizationId,
                                                                          feature_set_id=@FeatureSetId,
                                                                          name=@Name,
                                                                          description=@Description,
                                                                          code=@Code,
                                                                          is_active=@Is_active,
                                                                          level =@Level            
	                                 WHERE id = @Id RETURNING id;";


                    var orgid = await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);
                    relationship.Id = orgid;
                }
            }
            catch (Exception ex)
            {
                log.Info("Update Relationship method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(relationship));
                log.Error(ex.ToString());
                throw ex;
            }
            return relationship;
        }

        public async Task<bool> DeleteRelationship(int relationshipId)
        {
            log.Info("Delete Organization Relationship method called in repository");
            try
            {
                //check either relationship id maaped with organization or not 
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@relationship_id", relationshipId);
                var query = @"SELECT relationship_id FROM master.orgrelationshipmapping where relationship_id=@relationship_id";
                int relationshipexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                if (relationshipexist > 0)
                {
                    return false;
                }
                else
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", relationshipId);
                    var deletequery = @"update master.orgrelationship set is_active=false where id=@id";
                    int isdelete = await _dataAccess.ExecuteScalarAsync<int>(deletequery, parameter);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Info("Delete  Relationship method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(relationshipId));
                log.Error(ex.ToString());
                throw ex;
            }
        }


        public async Task<List<Relationship>> GetRelationship(Relationship filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                var relationships = new List<Relationship>();
                string query = string.Empty;

                query = @"select id, organization_id, feature_set_id, name, description, code, is_active, level,created_at from master.orgrelationship relationship where is_active = true ";

                if (filter != null)
                {
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and relationship.id=@id ";
                    }
                    if (filter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", filter.OrganizationId);
                        query = query + "and ( level=40 or relationship.organization_id=@organization_id )";
                    }

                    if (!string.IsNullOrEmpty(filter.Code))
                    {

                        parameter.Add("@code", filter.Code.ToLower());
                        query = query + " and LOWER(relationship.Code) = @code ";
                    }

                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        parameter.Add("@name", filter.Name + "%");
                        query = query + " and relationship.name like @name ";
                    }
                    if (!string.IsNullOrEmpty(filter.Description))
                    {
                        parameter.Add("@description", filter.Description + "%");
                        query = query + " and relationship.description like @description ";
                    }

                    if (filter.FeaturesetId > 0)
                    {
                        parameter.Add("@feature_set_id", filter.FeaturesetId);
                        query = query + " and relationship.feature_set_id = @feature_set_id ";
                    }

                    if (filter.Level != 0)
                    {
                        parameter.Add("@level", filter.Level);

                        query = query + " and relationship.level=@level";
                    }
                    query = query + "ORDER BY id ASC; ";
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);

                    foreach (dynamic record in result)
                    {

                        relationships.Add(MapData(record));
                    }
                }
                return relationships;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Relationship MapData(dynamic record)
        {
            var relationship = new Relationship();
            relationship.Id = record.id != null ? record.id : 0;
            relationship.Code = !string.IsNullOrEmpty(record.code) ? record.code : string.Empty;
            relationship.Level = record.level != null ? record.level : 0;
            relationship.Description = !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            relationship.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            relationship.FeaturesetId = record.feature_set_id != null ? record.feature_set_id : 0;
            relationship.OrganizationId = record.organization_id != null ? record.organization_id : 0;
            relationship.IsActive = record.is_active;
            relationship.CreatedAt = record.created_at;
            return relationship;
        }
        public async Task<RelationshipLevelCode> GetRelationshipLevelCode()
        {

            var levelCode = new RelationshipLevelCode();
            levelCode.Levels = Enum.GetValues(typeof(RelationshipLevel))
                 .Cast<RelationshipLevel>()
                 .Select(t => new Level
                 {
                     Id = ((int)t),
                     Name = t.ToString()
                 }).ToList();

            levelCode.Codes = Enum.GetValues(typeof(RelationshipCode))
                 .Cast<RelationshipCode>()
                 .Select(t => new Code
                 {
                     Id = ((int)t),
                     Name = t.ToString()
                 }).ToList();
            return levelCode;
        }

        public async Task<int> CreateRelationShipMapping(OrganizationRelationShip relationshipMapping)
        {

            var Inputparameter = new DynamicParameters();
            var relationships = new List<Relationship>();
            string query = string.Empty;
            Inputparameter.Add("@relationship_id", relationshipMapping.relationship_id);
            //Inputparameter.Add("@vehicle_id", relationshipMapping.vehicle_id);
            if (relationshipMapping.vehicle_group_id == 0)
            {
                Inputparameter.Add("@vehicle_group_id", null);
            }
            else
            {
                Inputparameter.Add("@vehicle_group_id", relationshipMapping.vehicle_group_id);
            }
            Inputparameter.Add("@owner_org_id", relationshipMapping.owner_org_id);
            Inputparameter.Add("@created_org_id", relationshipMapping.created_org_id);
            Inputparameter.Add("@target_org_id", relationshipMapping.target_org_id);
            Inputparameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            Inputparameter.Add("@end_date", null);
            Inputparameter.Add("@allow_chain", relationshipMapping.allow_chain);
            Inputparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

            var queryInsert = @"insert into master.orgrelationshipmapping(relationship_id,vehicle_group_id,
                     owner_org_id,created_org_id,target_org_id,start_date,end_date,allow_chain,created_at)                     
                     values(@relationship_id,@vehicle_group_id,@owner_org_id,@created_org_id,@target_org_id,@start_date,@end_date,@allow_chain,@created_at) returning id";

            var OwnerRelationshipId = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
            return OwnerRelationshipId;
        }

        public async Task<int> EndRelationShipMapping(int OrgRelationId)
        {
            var Inputparameter = new DynamicParameters();
            Inputparameter.Add("@orgrelationid", OrgRelationId);
            Inputparameter.Add("@enddate", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            var query = @"update master.orgrelationshipmapping set end_date = @enddate 
                            where id=@orgrelationid returning id";
            var OwnerRelationshipId = await _dataAccess.ExecuteScalarAsync<int>(query, Inputparameter);
            return OwnerRelationshipId;
        }

        public async Task<int> AllowChaining(int OrgRelationId,bool AllowChaining)
        {
            var Inputparameter = new DynamicParameters();
            Inputparameter.Add("@orgrelationid", OrgRelationId);
            Inputparameter.Add("@allowchaining", AllowChaining);
            Inputparameter.Add("@enddate", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            var query = @"update master.orgrelationshipmapping set allow_chain = @allowchaining 
                            where id=@orgrelationid returning id";
            var OwnerRelationshipId = await _dataAccess.ExecuteScalarAsync<int>(query, Inputparameter);
            return OwnerRelationshipId;
        }

    }
}

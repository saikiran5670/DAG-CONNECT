using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.group
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDataAccess _dataAccess;
        public GroupRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
        #region public methods
        public async Task<Group> Create(Group group)

        {
            try
            {

                // check for exists
                // group type single
                if (group.GroupType == GroupType.Single)
                {
                    group = await CheckSingleGroup(group);
                }
                else
                {
                    group = await Exists(group);
                }
                // duplicate group
                if (group.Exists)
                {
                    return group;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@object_type", (char)group.ObjectType);
                parameter.Add("@group_type", (char)group.GroupType);
                parameter.Add("@argument", string.IsNullOrEmpty(group.Argument) ? null : group.Argument);
                if (group.FunctionEnum == FunctionEnum.None) parameter.Add("@function_enum", null);
                else parameter.Add("@function_enum", (char)group.FunctionEnum);
                parameter.Add("@organization_id", group.OrganizationId);
                // if the group type is single
                if (group.GroupType == GroupType.Single && group.RefId > 0) parameter.Add("@ref_id", group.RefId);
                else parameter.Add("@ref_id", null);
                parameter.Add("@name", group.Name);
                parameter.Add("@description", group.Description);
                parameter.Add("@created_at", group.CreatedAt.Value);
                string query = "insert into master.group(object_type, group_type, argument, function_enum, organization_id, ref_id, name, description,created_at) " +
                              "values(@object_type, @group_type, @argument, @function_enum, @organization_id, @ref_id, @name, @description,@created_at) RETURNING id";

                var groupid = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                group.Id = groupid;
            }
            catch (Exception)
            {
                throw;
            }
            return group;
        }
        public async Task<Group> Update(Group group)
        {
            try
            {
                // check for exists
                // check for exists
                var result = await Exists(group);
                if (group.Exists)
                {
                    return group;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", group.Id);
                parameter.Add("@object_type", (char)group.ObjectType);
                parameter.Add("@group_type", (char)group.GroupType);
                parameter.Add("@argument", group.Argument);
                if (group.FunctionEnum == FunctionEnum.None) parameter.Add("@function_enum", null);
                else parameter.Add("@function_enum", (char)group.FunctionEnum);
                parameter.Add("@organization_id", group.OrganizationId);
                //parameter.Add("@ref_id", group.RefId);ref_id = @ref_id,
                parameter.Add("@name", group.Name);
                parameter.Add("@description", group.Description);

                var query = @"update master.group set object_type = @object_type,group_type = @group_type,
                                     argument = @argument,function_enum = @function_enum,                                     
                                     name = @name,description = @description
	                                 WHERE id = @id
                                     RETURNING id;";
                var groupid = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
            return group;
        }

        public async Task<bool> CanDelete(long groupId, ObjectType objectType)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", groupId);
                string query = string.Empty;

                if (objectType == ObjectType.AccountGroup)
                    query = @"select exists(select 1 from master.accessrelationship where account_group_id = @id)";
                else
                {
                    // Check for existing Access relationship, Alert and Org relationship
                    // TODO - Scheduler check
                    query = @"select exists
                              (
	                              select 1 from master.accessrelationship where vehicle_group_id = @id
	                              UNION
	                              select 1 from master.alert where vehicle_group_id = @id and state in ('A', 'I')
                                  UNION
	                              select 1
	                              from master.orgrelationshipmapping orm
                                  inner join master.orgrelationship ors on orm.relationship_id=ors.id and orm.vehicle_group_id = @id and ors.state='A' 
	                              where case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                    else COALESCE(end_date,0) = 0 end
                              )";
                }

                return !await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VehicleGroupDelete> Delete(long groupId, ObjectType objectType)
        {
            var response = new VehicleGroupDelete();
            try
            {
                response.CanDelete = await CanDelete(groupId, objectType);
                if (response.CanDelete)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", groupId);
                    string query = string.Empty;
                    //TODO: Need to prepare this as single for delete all ref. of group
                    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        // delete group ref
                        query = @"delete from master.groupref where group_id = @id";
                        await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                        // delete group 
                        query = @"delete from master.group where id = @id";
                        await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        transactionScope.Complete();

                        response.IsDeleted = true;
                    }
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Group>> Get(GroupFilter groupFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select id,object_type,group_type,argument,function_enum,organization_id,ref_id,name,description,created_at from master.group where 1=1 ";

                if (groupFilter != null)
                {
                    // group id filter
                    if (groupFilter.Id > 0)
                    {
                        parameter.Add("@id", groupFilter.Id);
                        query = query + " and id=@id ";
                    }
                    // ref id filter
                    if (groupFilter.RefId > 0)
                    {
                        return await GetByRefId(groupFilter);
                    }
                    // organization id filter
                    if (groupFilter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", groupFilter.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                    // group type filter
                    if (((char)groupFilter.GroupType) != ((char)GroupType.None))
                    {
                        parameter.Add("@group_type", (char)groupFilter.GroupType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and group_type=@group_type";
                    }
                    else
                    {
                        parameter.Add("@group_type_group", (char)GroupType.Group, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        parameter.Add("@group_type_dynamic", (char)GroupType.Dynamic, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and (group_type=@group_type_group or group_type=@group_type_dynamic) ";
                    }

                    // object type filter
                    if (((char)groupFilter.ObjectType) != ((char)ObjectType.None))
                    {

                        parameter.Add("@object_type", (char)groupFilter.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type=@object_type ";
                    }

                    // Account Id list Filter                       
                    if (groupFilter.GroupIds != null && groupFilter.GroupIds.Count() > 0)
                    {
                        parameter.Add("@groupids", groupFilter.GroupIds);
                        query = query + " and id=ANY(@groupids)";
                    }
                }
                IEnumerable<dynamic> groups = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                Group group = new Group();

                foreach (dynamic record in groups)
                {
                    group = Map(record);

                    if (group.GroupType == GroupType.Group)
                    {
                        if (groupFilter.GroupRef)
                        {
                            // group ref filter 
                            if (groupFilter.GroupRef)
                            {
                                group.GroupRef = GetRef(group.Id).Result;
                            }
                        }
                        if (groupFilter.GroupRefCount)
                        {

                            // group ref filter 
                            if (groupFilter.GroupRefCount)
                            {
                                group.GroupRefCount = GetRefCount(group.Id, groupFilter.OrganizationId).Result;
                            }
                        }
                    }
                    if (group.GroupType == GroupType.Dynamic && group.ObjectType == ObjectType.AccountGroup)
                    {
                        group.GroupRefCount = await GetAccountCount(group.OrganizationId);
                    }
                    groupList.Add(group);
                }

                return groupList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Group VEHICLE_QUERY
        /// </summary>
        /// <param name="groupIds"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetGroupVehicleCount(int groupId, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@GroupId", groupId);
                parameter.Add("@OrganizationId", organizationId);

                var queryOwnedG = @"                        
                        SELECT DISTINCT v.id
                        FROM master.vehicle v
                        INNER JOIN master.groupref gref ON v.id=gref.ref_id
                        INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V' AND grp.id = @GroupId
                        INNER JOIN master.orgrelationshipmapping as om on v.id = om.vehicle_id and v.organization_id=om.owner_org_id and om.owner_org_id=@OrganizationId
                        INNER JOIN master.orgrelationship as ors on om.relationship_id=ors.id and ors.state='A' and lower(ors.code)='owner'
                        WHERE 
	                        CASE when COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                        ELSE COALESCE(end_date,0) = 0 END";

                return await _dataAccess.QueryAsync<int>(queryOwnedG, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Dynamic VEHICLE_QUERY
        /// </summary>
        /// <param name="groupIds"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetDynamicVehicleCount(int organizationId, FunctionEnum functionEnum)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@OrganizationId", organizationId);
                string queryOwned = string.Empty, queryVisible = string.Empty, queryUnion = "UNION";
                string selectStatement = string.Empty;

                queryOwned = @"                        
                        SELECT DISTINCT v.id
                        FROM master.vehicle v
                        INNER JOIN master.orgrelationshipmapping as om on v.id = om.vehicle_id and v.organization_id=om.owner_org_id and om.owner_org_id=@OrganizationId
                        INNER JOIN master.orgrelationship as ors on om.relationship_id=ors.id and ors.state='A' and lower(ors.code)='owner'
                        WHERE 
	                        CASE when COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                        ELSE COALESCE(end_date,0) = 0 END";

                queryVisible =
                        @"SELECT v.id
                        FROM master.vehicle v
                        LEFT OUTER JOIN master.groupref gref ON v.id=gref.ref_id
                        INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V' AND grp.group_type='G'
                        INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.target_org_id=@OrganizationId
                        INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                        WHERE 
	                        CASE WHEN COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                        ELSE COALESCE(end_date,0) = 0 END
                        UNION
                        SELECT v.id
                        FROM master.group grp
                        INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id 
                                    AND orm.owner_org_id=grp.organization_id 
                                    AND orm.target_org_id=@OrganizationId
                                    AND grp.group_type='D' AND grp.object_type='V'
                        INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id AND ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                        INNER JOIN master.vehicle v on v.organization_id = grp.organization_id
                        WHERE 
	                        CASE WHEN COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                        ELSE COALESCE(end_date,0) = 0 END";

                string queryOEM = @"select veh.id
                                    from master.vehicle veh
                                    WHERE veh.oem_organisation_id=@OrganizationId";

                switch (functionEnum)
                {
                    case FunctionEnum.OwnedVehicles:
                        selectStatement = queryOwned;
                        break;
                    case FunctionEnum.VisibleVehicles:
                        selectStatement = queryVisible;
                        break;
                    case FunctionEnum.All:
                        selectStatement = $"{ queryOwned } { queryUnion } { queryVisible }";
                        break;
                    case FunctionEnum.OEM:
                        selectStatement = queryOEM;
                        break;
                    default:
                        break;
                }
                return await _dataAccess.QueryAsync<int>(selectStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddRefToGroups(List<GroupRef> groupRef)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                var result = false;
                if (groupRef != null)
                {
                    foreach (GroupRef gref in groupRef)
                    {
                        if (gref.Group_Id > 0 && gref.Ref_Id > 0)
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@group_id", gref.Group_Id);
                            parameter.Add("@ref_id", gref.Ref_Id);
                            query = @"insert into master.groupref (group_id,ref_id) values (@group_id,@ref_id)";
                            await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        }
                    }
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> UpdateRef(Group group)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                var result = false;

                if (await RemoveRef(group.Id))
                {
                    if (group.GroupRef != null)
                    {
                        parameter.Add("@group_id", group.Id);
                        query = @"insert into master.groupref (group_id,ref_id) values ";

                        foreach (GroupRef groupRef in group.GroupRef)
                        {
                            parameter.Add("@ref_id_" + groupRef.Ref_Id.ToString(), groupRef.Ref_Id);
                            query = query + @" (@group_id,@ref_id_" + groupRef.Ref_Id.ToString() + "),";
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            query = query.TrimEnd(',');
                            await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        }
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region private methods
        private async Task<Group> Exists(Group groupRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select id from master.group where 1=1 ";
                if (groupRequest != null)
                {
                    // id
                    if (Convert.ToInt32(groupRequest.Id) > 0)
                    {
                        parameter.Add("@id", groupRequest.Id);
                        query = query + " and id!=@id";
                    }
                    // name
                    if (!string.IsNullOrEmpty(groupRequest.Name))
                    {
                        parameter.Add("@name", groupRequest.Name);
                        query = query + " and name=@name";
                    }
                    // organization id filter
                    if (groupRequest.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", groupRequest.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                    // object type filter
                    if (((char)groupRequest.ObjectType) != ((char)ObjectType.None))
                    {

                        parameter.Add("@object_type", (char)groupRequest.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type=@object_type ";
                    }
                }
                var groupid = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (groupid > 0)
                {
                    groupRequest.Exists = true;
                    groupRequest.Id = groupid;
                }
                return groupRequest;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<Group> CheckSingleGroup(Group groupRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select id from master.group where 1=1 ";
                if (groupRequest != null)
                {

                    // organization id filter
                    if (groupRequest.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", groupRequest.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                    // object type filter
                    if (((char)groupRequest.ObjectType) != ((char)ObjectType.None))
                    {

                        parameter.Add("@object_type", (char)groupRequest.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type=@object_type ";
                    }
                    // organization id filter
                    if (groupRequest.RefId > 0)
                    {
                        parameter.Add("@ref_id", groupRequest.RefId);
                        query = query + " and ref_id=@ref_id and group_type='S'";

                    }
                }
                var groupid = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (groupid > 0)
                {
                    groupRequest.Exists = true;
                    groupRequest.Id = groupid;
                }
                return groupRequest;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Group Map(dynamic record)
        {
            Group entity = new Group();
            entity.Id = record.id;
            entity.Name = record.name;
            entity.Description = record.description;
            entity.ObjectType = (ObjectType)Convert.ToChar(record.object_type);
            entity.GroupType = (GroupType)Convert.ToChar(record.group_type);
            entity.Argument = record.argument;
            if (record.function_enum == null)
            {
                entity.FunctionEnum = FunctionEnum.None;
            }
            else
            {
                entity.FunctionEnum = (FunctionEnum)Convert.ToChar(record.function_enum);
            }

            entity.OrganizationId = record.organization_id;
            entity.RefId = record.ref_id;
            if (record.created_at is object)
            {
                entity.CreatedAt = record.created_at;
            }
            return entity;
        }

        private async Task<List<Group>> GetByRefId(GroupFilter filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select g.id,g.object_type,g.group_type,g.argument,g.function_enum,g.organization_id,g.ref_id,g.name,
                            g.description from master.group g inner join master.groupref gr on g.id = gr.group_id";

                // ref id filter
                if (filter.RefId > 0)
                {
                    query = query + " and gr.ref_id=@ref_id ";
                    parameter.Add("@ref_id", filter.RefId);
                }
                // organization id filter
                if (filter.OrganizationId > 0)
                {
                    parameter.Add("@organization_id", filter.OrganizationId);
                    query = query + " and g.organization_id=@organization_id ";
                }
                // group type filter
                if (((char)filter.GroupType) != ((char)GroupType.None))
                {
                    parameter.Add("@group_type", (char)filter.GroupType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                    query = query + " and g.group_type=@group_type";
                }

                // object type filter
                if (((char)filter.ObjectType) != ((char)ObjectType.None))
                {

                    parameter.Add("@object_type", (char)filter.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                    query = query + " and g.object_type=@object_type ";
                }

                IEnumerable<dynamic> groups = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                Group group = new Group();

                foreach (dynamic record in groups)
                {
                    group = Map(record);
                    if (filter.GroupRef || filter.GroupRefCount)
                    {
                        // group ref filter 
                        if (filter.GroupRef)
                        {
                            group.GroupRef = GetRef(group.Id).Result;
                        }
                        // group ref filter 
                        if (filter.GroupRefCount)
                        {
                            group.GroupRefCount = GetRefCount(group.Id, filter.OrganizationId).Result;
                        }
                    }
                    groupList.Add(group);
                }
                return groupList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<GroupRef>> GetRef(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select group_id,ref_id from master.groupref where group_id = @group_id";
                parameter.Add("@group_id", groupid);
                var groupref = await _dataAccess.QueryAsync<GroupRef>(query, parameter);
                return groupref.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GroupRef>> GetGroupRef(int groupid, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select group_id,ref_id  from master.groupref where ref_id in (
                                        SELECT v.id
                                        FROM master.vehicle v
                                        INNER JOIN master.orgrelationshipmapping as om on v.id = om.vehicle_id and v.organization_id=om.owner_org_id and om.owner_org_id=@organizationId
                                        INNER JOIN master.orgrelationship as ors on om.relationship_id=ors.id and ors.state='A' and lower(ors.code)='owner'
                                        INNER JOIN master.groupref gref on v.id=gref.ref_id
                                        WHERE 
                                        case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
                                        else COALESCE(end_date,0) = 0 end
                                        and gref.group_id=@group_id
                                        UNION
                                        -- Visible vehicles of type G
                                        SELECT  distinct gref.ref_id
                                        FROM master.vehicle v
                                        INNER JOIN master.groupref gref ON v.id=gref.ref_id
                                        INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V'
                                        INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.target_org_id=@organizationId
                                        INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                        WHERE 
                                        case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
                                        else COALESCE(end_date,0) = 0 end
	                                        ) and group_id= @group_id";
                parameter.Add("@group_id", groupid);
                parameter.Add("@organizationId", organizationId);
                var groupref = await _dataAccess.QueryAsync<GroupRef>(query, parameter);
                return groupref.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> GetRefCount(int groupid, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select count(distinct ref_id) as refcount  from master.groupref where ref_id in (
                                            SELECT v.id
                                            FROM master.vehicle v
                                            INNER JOIN master.orgrelationshipmapping as om on v.id = om.vehicle_id and v.organization_id=om.owner_org_id and om.owner_org_id=@organizationId
                                            INNER JOIN master.orgrelationship as ors on om.relationship_id=ors.id and ors.state='A' and lower(ors.code)='owner'
                                            INNER JOIN master.groupref gref on v.id=gref.ref_id
                                            WHERE 
                                            case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
                                            else COALESCE(end_date,0) = 0 end
                                            and gref.group_id=@group_id
                                            UNION
                                            -- Visible vehicles of type G
                                            SELECT  distinct gref.ref_id
                                            FROM master.vehicle v
                                            INNER JOIN master.groupref gref ON v.id=gref.ref_id
                                            INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V'
                                            INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.target_org_id=@organizationId
                                            INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                            WHERE 
                                            case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
                                            else COALESCE(end_date,0) = 0 end
	                                            ) and group_id=@group_id  ";
                parameter.Add("@group_id", groupid);
                parameter.Add("@organizationId", organizationId);
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> GetAccountCount(int organization_id)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                int count = 0;
                query = @"select count(1) from master.account a join master.accountorg ag on a.id = ag.account_id and a.state='A' 
                and ag.state='A' where ag.organization_id=@organization_id";
                parameter.Add("@organization_id", organization_id);
                count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> RemoveRef(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", groupid);
                var query = @"delete from master.groupref where group_id = @id";
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveRefByRefId(int refId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@ref_id", refId);
                var query = @"delete from master.groupref where ref_id=@ref_id";
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region vehicle Group


        public async Task<IEnumerable<Group>> GetVehicleGroupWithVehCount(GroupFilter groupFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select id,object_type,group_type,argument,function_enum,organization_id,ref_id,name,description,created_at from master.group where 1=1 and COALESCE(function_enum,'') <> 'M' ";

                if (groupFilter != null)
                {
                    // group id filter
                    if (groupFilter.Id > 0)
                    {
                        parameter.Add("@id", groupFilter.Id);
                        query = query + " and id=@id ";
                    }

                    // organization id filter
                    if (groupFilter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", groupFilter.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                    // group type filter
                    if (((char)groupFilter.GroupType) != ((char)GroupType.None))
                    {
                        parameter.Add("@group_type", (char)groupFilter.GroupType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and group_type=@group_type";
                    }
                    else
                    {
                        parameter.Add("@group_type_group", (char)GroupType.Group, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        parameter.Add("@group_type_dynamic", (char)GroupType.Dynamic, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and (group_type=@group_type_group or group_type=@group_type_dynamic) ";
                    }

                    // object type filter
                    if (((char)groupFilter.ObjectType) != ((char)ObjectType.None))
                    {

                        parameter.Add("@object_type", (char)groupFilter.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type=@object_type ";
                    }
                }
                IEnumerable<dynamic> groups = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                Group group = new Group();

                foreach (dynamic record in groups)
                {
                    group = Map(record);

                    switch (group.GroupType)
                    {
                        case GroupType.Single:
                            //Single
                            group.GroupRefCount = 1;
                            break;
                        case GroupType.Group:
                            //Group
                            group.GroupRefCount = GetRefCount(group.Id, groupFilter.OrganizationId).Result;
                            break;
                        case GroupType.Dynamic:
                            //Dynamic
                            switch (group.FunctionEnum)
                            {
                                case FunctionEnum.All:
                                case FunctionEnum.OwnedVehicles:
                                case FunctionEnum.VisibleVehicles:
                                    var vehicleIds = await GetDynamicVehicleCount(group.OrganizationId, group.FunctionEnum);
                                    group.GroupRefCount = vehicleIds.Count();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                    groupList.Add(group);
                }

                return groupList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}

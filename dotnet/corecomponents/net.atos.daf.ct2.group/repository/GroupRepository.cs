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

        public async Task<bool> Delete(long groupid, ObjectType objectType)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", groupid);
                string query = string.Empty;
                //TODO: Need to prepare this as single for delete all ref. of group
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // delete access relation ship
                    if (objectType == ObjectType.AccountGroup)
                        query = @"delete from master.accessrelationship where account_group_id = @id";
                    else query = @"delete from master.accessrelationship where vehicle_group_id = @id";
                    await _dataAccess.ExecuteScalarAsync<int>(query, parameter);


                    // delete group ref
                    query = @"delete from master.groupref where group_id = @id";
                    await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    // delete group 
                    query = @"delete from master.group where id = @id";
                    await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    transactionScope.Complete();
                    return true;
                }

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

                    //// function functional enum filter
                    //if (((char)groupFilter.FunctionEnum) != ((char)FunctionEnum.None))
                    //{
                    //    parameter.Add("@function_enum", (char)groupFilter.FunctionEnum, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                    //    query = query + " and function_enum=@function_enum";
                    //}
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
                                group.GroupRefCount = GetRefCount(group.Id).Result;
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
                //// function functional enum filter
                //if (((char)filter.FunctionEnum) != ((char)FunctionEnum.None))
                //{
                //    parameter.Add("@function_enum", (char)filter.FunctionEnum, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                //    query = query + " and g.function_enum=@function_enum";
                //}
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
                            group.GroupRefCount = GetRefCount(group.Id).Result;
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
        private async Task<int> GetRefCount(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select count(ref_id) as refcount from master.groupref where group_id = @group_id";
                parameter.Add("@group_id", groupid);
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
                and ag.state='A' where lower(a.type)='p' and ag.organization_id=@organization_id";
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
                var query = @"select id,object_type,group_type,argument,function_enum,organization_id,ref_id,name,description,created_at from master.group where 1=1 ";

                if (groupFilter != null)
                {
                    // group id filter
                    if (groupFilter.Id > 0)
                    {
                        parameter.Add("@id", groupFilter.Id);
                        query = query + " and id=@id ";
                    }
                    //// ref id filter
                    //if (groupFilter.RefId > 0)
                    //{
                    //    return await GetByRefId(groupFilter);
                    //}
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

                    //// function functional enum filter
                    //if (((char)groupFilter.FunctionEnum) != ((char)FunctionEnum.None))
                    //{
                    //    parameter.Add("@function_enum", (char)groupFilter.FunctionEnum, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                    //    query = query + " and function_enum=@function_enum";
                    //}
                    // object type filter
                    if (((char)groupFilter.ObjectType) != ((char)ObjectType.None))
                    {

                        parameter.Add("@object_type", (char)groupFilter.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type=@object_type ";
                    }

                    //// Account Id list Filter                       
                    //if (groupFilter.GroupIds != null && groupFilter.GroupIds.Count() > 0)
                    //{
                    //    parameter.Add("@groupids", groupFilter.GroupIds);
                    //    query = query + " and id=ANY(@groupids)";
                    //}
                }
                IEnumerable<dynamic> groups = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                Group group = new Group();

                foreach (dynamic record in groups)
                {
                    group = Map(record);

                    if (group.GroupType == GroupType.Group)
                    {
                        //if (groupFilter.GroupRef)
                        //{
                        //    // group ref filter 
                        //    if (groupFilter.GroupRef)
                        //    {
                        //        group.GroupRef = GetRef(group.Id).Result;
                        //    }
                        //}
                        //if (groupFilter.GroupRefCount)
                        //{

                        // group ref filter 
                        //if (groupFilter.GroupRefCount)
                        //{
                        group.GroupRefCount = GetRefCount(group.Id).Result;
                        //}
                        //}
                    }
                    if (group.GroupType == GroupType.Dynamic && group.FunctionEnum == FunctionEnum.All)
                    {
                        group.GroupRefCount = GetDynamicAllRefCount(group.OrganizationId).Result;

                    }
                    else if (group.GroupType == GroupType.Dynamic && group.FunctionEnum == FunctionEnum.OwnedVehicles)
                    {
                        group.GroupRefCount = GetDynamicOwnedRefCount(group.OrganizationId).Result;
                    }
                    else if (group.GroupType == GroupType.Dynamic && group.FunctionEnum == FunctionEnum.VisibleVehicles)
                    {
                        group.GroupRefCount = GetDynamicVisibleRefCount(group.OrganizationId).Result;
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

        private async Task<int> GetDynamicAllRefCount(int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select count(distinct veh.id)
	                               from master.vehicle veh
                            Inner join master.orgrelationshipmapping  orm
                            on orm.vehicle_id=veh.id
                            Inner join master.orgrelationship ors
                            on ors.id=orm.relationship_id
                            where  1=1
                            and ors.state='A'
                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                            else COALESCE(end_date,0) =0 end
							and (orm.created_org_id=@organization_id or orm.owner_org_id=@organization_id or orm.target_org_id=@organization_id)";
                parameter.Add("@organization_id", orgId);
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> GetDynamicOwnedRefCount(int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select Count(distinct veh.id)
	                                   from master.vehicle veh
                                Left join master.orgrelationshipmapping  orm
                                on orm.vehicle_id=veh.id
                                Inner join master.orgrelationship ors
                                on ors.id=orm.relationship_id
                                 where ors.state='A'
                                and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                                else COALESCE(end_date,0) =0 end
							    and (orm.created_org_id=@organization_id or veh.organization_id=@organization_id)";
                parameter.Add("@organization_id", orgId);
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> GetDynamicVisibleRefCount(int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select count(distinct veh.id)
	                               from master.vehicle veh
                            Inner join master.orgrelationshipmapping  orm
                            on orm.vehicle_id=veh.id
                            Inner join master.orgrelationship ors
                            on ors.id=orm.relationship_id
                            where ors.state='A'
                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                            else COALESCE(end_date,0) =0 end
							and orm.target_org_id=@organization_id";
                parameter.Add("@organization_id", orgId);
                var count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}

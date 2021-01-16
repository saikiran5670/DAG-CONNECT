using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDataAccess dataAccess;
        public GroupRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        #region public methods
        public async Task<Group> Create(Group group)

        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@object_type", (char)group.ObjectType);
                parameter.Add("@group_type", (char)group.GroupType);
                parameter.Add("@argument", group.Argument);
                parameter.Add("@function_enum", (char)group.FunctionEnum);
                parameter.Add("@organization_id", group.OrganizationId);
                parameter.Add("@ref_id", group.RefId);
                parameter.Add("@name", group.Name);
                parameter.Add("@description", group.Description);
                string query = "insert into master.group(object_type, group_type, argument, function_enum, organization_id, ref_id, name, description) " +
                              "values(@object_type, @group_type, @argument, @function_enum, @organization_id, @ref_id, @name, @description) RETURNING id";

                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                group.Id = groupid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return group;
        }
        public async Task<Group> Update(Group group)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", group.Id);
                parameter.Add("@object_type", (char)group.ObjectType);
                parameter.Add("@group_type", (char)group.GroupType);
                parameter.Add("@argument", group.Argument);
                parameter.Add("@function_enum", (char)group.FunctionEnum);
                parameter.Add("@organization_id", group.OrganizationId);
                parameter.Add("@ref_id", group.RefId);
                parameter.Add("@name", group.Name);
                parameter.Add("@description", group.Description);

                var query = @"update master.group set object_type = @object_type,group_type = @group_type,
                                     argument = @argument,function_enum = @function_enum,
                                     organization_id = @organization_id,
                                     ref_id = @ref_id,
                                     name = @name,description = @description
	                                 WHERE id = @id
                                     RETURNING id;";
                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return group;
        }

        public async Task<bool> Delete(long groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", groupid);
                var query = @"delete from master.groupref where group_id = @id";
                await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                query = @"delete from master.group where id = @id";
                await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Group>> Get(GroupFilter groupFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Group> groupList = new List<Group>();
                var query = @"select id,object_type,group_type,argument,function_enum,organization_id,ref_id,name,description from master.group where 1=1 ";

                if (groupFilter != null)
                {
                    // group id filter
                    if (groupFilter.Id > 0)
                    {
                        parameter.Add("@id", groupFilter.Id);
                        query = query + " and id  = @id ";
                    }
                    // organization id filter
                    if (groupFilter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", groupFilter.OrganizationId);
                        query = query + " and organization_id  = @organization_id ";
                    }
                    // function functional enum filter
                    if (((char)groupFilter.FunctionEnum) != ((char)FunctionEnum.None))
                    {
                        parameter.Add("@function_enum", (char)groupFilter.FunctionEnum,DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and function_enum  = @function_enum ";
                    }

                    // object type filter
                    if (((char)groupFilter.ObjectType) != ((char)ObjectType.None))
                    {
                        parameter.Add("@object_type", (char) groupFilter.ObjectType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and object_type = @object_type ";
                    }

                    // group type filter
                    if (((char)groupFilter.GroupType) != ((char)GroupType.None))
                    {
                        parameter.Add("@group_type", (char)groupFilter.GroupType,DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and group_type = @group_type ";
                    }
                }
                IEnumerable<dynamic> groups = await dataAccess.QueryAsync<dynamic>(query, parameter);
                Group group = new Group();
                
                foreach (dynamic record in groups)
                {
                    group = Map(record);
                    if (groupFilter.GroupRef || groupFilter.GroupRefCount)
                    {
                        // group ref filter 
                        if (groupFilter.GroupRef)
                        {
                            group.GroupRef = GetRef(group.Id).Result;
                        }
                        // group ref filter 
                        if (groupFilter.GroupRefCount)
                        {
                            group.GroupRefCount = GetRefCount(group.Id).Result;
                        }
                    }
                    groupList.Add(group);
                }

                return groupList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddRef(Group group)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                var result = false;

                if (RemoveRef(group.Id))
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
                            await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        }
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region private methods
        private Group Map(dynamic record)
        {
            Group entity = new Group();
            entity.Id = record.id;
            entity.Name = record.name;
            entity.Description = record.description;
            entity.ObjectType = (ObjectType)Convert.ToChar(record.object_type);
            entity.GroupType = (GroupType)Convert.ToChar(record.group_type);
            entity.Argument = record.argument;
            entity.FunctionEnum = (FunctionEnum)Convert.ToChar(record.function_enum);
            entity.OrganizationId = record.organization_id;
            entity.RefId = record.ref_id;
            return entity;
        }
        public async Task<List<GroupRef>> GetRef(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select group_id,ref_id from master.groupref where group_id = @group_id";
                parameter.Add("@group_id", groupid);
                var groupref = await dataAccess.QueryAsync<GroupRef>(query, parameter);
                return groupref.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> GetRefCount(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select count(ref_id) as refcount from master.groupref where group_id = @group_id";
                parameter.Add("@group_id", groupid);
                var count = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool RemoveRef(int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", groupid);
                var query = @"delete from master.groupref where group_id = @id";
                dataAccess.ExecuteScalar<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

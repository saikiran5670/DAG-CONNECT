using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
namespace net.atos.daf.ct2.account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
            //SqlMapper.AddTypeHandler(new StringEnumTypeHandler<AccountType>());
        }
        public async Task<Account> Create(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();

                //parameter.Add("@id", account.Id);
                parameter.Add("@email", account.EmailId);
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                // this can null  as well
                // if (account.Dob.HasValue)
                // {
                //     //parameter.Add("@dob", UTCHandling.GetUTCFromDateTime(account.Dob.ToString()));
                //     parameter.Add("@dob", account.Dob);
                // }
                // else
                // {
                //     parameter.Add("@dob", DBNull.Value);
                // }


                parameter.Add("@type", (char)account.AccountType);

                string query = @"insert into master.account(email,salutation,first_name,last_name,type) " +
                              "values(@email,@salutation,@first_name,@last_name,@type) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                account.Id = id;
                if (account.Organization_Id > 0)
                {
                    parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(account.StartDate.ToString()));
                    if (account.EndDate.HasValue)
                    {
                        parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(account.EndDate.ToString()));
                    }
                    else
                    {
                        parameter.Add("@end_date", null);
                    }
                    parameter.Add("@is_active", true);
                    parameter.Add("@account_id", account.Id);
                    parameter.Add("@organization_Id", account.Organization_Id);
                    query = @"insert into master.accountorg(account_id,organization_id,start_date,end_date,is_active)  
                                   values(@account_id,@organization_Id,@start_date,@end_date,@is_active) RETURNING id";
                    account.Account_OrgId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return account;
        }
        public async Task<Account> Update(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", account.Id);
                parameter.Add("@email", account.EmailId);
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                //parameter.Add("@dob", account.Dob != null ? UTCHandling.GetUTCFromDateTime(account.Dob.ToString()) : 0);
                //parameter.Add("@dob", account.Dob != null ? account.Dob : null);
                parameter.Add("@type", (char)account.AccountType);

                string query = @"update master.account set id = @id,email = @email,salutation = @salutation,
                                first_name = @first_name,last_name = @last_name ,type = @type
                                where id = @id RETURNING id";

                account.Id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return account;
        }
        public async Task<bool> Delete(int accountid, int organization_id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", accountid);
                parameter.Add("@organization_id", organization_id);

                string query = @"update master.accountorg set is_active = 0 where account_id = @id and organization_id = @organization_id";

                var result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Account>> Get(AccountFilter filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                //List<Account> accounts = new List<Account>();
                List<Account> accounts = new List<Account>();
                string query = string.Empty;
                query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.dob,a.type as accounttype,ag.organization_id as Organization_Id from master.account a join master.accountorg ag on a.id = ag.account_id and ag.is_active=true where 1=1 ";
                if (filter != null)
                {
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and a.id=@id ";
                    }
                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        parameter.Add("@email", filter.Email);
                        query = query + " and a.email = @email ";
                    }
                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        parameter.Add("@name", filter.Name + "%");
                        query = query + " and (a.first_name || ' ' || a.last_name) like @name ";
                    }
                    // organization id filter
                    if (filter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", filter.OrganizationId);
                        query = query + " and ag.organization_id = @organization_id ";
                    }
                    // account type filter 
                    if (((char)filter.AccountType) != ((char)AccountType.None))
                    {
                        parameter.Add("@type", (char)filter.AccountType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);

                        query = query + " and a.type=@type";
                    }

                    // account ids filter                    
                    if ((!string.IsNullOrEmpty(filter.AccountIds)) && Convert.ToInt32(filter.AccountIds.Length) > 0)
                    {
                        // Account Id list Filter
                        filter.AccountIds = filter.AccountIds.TrimEnd(',');
                        List<int> accountids = filter.AccountIds.Split(',').Select(int.Parse).ToList();
                        parameter.Add("@accountids", accountids);
                        query = query + " and a.id = ANY(@accountids)";

                    }
                    dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);

                    foreach (dynamic record in result)
                    {
                        accounts.Add(Map(record));
                    }
                }
                return accounts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity)
        {
            try
            {
                var parameter = new DynamicParameters();

                //parameter.Add("@id", account.Id);
                parameter.Add("@access_type", (char)entity.AccessRelationType);
                parameter.Add("@account_group_id", entity.AccountGroupId);
                parameter.Add("@vehicle_group_id", entity.VehicleGroupId);

                string query = @"insert into master.accessrelationship(access_type,account_group_id,vehicle_group_id) " +
                              "values(@access_type,@account_group_id,@vehicle_group_id) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                entity.Id = id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entity;
        }
        // TODO: Update should delete existing relationship and insert new vehicle groups to account group
        public async Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", entity.Id);
                parameter.Add("@access_type", (char)entity.AccessRelationType);
                parameter.Add("@account_group_id", entity.AccountGroupId);
                parameter.Add("@vehicle_group_id", entity.VehicleGroupId);

                string query = @"update master.accessrelationship set access_type=@access_type,
                                account_group_id=@account_group_id,vehicle_group_id=@vehicle_group_id" +
                                " where id=@id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                entity.Id = id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entity;
        }
        public async Task<bool> DeleteAccessRelationship(int accountGroupId,int vehicleGroupId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_group_id", accountGroupId);
                parameter.Add("@vehicle_group_id", vehicleGroupId);
                string query = @"delete from master.accessrelationship where account_group_id=@account_group_id and vehicle_group_id=@vehicle_group_id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        public async Task<List<AccessRelationship>> GetAccessRelationship(AccessRelationshipFilter filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<AccessRelationship> entity = new List<AccessRelationship>();
                string query = string.Empty;

                //and gr.ref_id=1 ";
                if (filter != null)
                {
                    // id filter
                    if (filter.AccountId > 0)
                    {
                        query = @"select id,access_type,account_group_id,vehicle_group_id  
                        from master.accessRelationship ac
                        inner join master.groupref gr on ac.account_group_id = gr.group_id where 1= 1";
                        parameter.Add("@ref_id", filter.AccountId);
                        query = query + " and gr.ref_id=@ref_id ";
                    }
                    // organization id filter
                    else if (filter.AccountGroupId > 0)
                    {
                        query = @"select id,access_type,account_group_id,vehicle_group_id 
                                    from master.accessRelationship where account_group_id=@account_group_id";
                        parameter.Add("@account_group_id", filter.AccountGroupId);
                    }
                    dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                    //Account account;
                    foreach (dynamic record in result)
                    {
                        entity.Add(MapAccessRelationship(record));
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Begin Add Account to Role
        public async Task<bool> AddRole(AccountRole accountRoles)
        {
            bool result = false;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                bool execute=false;
                if (accountRoles != null)
                {
                    // check for roles
                    if (accountRoles != null && Convert.ToInt32(accountRoles.RoleIds.Count) > 0)
                    {
                        parameter.Add("@account_id", accountRoles.AccountId);
                        parameter.Add("@organization_id", accountRoles.OrganizationId);
                        parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(accountRoles.StartDate.ToString()));
                        parameter.Add("@end_date", accountRoles.EndDate);

                        query = @"insert into master.accountrole (account_id,organization_id,start_date,end_date,role_id) values ";
                        // get all roles
                        foreach (int roleid in accountRoles.RoleIds)
                        {
                            if (roleid > 0)
                            {
                                parameter.Add("@role_id_" + roleid.ToString(), roleid);
                                query = query + @" (@account_id,@organization_id,@start_date,@end_date,@role_id_" + roleid.ToString() + "),";
                                execute=true;
                            }
                        }
                        if (!string.IsNullOrEmpty(query) && execute)
                        {
                            query = query.TrimEnd(',');
                            await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        }
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<bool> RemoveRole(AccountRole accountRoles)
        {
            bool result = false;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (accountRoles != null)
                {
                    parameter.Add("@account_id", accountRoles.AccountId);
                    parameter.Add("@organization_id", accountRoles.OrganizationId);                    
                    query = @"delete from master.accountrole where account_id = @account_id and organization_id=@organization_id";
                    await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    result = true;
                    //TODO: Do we need to remove specified roles only.
                    // if (!string.IsNullOrEmpty(query))
                    // {
                    //     query = query.TrimEnd(',');
                    //     List<int> roleIds = accountRoles.RoleIds.ToList();
                    //     parameter.Add("@roleIds", roleIds);
                    //     query = query + " and role_id = ANY(@roleIds)";
                        
                    //     await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    // }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<List<int>> GetRoleAccounts(int roleId)
        {
            List<int> accountIds = null;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (roleId > 0)
                {
                    parameter.Add("@role_id", roleId);
                    query = @"select a.id from master.account a inner join master.accountrole ac on  a.id=ac.account_id
                            inner join master.role r on r.id=ac.role_id where ac.role_id=@role_id";
                    query = query.TrimEnd(',');
                    accountIds = new List<int>();
                    accountIds = await dataAccess.ExecuteScalarAsync<List<int>>(query, parameter);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accountIds;
        }
        public async Task<List<KeyValue>> GetRoles(AccountRole accountRole)
        {
            List<KeyValue> Roles = new List<KeyValue>();
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (accountRole != null)
                {
                    parameter.Add("@account_id", accountRole.AccountId);
                    parameter.Add("@organization_id", accountRole.OrganizationId);
                    query = @"select r.id,r.name from master.account a inner join master.accountrole ac on a.id = ac.account_id 
                                    inner join master.role r on r.id = ac.role_id where 
                                    ac.account_id = @account_id and ac.organization_id=@organization_id";
                    dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                    foreach (dynamic record in result)
                    {
                        Roles.Add(new KeyValue(){ Id = record.id, Name = record.name});
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Roles;
        }
        // End Add Account to Role

        // Begig - Account rendering

        public async Task<List<KeyValue>> GetAccountOrg(int accountId)
        {
            List<KeyValue> keyValueList = null;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (accountId > 0)
                {
                    parameter.Add("@account_id", accountId);
                    query = @"select o.id,o.name from master.organization o inner join master.accountorg ao on o.id=ao.organization_id and ao.is_active=true where ao.account_id=@account_id";                    
                    keyValueList = await dataAccess.ExecuteScalarAsync<List<KeyValue>>(query, parameter);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return keyValueList;
        }
        public async Task<List<KeyValue>> GetAccountRole(int accountId)
        {
            List<KeyValue> keyValueList = null;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (accountId > 0)
                {
                    parameter.Add("@account_id", accountId);
                    query = @"select r.id,r.name from master.role r inner join master.accountrole ac on r.id=ac.role_id and r.is_active=true where ac.account_id=@account_id";
                    keyValueList = await dataAccess.ExecuteScalarAsync<List<KeyValue>>(query, parameter);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return keyValueList;
        }
        // End - Account Rendering


        private Account Map(dynamic record)
        {
            Account account = new Account();
            account.Id = record.id;
            account.EmailId = record.email;
            account.Salutation = record.salutation;
            account.FirstName = record.first_name;
            account.LastName = record.last_name;
            account.Dob = record.dob;
            account.Organization_Id = record.organization_id;
            account.AccountType = (AccountType)Convert.ToChar(record.accounttype);
            return account;
        }
        private AccessRelationship MapAccessRelationship(dynamic record)
        {
            AccessRelationship entity = new AccessRelationship();
            entity.Id = record.id;
            entity.AccessRelationType = (AccessRelationType)Convert.ToChar(record.access_type);
            entity.AccountGroupId = record.account_group_id;
            entity.VehicleGroupId = record.vehicle_group_id;
            return entity;
        }

    }

}

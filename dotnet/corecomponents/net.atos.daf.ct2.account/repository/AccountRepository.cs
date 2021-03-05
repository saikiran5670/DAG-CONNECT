using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using System.Text;

namespace net.atos.daf.ct2.account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        #region Account
        public async Task<Account> Create(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();

                //parameter.Add("@id", account.Id);
                parameter.Add("@email", String.IsNullOrEmpty(account.EmailId) ? account.EmailId : account.EmailId.ToLower());
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                parameter.Add("@type", (char)account.AccountType);
                parameter.Add("@driver_id", account.DriverId);
                parameter.Add("@created_at", account.CreatedAt.Value);

                string query = @"insert into master.account(email,salutation,first_name,last_name,type,driver_id,is_active,preference_id,blob_id,created_at) " +
                              "values(@email,@salutation,@first_name,@last_name,@type,@driver_id,true,null,null,@created_at) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                account.Id = id;
                if (account.Organization_Id > 0)
                {
                    if (account.StartDate == null || account.StartDate <= 0) account.StartDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                    account.EndDate = null;
                    parameter.Add("@start_date", account.StartDate);
                    if (account.EndDate.HasValue)
                    {
                        parameter.Add("@end_date", account.EndDate);
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
                    var AccountOrgId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
                parameter.Add("@email", String.IsNullOrEmpty(account.EmailId) ? account.EmailId : account.EmailId.ToLower());
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                parameter.Add("@type", (char)account.AccountType);
                parameter.Add("@driver_id", account.DriverId);
                string query = @"update master.account set email = @email,salutation = @salutation,
                                first_name = @first_name,last_name = @last_name ,driver_id=@driver_id, type = @type
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
                string query = string.Empty;
                int result = 0;

                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // check in user need to delete 
                    // Delete Account Group Reference
                    query = @"delete from master.groupref gr
                         using master.group g,master.accountorg ao 
                         where gr.ref_id = @id and ao.organization_id = @organization_id 
                         and g.id=gr.group_id and ao.is_active=true";
                    result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    // Delete account role
                    query = @"delete from master.accountrole where account_id = @id and organization_id = @organization_id;";
                    result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    // disable account with organization
                    query = @"update master.accountorg set is_active=false where account_id = @id and organization_id = @organization_id";
                    result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    transactionScope.Complete();
                }
                // check if account associated with multiple organization
                query = @"select count(1) from master.accountorg where is_active=true and account_id = @id";
                result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (result <= 0)
                {
                    // disable preference
                    query = @"update master.accountpreference set is_active=false from master.account where master.accountpreference.id=master.account.preference_id and master.account.id=@id;";
                    //query += @"delete from master.accountblob ab using master.account a where a.id = @id and a.blob_id = ab.id and a.is_active = true;";
                    result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    // disable account 
                    query = @"update master.account set is_active=false where id = @id;";
                    result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                return true;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Account> Duplicate(AccountFilter filter)
        {
            var parameter = new DynamicParameters();
            Account account = null;
            string query = string.Empty;
            try
            {
                query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.driver_id,a.type as accounttype,ag.organization_id as 
                Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.is_active=true 
                and ag.is_active=true where 1=1 ";

                // organization id filter
                if (filter.OrganizationId > 0)
                {
                    parameter.Add("@organization_id", filter.OrganizationId);
                    query = query + " and ag.organization_id = @organization_id ";
                }
                // email id filter
                if (!string.IsNullOrEmpty(filter.Email))
                {
                    parameter.Add("@email", filter.Email.ToLower());
                    query = query + " and LOWER(a.email) = @email ";
                }
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                foreach (dynamic record in result)
                {

                    account = Map(record);
                    account.isDuplicateInOrg = true;
                }
                // account is part of other organization
                if (account == null)
                {
                    query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.driver_id,a.type as accounttype,ag.organization_id as 
                    Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.is_active=true 
                    and ag.is_active=true where 1=1 ";

                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        parameter.Add("@email", filter.Email.ToLower());
                        query = query + " and LOWER(a.email) = @email ";
                    }
                    query = query + "limit 1";
                    result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                    foreach (dynamic record in result)
                    {

                        account = Map(record);
                        account.isDuplicate = true;                        
                    }
                }
                return account;
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
                query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.driver_id,a.type as accounttype,ag.organization_id as 
                Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.is_active=true 
                and ag.is_active=true where 1=1 ";

                if (filter != null)
                {
                    // organization id filter
                    if (filter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", filter.OrganizationId);
                        query = query + " and ag.organization_id = @organization_id ";
                    }
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and a.id=@id ";
                    }
                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        parameter.Add("@email", filter.Email.ToLower());
                        query = query + " and LOWER(a.email) = @email ";
                    }
                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        parameter.Add("@name", filter.Name + "%");
                        query = query + " and (a.first_name || ' ' || a.last_name) like @name ";
                    }

                    //// account type filter 
                    //if (((char)filter.AccountType) != ((char)AccountType.None))
                    //{
                    //    parameter.Add("@type", (char)filter.AccountType, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);

                    //    query = query + " and a.type=@type";
                    //}

                    // account ids filter                    
                    if ((!string.IsNullOrEmpty(filter.AccountIds)) && Convert.ToInt32(filter.AccountIds.Length) > 0)
                    {
                        // Account Id list Filter
                        filter.AccountIds = filter.AccountIds.TrimEnd(',');
                        List<int> accountids = filter.AccountIds.Split(',').Select(int.Parse).ToList();
                        parameter.Add("@accountids", accountids);
                        query = query + " and a.id = ANY(@accountids)";
                    }
                    // account group filter
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

        public async Task<Account> AddAccountToOrg(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                parameter.Add("@account_id", account.Id);
                parameter.Add("@organization_Id", account.Organization_Id);
                parameter.Add("@start_date", account.StartDate);
                if (account.EndDate.HasValue)
                {
                    parameter.Add("@end_date", account.EndDate);
                }
                else
                {
                    parameter.Add("@end_date", null);
                }
                query = @"insert into master.accountorg(account_id,organization_id,start_date,end_date,is_active)  
                                   values(@account_id,@organization_Id,@start_date,@end_date,true) RETURNING id";
                var AccountOrgId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                account.Id = AccountOrgId;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return account;
        }
        #endregion

        #region AccountBlob
        public async Task<AccountBlob> CreateBlob(AccountBlob accountBlob)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                parameter.Add("@id", accountBlob.Id);
                parameter.Add("@account_id", accountBlob.AccountId);
                parameter.Add("@image_type", (char)accountBlob.Type);
                parameter.Add("@image", accountBlob.Image);

                // new profile picture
                if (accountBlob.Id > 0)
                {
                    query = @"update master.accountblob set image_type=@image_type,image=@image where id=@id RETURNING id";
                    var blobId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                else // update profile picture
                {
                    query = @"insert into master.accountblob(image_type,image) values(@image_type,@image) RETURNING id";
                    var blobId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    accountBlob.Id = blobId;
                    if (blobId > 0)
                    {
                        parameter.Add("@blob_id", blobId);
                        query = "update master.account set blob_id=@blob_id where id=@account_id";
                        await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accountBlob;
        }
        public async Task<AccountBlob> GetBlob(int blobId)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                AccountBlob accountBlob = null;
                if (blobId <= 0) return accountBlob;

                parameter.Add("@id", blobId);
                query = @"select id,image_type,image from master.accountblob where id=@id";
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                if (Enumerable.Count(result) > 0)
                {
                    accountBlob = new AccountBlob();
                    foreach (dynamic record in result)
                    {
                        accountBlob = ToBlob(record);
                    }
                }
                return accountBlob;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Account Access Relationship
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
                string query = string.Empty;
                int id = 0;
                parameter.Add("@id", entity.Id);
                parameter.Add("@access_type", (char)entity.AccessRelationType);
                parameter.Add("@account_group_id", entity.AccountGroupId);
                parameter.Add("@vehicle_group_id", entity.VehicleGroupId);
                if (entity.Id > 0)
                {
                    query = @"update master.accessrelationship set access_type=@access_type
                                ,vehicle_group_id=@vehicle_group_id where id=@id RETURNING id";
                    id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    entity.Id = id;
                }
                // else
                // {
                //     query = @"update master.accessrelationship set access_type=@access_type
                //                 ,vehicle_group_id=@vehicle_group_id where account_group_id=@account_group_id and vehicle_group_id=@vehicle_group_id";
                //     id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                //     entity.Id = id;
                // }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entity;
        }
        public async Task<bool> DeleteAccessRelationship(int accessId, int vehicleGroupId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", accessId);
                //parameter.Add("@vehicle_group_id", vehicleGroupId);
                string query = @"delete from master.accessrelationship where id=@id";
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
                    // account group id filter
                    else if (filter.AccountGroupId > 0)
                    {
                        query = @"select id,access_type,account_group_id,vehicle_group_id 
                                    from master.accessRelationship where account_group_id=@account_group_id";
                        parameter.Add("@account_group_id", filter.AccountGroupId);
                    }
                    // vehicle group filter 
                    else if (filter.VehicleGroupId > 0)
                    {
                        query = @"select id,access_type,account_group_id,vehicle_group_id 
                                    from master.accessRelationship where vehicle_group_id=@vehicle_group_id";
                        parameter.Add("@vehicle_group_id", filter.VehicleGroupId);
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
        #endregion

        #region Account Role

        // Begin Add Account to Role
        public async Task<bool> AddRole(AccountRole accountRoles)
        {
            bool result = false;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                bool execute = false;
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
                                execute = true;
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
                    query = @"select a.id from master.account a inner join master.accountrole ac on  a.id=ac.account_id inner join master.role r on r.id=ac.role_id where ac.role_id=@role_id";
                    accountIds = new List<int>();
                    dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                    if (result is int)
                    {
                        accountIds.Add(result);
                    }
                    else
                    {
                        foreach (dynamic record in result)
                        {
                            accountIds.Add(record.id);
                        }
                    }
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
                        Roles.Add(new KeyValue() { Id = record.id, Name = record.name });
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
        #endregion

        #region AccountOrg
        // Begin - Account rendering

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
                    IEnumerable<KeyValue> result = await dataAccess.QueryAsync<KeyValue>(query, parameter);
                    keyValueList = result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return keyValueList;
        }
        public async Task<List<AccountOrgRole>> GetAccountRole(int accountId)
        {
            List<AccountOrgRole> AccountOrgRoleList = null;
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (accountId > 0)
                {
                    parameter.Add("@account_id", accountId);
                    query = @"select r.id,r.name,r.id,ac.organization_id as Organization_Id from master.role r inner join master.accountrole ac on r.id=ac.role_id and r.is_active=true where ac.account_id=@account_id";
                    IEnumerable<AccountOrgRole> result = await dataAccess.QueryAsync<AccountOrgRole>(query, parameter);
                    AccountOrgRoleList = result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AccountOrgRoleList;
        }
        // End - Account Rendering
        #endregion

        #region Private Methods
        private AccountBlob ToBlob(dynamic record)
        {
            AccountBlob accountBlob = new AccountBlob();
            accountBlob.Id = record.id;
            accountBlob.Type = (ImageType)Convert.ToChar(record.image_type);
            accountBlob.Image = record.image;
            return accountBlob;

        }
        private Account Map(dynamic record)
        {
            Account account = new Account();
            account.Id = record.id;
            account.EmailId = record.email;
            account.Salutation = record.salutation;
            account.FirstName = record.first_name;
            account.LastName = record.last_name;
            account.Organization_Id = record.organization_id;
            account.AccountType = (AccountType)Convert.ToChar(record.accounttype);
            if ((object)record.preference_id != null)
                account.PreferenceId = (int)record.preference_id;
            if ((object)record.blob_id != null) account.BlobId = (int)record.blob_id;
            if ((object)record.driver_id != null) account.DriverId = record.driver_id;
            account.CreatedAt = null;
            if ((object)record.created_at != null)
            {
                account.CreatedAt = record.created_at;
                //account.CreatedAt = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.created_at, "America/New_York", "yyyy-MM-ddTHH:mm:ss"));
            }
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
        #endregion
    }

}

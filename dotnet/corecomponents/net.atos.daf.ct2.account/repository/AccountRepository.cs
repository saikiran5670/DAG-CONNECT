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
                if (account.Dob.HasValue)
                {
                    //parameter.Add("@dob", UTCHandling.GetUTCFromDateTime(account.Dob.ToString()));
                    parameter.Add("@dob", account.Dob);
                }
                else
                {
                    parameter.Add("@dob", DBNull.Value);
                }


                parameter.Add("@type", (char)account.AccountType);

                string query = @"insert into master.account(email,salutation,first_name,last_name,dob,type) " +
                              "values(@email,@salutation,@first_name,@last_name,@dob,@type) RETURNING id";

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
                    parameter.Add("@is_active", account.Active);
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
                parameter.Add("@dob", account.Dob != null ? account.Dob : null);
                parameter.Add("@type", (char)account.AccountType);

                string query = @"update master.account set id = @id,email = @email,salutation = @salutation,
                                first_name = @first_name,last_name = @last_name ,dob = @dob,type = @type)
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
                string query = @"delete from master.accountorg where account_id = @id and organization_id = @organization_id";
                var result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (accountid > 0)
                {
                    query = @"delete from master.account where id = @id";
                    await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
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
                query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.dob,a.type as accounttype,ag.organization_id as Organization_Id from master.account a join master.accountorg ag on a.id = ag.account_id where 1=1 ";
                if (filter != null)
                {
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and a.id = @id ";
                    }
                    // organization id filter
                    if (filter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", filter.OrganizationId);
                        query = query + " and ag.organization_id = @organization_id ";
                    }
                    // account type filter 
                    if (((char)filter.AccountType) != ((char) AccountType.None))
                    {
                        parameter.Add("@type", (char)filter.AccountType);
                        query = query + " and a.type = @type";
                    }

                    // account ids filter                    
                    if ((!string.IsNullOrEmpty(filter.AccountIds)) && Convert.ToInt32(filter.AccountIds.Length) > 0)
                    {
                        // Account Id list Filter                       
                        List<int> accountids = filter.AccountIds.Split(',').Select(int.Parse).ToList();
                        parameter.Add("@accountids", accountids);
                        query = query + " and a.id = ANY(@accountids) ";
                        
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
                parameter.Add("@access_type", (char) entity.AccessRelationType);
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
         public async Task<AccessRelationship> UpdateAccessRelationship(AccessRelationship entity)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@access_type", (char) entity.AccessRelationType);
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
                        query = query + " and gr.ref_id = @ref_id ";
                    }
                    // organization id filter
                    else if (filter.AccountGroupId > 0)
                    {
                        query = @"select id,access_type,account_group_id,vehicle_group_id 
                                    from master.accessRelationship where ac.account_group_id=@account_group_id";
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

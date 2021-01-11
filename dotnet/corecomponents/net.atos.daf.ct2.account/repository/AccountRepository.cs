using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.utilities;
namespace net.atos.daf.ct2.account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
            SqlMapper.AddTypeHandler(new StringEnumTypeHandler<AccountType>());
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
                //parameter.Add("@dob", account.Dob != null ? UTCHandling.GetUTCFromDateTime(account.Dob.ToString()) : 0);
                parameter.Add("@dob", DateTimeOffset.Parse(account.Dob.ToString(),null));                
                parameter.Add("@type", (char) account.AccountType);                               
                
                string query= @"insert into master.account(email,salutation,first_name,last_name,dob,type) " +
                              "values(@email,@salutation,@first_name,@last_name,@dob,@type) RETURNING id";

                var id =   await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                account.Id = id;
                if(account.Organization_Id > 0)
                {
                    parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(account.StartDate.ToString()));
                    parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(account.EndDate.ToString()));
                    parameter.Add("@is_active", account.Active); 
                    parameter.Add("@account_id", account.Id);
                    parameter.Add("@organization_Id", account.Organization_Id);
                    
                    query= @"insert into master.accountorg(account_id,organization_id,start_date,end_date,is_active)  
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
                parameter.Add("@dob", account.Dob != null ? UTCHandling.GetUTCFromDateTime(account.Dob.ToString()) : 0);
                parameter.Add("@type", (char) account.AccountType);
                
                string query= @"update master.account set id = @id,email = @email,salutation = @salutation,
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
        public async Task<bool> Delete(int accountid,int organization_id)
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
                IEnumerable<Account> accounts = null;
                var query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.type as accounttype,ag.organization_id as Organization_Id from master.account a join master.accountorg ag on a.id = ag.account_id where 1=1 ";
                if(filter != null)
                {
                    // id filter
                    if(filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and a.id = @id ";                        
                    }
                     // organization id filter
                    if(filter.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", filter.OrganizationId);
                        query = query + " and ag.organization_id = @organization_id ";                        
                    }
                    // account type filter 
                    if (((char)filter.AccountType ) != ((char)AccountType.None))
                    {
                        parameter.Add("@type", (char) filter.AccountType);
                        query = query + " and a.type = @type";                        
                    }
                    
                   accounts = await dataAccess.QueryAsync<Account>(query, parameter);
                    // Account account;
                    // foreach (dynamic record in result)
                    // {
                    //      account = new Account();
                    //      account.Id = record.id;
                    //      account.AccountType = Enum.Parse(typeof(AccountType), record.accounttype);
                    //      accounts.Add(account);
                    // }
                    
                }                
                return accounts.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }   
        }

    }

    public class StringEnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, IConvertible
    {
    static StringEnumTypeHandler()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumeration type");
        }
    }

    public override T Parse(object value)
    {
        return (T)Enum.Parse(typeof(T), Convert.ToString(value));
    }

    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.AnsiString;
    }
    }
}

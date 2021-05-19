using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.data;
using Dapper;

namespace net.atos.daf.ct2.identitysession.repository
{
    public class AccountAssertionRepository : IAccountAssertionRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountAssertionRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<int> InsertAssertion(AccountAssertion accountAssertion)//TODO Use bulk insertion
        {
            try
            {          

            //Insert query in account assertion
            var QueryStatement = @"INSERT INTO master.accountassertion
                                      (
                                        key,
                                        value,
                                        account_id,
                                        session_id,
                                        created_at)                                        
                            	VALUES(
                                       @key 
                                      ,@value
                                      ,@account_id
                                      ,@session_id
                                      ,@created_at                                      
                                     ) RETURNING account_id";

            var parameter = new DynamicParameters();
            
            parameter.Add("@key", accountAssertion.Key);
            parameter.Add("@value", accountAssertion.Value);
            parameter.Add("@account_id", Convert.ToInt32(accountAssertion.AccountId));
            parameter.Add("@session_id", accountAssertion.Session_Id);           
            parameter.Add("@created_at", Convert.ToInt64(accountAssertion.CreatedAt)); 

            int result_CheckDuplicate =await CheckDuplicateKeyValue(accountAssertion.Key,accountAssertion.Value,Convert.ToInt32(accountAssertion.AccountId));

            int InsertedAccountAssertionId=0;   
            if(result_CheckDuplicate==0)  
            {
               InsertedAccountAssertionId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);   
            } 
            else
            {
               InsertedAccountAssertionId=await UpdateAssertion(accountAssertion);
            }          
           
            return InsertedAccountAssertionId;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private async Task<int> CheckDuplicateKeyValue(string Key,string Value,int AccountId)
        {
             //Query for check duplication value and key        
            var QueryStatement_select = @"SELECT 
                                    count(*)
                                    FROM master.accountassertion
                                    WHERE account_id=@account_id
                                    AND key=@key
                                    AND value=@value;";

            var parameter = new DynamicParameters();            
            parameter.Add("@key", Key);
            parameter.Add("@value", Value);
            parameter.Add("@account_id", AccountId);

            int result_CheckDuplicate = await dataAccess.ExecuteScalarAsync<int>(QueryStatement_select, parameter); 

            return result_CheckDuplicate;            
        }

        public async Task<int> UpdateAssertion(AccountAssertion accountAssertion)
        {      
            try
            { 
                var QueryStatement=@"UPDATE master.accountassertion
                                    SET                                
                                    key=@key, 
                                    value=@value,
                                    account_id=@account_id, 
                                    session_id=@session_id, 
                                    created_at=@created_at
                                    WHERE account_id=@account_id
                                    AND session_id=@session_id
                                    AND key=@key
                                    AND value=@value
                                    RETURNING account_id;";

                var parameter = new DynamicParameters();                
                parameter.Add("@key", accountAssertion.Key);
                parameter.Add("@value", accountAssertion.Value);
                parameter.Add("@account_id", Convert.ToInt32(accountAssertion.AccountId));
                parameter.Add("@session_id", accountAssertion.Session_Id);           
                parameter.Add("@created_at", Convert.ToInt64(accountAssertion.CreatedAt));                    

                int accountassertionId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                return accountassertionId;
            }
            catch(Exception ex)
            {
                throw;
            }

        }
        public async Task<int> DeleteAssertion(int accountId)
        {
            try
            {
                var QueryStatement=@"DELETE FROM master.accountassertion	                           
                                    WHERE account_id=@account_id
                                    RETURNING account_id;";

                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);    
                
                int account_Id = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                return account_Id;
            }
            catch(Exception ex)
            {
                 throw;   
            }
        }

        public async Task<int> DeleteAssertionbySessionId(int sessionId)
        {
            try
            {
                var QueryStatement=@"DELETE FROM master.accountassertion	                           
                                    WHERE session_id=@session_id
                                    RETURNING session_id;";
                                    

                var parameter = new DynamicParameters();
                parameter.Add("@session_id", sessionId);    
                
                int session_Id = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                return session_Id;
            }
            catch(Exception ex)
            {
                 throw;   
            }
        }

        public async Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId)
        {
            try
            {
                var QueryStatement=@"SELECT                                     
                                    key, 
                                    value, 
                                    account_id, 
                                    session_id, 
                                    created_at
                                    FROM master.accountassertion
                                    WHERE account_id=@account_id;";

                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                
                List<AccountAssertion> accountAssertions = new List<AccountAssertion>();

                dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                
                foreach (dynamic record in result)
                {                    
                    accountAssertions.Add(Map(record));
                }
                return accountAssertions.AsEnumerable();  
            }
            catch(Exception ex)
            {
                throw;
            }                  
        }       


        private AccountAssertion Map(dynamic record)
        {
            AccountAssertion entity = new AccountAssertion();            
            entity.Key = record.key;
            entity.Value = record.value;
            entity.AccountId =Convert.ToString(record.account_id);
            entity.Session_Id = record.session_id;
            entity.CreatedAt =Convert.ToString(record.created_at);           
            return entity;
        }
    }
}
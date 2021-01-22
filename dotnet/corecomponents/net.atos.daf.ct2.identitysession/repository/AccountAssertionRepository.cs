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
        public async Task<int> InsertAssertion(AccountAssertion accountAssertion)
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
                                     )";

            var parameter = new DynamicParameters();
            
            parameter.Add("@key", accountAssertion.Key);
            parameter.Add("@value", accountAssertion.Value);
            parameter.Add("@account_id", accountAssertion.AccountId);
            parameter.Add("@session_id", accountAssertion.SessionState);           
            parameter.Add("@created_at", accountAssertion.CreatedAt);
           
            int InsertedAccountAssertionId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);          
           
            return InsertedAccountAssertionId;
        }

        public async Task<int> UpdateAssertion(AccountAssertion accountAssertion)
        {            

            var QueryStatement=@"UPDATE master.accountassertion
	                            SET                                
                                key=@key, 
                                value=@value,
                                account_id=@account_id, 
                                session_id=@session_id, 
                                created_at=@created_at
	                            WHERE id=@id
                                RETURNING id;";

            var parameter = new DynamicParameters();
            parameter.Add("@key", accountAssertion.Key);
            parameter.Add("@value", accountAssertion.Value);
            parameter.Add("@account_id", accountAssertion.AccountId);
            parameter.Add("@session_id", accountAssertion.SessionState);           
            parameter.Add("@created_at", accountAssertion.CreatedAt);                    

            int accountassertionId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            return accountassertionId;

        }
        public async Task<int> DeleteAssertion(int accountId)
        {
            var QueryStatement=@"DELETE FROM master.accountassertion	                           
	                            WHERE account_id=@account_id
                                RETURNING id;";

            var parameter = new DynamicParameters();
            parameter.Add("@account_id", accountId);    
            
            int accountassertionId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            return accountassertionId;
        }

        public async Task<IEnumerable<AccountAssertion>> GetAssertion(int accountId)
        {
            var QueryStatement=@"SELECT 
                                id, 
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
                accountAssertions.Add(record);
            }
            return accountAssertions.AsEnumerable();                    
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.identitysession.ENUM;

namespace net.atos.daf.ct2.identitysession.repository
{
    public class AccountTokenRepository : IAccountTokenRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountTokenRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
         public async Task<int> InsertToken(AccountToken accountToken)
        {
            try
            {
                var QueryStatement = @"INSERT INTO  master.accounttoken 
                                        (
                                         user_name
                                        ,access_token
                                        ,expire_in                                        
                                        ,account_id
                                        ,type                                        
                                        ,scope
                                        ,idp_type
                                        ,created_at
                                        ,token_id
                                        ,session_id
                                        ,role_id
                                        ,organization_id
                                        ) 
                                    VALUES(
                                        @user_name
                                        ,@access_token
                                        ,@expire_in
                                        ,@account_id
                                        ,@type                                        
                                        ,@scope
                                        ,@idp_type
                                        ,@created_at
                                        ,@token_id
                                        ,@session_id    
                                        ,@role_id
                                        ,@organization_id
                                        )RETURNING id";

                var parameter = new DynamicParameters();
                
                parameter.Add("@user_name", accountToken.UserName);
                parameter.Add("@access_token", accountToken.AccessToken);
                parameter.Add("@expire_in", accountToken.ExpireIn);
                parameter.Add("@account_id", accountToken.AccountId);
                parameter.Add("@type", (char)accountToken.TokenType);
                parameter.Add("@scope", accountToken.Scope);
                parameter.Add("@idp_type", (char)accountToken.IdpType);
                parameter.Add("@created_at", accountToken.CreatedAt);
                parameter.Add("@token_id", Guid.Parse(accountToken.TokenId));
                parameter.Add("@session_id", accountToken.Session_Id);
                parameter.Add("@role_id", accountToken.RoleId);
                parameter.Add("@organization_id", accountToken.OrganizationId);

                int tokenId =await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return tokenId;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }
        public async Task<int> DeleteToken(List<string> token_Id)//DONE (testing pending) Parameter change list<token_id> 
        {
            try
            {
                int accountID=0;
                foreach(string item in token_Id)
                {
                   accountID=await DeleteTokenByTokenId(Guid.Parse(item));
                }
              
                return accountID;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<int> DeleteTokenByTokenId(Guid tokenID)
        {
            var QueryStatement = @"DELETE FROM
                                        master.accounttoken 
                                        where token_id in (@token_id)
                                        RETURNING account_id";
                var parameter = new DynamicParameters();
                parameter.Add("@token_id",tokenID);
                int Id= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return Id;
        }

        public async Task<int> DeleteTokenbySessionId(int sessionId)
        {
            try
            {
                var QueryStatement = @"DELETE FROM
                                        master.accounttoken 
                                        where session_id=@session_id
                                        RETURNING session_id";
                var parameter = new DynamicParameters();
                parameter.Add("@session_id",sessionId);
                int session_Id= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return session_Id;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteTokenbyAccountId(int sessionId)
        {
            try
            {
                var QueryStatement = @"DELETE FROM
                                        master.accounttoken 
                                        where account_id=@account_id
                                        RETURNING account_id";
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", sessionId);
                int account_Id= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return account_Id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID)
        {
            try
            {
                var QueryStatement = @"select 
                                        id
                                        ,user_name
                                        ,access_token
                                        ,expire_in
                                        ,account_id
                                        ,type
                                        ,session_id
                                        ,scope
                                        ,idp_type
                                        ,created_at
                                        ,token_id
                                        ,session_id
                                        ,case when role_id is null  then 0 else role_id end as RoleId
                                        ,case when organization_id is null then 0 else organization_id end as OrganizationId
                                        from master.accounttoken 
                                        where account_id=@AccountID";
                var parameter = new DynamicParameters();
            
                parameter.Add("@AccountID", AccountID);
                dynamic accounttoken = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                
                List<AccountToken> accountTokenList = new List<AccountToken>();

                foreach (dynamic record in accounttoken)
                {                    
                    accountTokenList.Add(Map(record));
                }
                return accountTokenList.AsEnumerable();
                
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(string TokenId)
        {
            try 
            {
                var QueryStatement = @"select 
                                        id
                                        ,user_name
                                        ,access_token
                                        ,expire_in
                                        ,account_id
                                        ,type
                                        ,session_id
                                        ,scope
                                        ,idp_type
                                        ,created_at
                                        ,token_id
                                        ,session_id
                                        ,case when role_id is null  then 0 else role_id end as RoleId
                                        ,case when organization_id is null then 0 else organization_id end as OrganizationId
                                        from master.accounttoken 
                                        where token_id=@token_id";
                var parameter = new DynamicParameters();
            
                parameter.Add("@token_id", Guid.Parse(TokenId));
                dynamic accounttoken = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<AccountToken> accountTokenList = new List<AccountToken>();

                foreach (dynamic record in accounttoken)
                {                    
                    accountTokenList.Add(Map(record));
                }
                return accountTokenList.AsEnumerable();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> ValidateToken(string TokenId)
        {
            try
            {
                long currentUTCFormate=UTCHandling.GetUTCFromDateTime(DateTime.Now);
                var QueryStatement = @"select
                                       count(id)
                                       from master.accounttoken 
                                       where token_id=@token_id
                                       AND expire_in < @expire_in;";//DONE (testing pending)utc time with grether than datetime.now() with expireat
                var parameter = new DynamicParameters();
            
                parameter.Add("@token_id",Guid.Parse(TokenId));
                parameter.Add("@expire_in",currentUTCFormate);
                int accounttoken = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                bool isValideToken=accounttoken>0;
                
                return isValideToken;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<int> GetTokenCount(int AccountID)
        {
            try
            {
                var QueryStatement = @"select 
                                        count(id)
                                        from master.accounttoken 
                                        where account_id=@AccountID";
                var parameter = new DynamicParameters();
                parameter.Add("@AccountID", AccountID);
                return await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        private AccountToken Map(dynamic record)
        {
            AccountToken entity = new AccountToken();
            entity.Id=record.id;
            entity.UserName= record.user_name;
            entity.AccessToken=record.access_token;
            entity.ExpireIn=record.expire_in;
            entity.AccountId=record.account_id;
            entity.TokenType = (TokenType)Convert.ToChar(record.type);
            entity.Scope=record.scope;
            entity.IdpType = (IDPType)Convert.ToChar(record.idp_type);
            entity.CreatedAt=record.created_at;
            entity.TokenId = Convert.ToString(record.token_id);
            entity.Session_Id = record.session_id;
            entity.RoleId= record.roleid;
            entity.OrganizationId= record.organizationid;
            return entity;
        }

    }
}
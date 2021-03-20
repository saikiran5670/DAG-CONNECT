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
                                        ,refresh_token
                                        ,refresh_expire_in
                                        ,account_id
                                        ,type
                                        ,session_id
                                        ,scope
                                        ,idp_type
                                        ,created_at
                                        ,token_id
                                        ) 
                                    VALUES(
                                        @user_name
                                        ,@access_token
                                        ,@expire_in
                                        ,@refresh_token
                                        ,@refresh_expire_in
                                        ,@account_id
                                        ,@type
                                        ,@session_id
                                        ,@scope
                                        ,@idp_type
                                        ,@created_at
                                        ,@token_id)RETURNING id";

                var parameter = new DynamicParameters();
                
                parameter.Add("@user_name", accountToken.UserName);
                parameter.Add("@access_token", accountToken.AccessToken);
                parameter.Add("@expire_in", accountToken.ExpireIn);
                parameter.Add("@refresh_token", accountToken.RefreshToken);
                parameter.Add("@refresh_expire_in", accountToken.RefreshExpireIn); 
                parameter.Add("@account_id", accountToken.AccountId);
                parameter.Add("@type", accountToken.TokenType);
                parameter.Add("@session_id", accountToken.Session_Id);
                parameter.Add("@scope", accountToken.Scope);
                parameter.Add("@idp_type", accountToken.IdpType);
                parameter.Add("@created_at", accountToken.CreatedAt);
                parameter.Add("@token_id", accountToken.TokenId);

                int tokenId=await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return tokenId;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<int> DeleteToken(List<string> token_Id)//DONE (testing pending) Parameter change list<token_id> 
        {
            try
            {
                int accountID=0;
                foreach(string item in token_Id)
                {
                   accountID=await DeleteTokenByTokenId( new Guid(item));
                }
              
                return accountID;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<int> DeleteTokenByTokenId(Guid tokenID)
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
                throw ex;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID)
        {
            try
            {
                var QueryStatement = @"select token_id
                                        ,user_name
                                        ,access_token
                                        ,expire_in
                                        ,refresh_token
                                        ,refresh_expire_in
                                        ,account_id
                                        ,type
                                        ,session_id
                                        ,scope
                                        ,idp_type
                                        ,created_at
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
                throw ex;
            }
        }
        public async Task<IEnumerable<AccountToken>> GetTokenDetails(string TokenId)
        {
            try 
            {
                var QueryStatement = @"select token_id
                                        ,user_name
                                        ,access_token
                                        ,expire_in
                                        ,refresh_token
                                        ,refresh_expire_in
                                        ,account_id
                                        ,type
                                        ,session_id
                                        ,scope
                                        ,idp_type
                                        ,created_at
                                        from master.accounttoken 
                                        where token_id=@token_id";
                var parameter = new DynamicParameters();
            
                parameter.Add("@token_id",new Guid(TokenId));
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
                throw ex;
            }
        }
        public async Task<bool> ValidateToken(string TokenId)
        {
            try
            {
                long currentUTCFormate=UTCHandling.GetUTCFromDateTime(DateTime.Now);
                var QueryStatement = @"select
                                       count(*)
                                       from master.accounttoken 
                                       where token_id=@token_id
                                       AND expire_in < @expire_in;";//DONE (testing pending)utc time with grether than datetime.now() with expireat
                var parameter = new DynamicParameters();
            
                parameter.Add("@token_id",new Guid(TokenId));
                parameter.Add("@expire_in",currentUTCFormate);
                int accounttoken = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                bool isValideToken=accounttoken>0;
                
                return isValideToken;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private AccountToken Map(dynamic record)
        {
            AccountToken entity = new AccountToken();
            entity.TokenId=Convert.ToString(record.token_id);
            entity.UserName=record.user_name;
            entity.AccessToken=record.access_token;
            entity.ExpireIn=record.expire_in;
            entity.RefreshToken=record.refresh_token;
            entity.RefreshExpireIn=record.refresh_expire_in;
            entity.AccountId=record.account_id;
            entity.TokenType=record.type;
            entity.Session_Id=record.session_id;
            entity.Scope=record.scope;
            entity.IdpType=record.idp_type;
            entity.CreatedAt=record.created_at; 
            return entity;
        }

    }
}
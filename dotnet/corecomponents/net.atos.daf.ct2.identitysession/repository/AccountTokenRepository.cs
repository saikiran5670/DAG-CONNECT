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
                                      ,@created_at)RETURNING Id";


            var parameter = new DynamicParameters();
            
            parameter.Add("@user_name", accountToken.User_Name);
            parameter.Add("@access_token", accountToken.AccessToken);
            parameter.Add("@expire_in", accountToken.ExpiresIn);
            parameter.Add("@refresh_token", accountToken.Refresh_Token);
            parameter.Add("@refresh_expire_in", accountToken.Refresh_Expire_In); 
            parameter.Add("@account_id", accountToken.Account_Id);
            parameter.Add("@type", accountToken.TokenType);
            parameter.Add("@session_id", accountToken.Session_Id);
            parameter.Add("@scope", accountToken.Scope);
            parameter.Add("@idp_type", accountToken.Idp_Type);
            parameter.Add("@created_at", accountToken.Created_At);
            //parameter.Add("@status", ((char)vehicle.Status).ToString() != null ? (char)vehicle.Status:'P');
            int Id = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            accountToken.Id = Id;
            return Id;
            
        }
        public async Task<int> DeleteToken(AccountToken accountToken)
        {
             var QueryStatement = @"DELETE FROM
                                    master.accounttoken 
                                    where id=@id";
            var parameter = new DynamicParameters();
            parameter.Add("@id", accountToken.Id);
            int Id= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return Id;
        }
         public async Task<IEnumerable<AccountToken>> GetTokenDetails(int AccountID)
        {
             var QueryStatement = @"select id
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
                                    where id=@AccountID";
            var parameter = new DynamicParameters();
         
            parameter.Add("@AccountID", AccountID);
            IEnumerable<AccountToken> accounttoken = await dataAccess.QueryAsync<AccountToken>(QueryStatement, parameter);
            return accounttoken;
        }
         public async Task<IEnumerable<AccountToken>> GetTokenDetails(string AccessToken)
        {
             var QueryStatement = @"select id
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
                                    where access_token=@AccessToken";
            var parameter = new DynamicParameters();
         
            parameter.Add("@AccessToken", AccessToken);
            IEnumerable<AccountToken> accounttoken = await dataAccess.QueryAsync<AccountToken>(QueryStatement, parameter);
            return accounttoken;
        }
         public async Task<bool> ValidateToken(AccountToken accountToken)
        {
             var QueryStatement = @"select id
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
                                    where access_token=@AccessToken";
            var parameter = new DynamicParameters();
         
            parameter.Add("@AccessToken", accountToken.AccessToken);
            bool accounttoken = await dataAccess.ExecuteScalarAsync<bool>(QueryStatement, parameter);
            
            return accounttoken;
        }

    }
}
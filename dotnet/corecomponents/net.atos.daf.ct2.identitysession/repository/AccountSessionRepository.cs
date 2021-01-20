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
    public class AccountSessionRepository : IAccountSessionRepository
    {
         private readonly IDataAccess dataAccess;
        public AccountSessionRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
         public async Task<int> InsertSession(AccountSession accountSession)
        {
            var QueryStatement = @"INSERT INTO  master.accountsession 
                                      (
                                       ip_address
                                      ,last_session_refresh
                                      ,session_started_at
                                      ,sessoin_expired_at
                                      ,account_id
                                      ,created_at
                                     ) 
                            	VALUES(
                                       @ip_address
                                      ,@last_session_refresh
                                      ,@session_started_at
                                      ,@sessoin_expired_at
                                      ,@account_id
                                      ,@created_at) RETURNING id";


            var parameter = new DynamicParameters();
            
            parameter.Add("@ip_address", accountSession.IpAddress);
            parameter.Add("@last_session_refresh", accountSession.LastSessionRefresh);
            parameter.Add("@session_started_at", accountSession.SessionStartedAt);
            parameter.Add("@sessoin_expired_at", accountSession.SessionExpiredAt);
            parameter.Add("@account_id", accountSession.AccountId); // snehal
            parameter.Add("@created_at", accountSession.CreatedAt);
            //parameter.Add("@status", ((char)vehicle.Status).ToString() != null ? (char)vehicle.Status:'P');
            int sessionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            accountSession.Id = sessionID;
            return sessionID;
            
        }
         public async Task<int> UpdateSession(AccountSession accountSession)
        {
             //int sessionID=0;
            //  if (accountSession.Id.ToString() == null || accountSession.Id == 0)
            // {
            //     var QueryStatement = @" UPDATE master.accountsession
            //                             SET 
            //                            ip_address=@ip_address
            //                           ,last_session_refresh= @last_session_refresh
            //                           ,session_started_at= @session_started_at
            //                           ,sessoin_expired_at = @sessoin_expired_at
            //                           ,account_id = @account_id
            //                           ,created_at = @created_at                                       
            //                             WHERE id = @Id
            //                              RETURNING Id;";

            //     var parameter = new DynamicParameters();
            //     parameter.Add("@Id", accountSession.Id);
            //     parameter.Add("@ip_address", accountSession.IpAddress);
            //     parameter.Add("@last_session_refresh", accountSession.LastSessionRefresh);
            //     parameter.Add("@session_started_at", accountSession.SessionStartedAt);
            //     parameter.Add("@sessoin_expired_at", accountSession.SessionExpiredAt);
            //     parameter.Add("@account_id", accountSession.AccountId); // snehal
            //     parameter.Add("@created_at", accountSession.CreatedAt);
                
            //      sessionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            // }
            // else
            // {
                var QueryStatement = @" UPDATE master.accountsession
                                        SET 
                                       ip_address=@ip_address
                                      ,last_session_refresh= @last_session_refresh
                                      ,session_started_at= @session_started_at
                                      ,sessoin_expired_at = @sessoin_expired_at
                                      ,account_id = @account_id
                                      ,created_at = @created_at                                       
                                        WHERE id = @Id
                                         RETURNING Id;";

                var parameter = new DynamicParameters();
                parameter.Add("@Id", accountSession.Id);
                parameter.Add("@ip_address", accountSession.IpAddress);
                parameter.Add("@last_session_refresh", accountSession.LastSessionRefresh);
                parameter.Add("@session_started_at", accountSession.SessionStartedAt);
                parameter.Add("@sessoin_expired_at", accountSession.SessionExpiredAt);
                parameter.Add("@account_id", accountSession.AccountId); 
                parameter.Add("@created_at", accountSession.CreatedAt);
                int  sessionID= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
           // }
            return sessionID;
        }
         public async Task<int> DeleteSession(AccountSession accountSession)
        {
            var QueryStatement = @"DELETE FROM
                                    master.accountsession 
                                    where id=@id";
            var parameter = new DynamicParameters();
            parameter.Add("@id", accountSession.Id);
            int Id= await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return Id;
        }
          public async Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId)
        {
            var QueryStatement = @"select id
                                    ,ip_address
                                      ,last_session_refresh
                                      ,session_started_at
                                      ,sessoin_expired_at
                                      ,account_id
                                      ,created_at
                                    from master.accountsession 
                                    where account_id=@AccountId";
            var parameter = new DynamicParameters();
         
            parameter.Add("@AccountId", AccountId);
            IEnumerable<AccountSession> accountsessions = await dataAccess.QueryAsync<AccountSession>(QueryStatement, parameter);
            return accountsessions;
           
            // List<AccountSession> accountSessions = new List<AccountSession>();
            // dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            // foreach (dynamic record in result)
            // {
            //     accountSessions.Add(Map(record));
            // }
            // return accountSessions.AsEnumerable();
        }
        // private AccountSession Map(dynamic record)
        // {
        //     AccountSession accountsession = new AccountSession();
        //     accountsession.Id = record.id;
        //     accountsession.IpAddress = record.ip_address;
        //     accountsession.LastSessionRefresh = record.last_session_refresh;
        //     accountsession.SessionStartedAt = record.session_started_at;
        //     accountsession.SessionExpiredAt = record.sessoin_expired_at;
        //     accountsession.AccountId = record.account_id;
        //     accountsession.CreatedAt = record.created_at;
        //     return accountsession;
        // }
       
    }
}
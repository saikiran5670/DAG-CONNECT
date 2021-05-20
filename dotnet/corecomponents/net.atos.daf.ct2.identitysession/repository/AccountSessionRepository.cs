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
            try
            {
                var QueryStatement = @"INSERT INTO  master.accountsession 
                                        (
                                         session_id
                                        ,ip_address
                                        ,last_session_refresh
                                        ,session_started_at
                                        ,sessoin_expired_at
                                        ,account_id
                                        ,created_at
                                        ) 
                                    VALUES(
                                         @session_id 
                                        ,@ip_address
                                        ,@last_session_refresh
                                        ,@session_started_at
                                        ,@sessoin_expired_at
                                        ,@account_id
                                        ,@created_at
                                        ) RETURNING id";


                var parameter = new DynamicParameters();
                //parameter.Add("@id", accountSession.Id);
                parameter.Add("@session_id", accountSession.Session_Id);
                parameter.Add("@ip_address", accountSession.IpAddress);
                parameter.Add("@last_session_refresh", accountSession.LastSessionRefresh);
                parameter.Add("@session_started_at", accountSession.SessionStartedAt);
                parameter.Add("@sessoin_expired_at", accountSession.SessionExpiredAt);
                parameter.Add("@account_id", accountSession.AccountId);
                parameter.Add("@created_at", accountSession.CreatedAt);
                int sessionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return sessionID;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> UpdateSession(AccountSession accountSession)
        {
            try
            {
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
                //parameter.Add("@session_id", accountSession.Session_Id);
                parameter.Add("@ip_address", accountSession.IpAddress);
                parameter.Add("@last_session_refresh", accountSession.LastSessionRefresh);
                parameter.Add("@session_started_at", accountSession.SessionStartedAt);
                parameter.Add("@sessoin_expired_at", accountSession.SessionExpiredAt);
                parameter.Add("@account_id", accountSession.AccountId);
                parameter.Add("@created_at", accountSession.CreatedAt);
                int sessionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return sessionID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteSession(string SessionId)
        {
            try
            {
                long currentUTCFormate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                var QueryStatement = @"DELETE FROM
                                        master.accountsession 
                                        where id=@id
                                        AND sessoin_expired_at < @sessoin_expired_at
                                        RETURNING Id;";//DONE (testing pending) expired at check for delete sesion
                var parameter = new DynamicParameters();
                parameter.Add("@id", Guid.Parse(SessionId));
                parameter.Add("@sessoin_expired_at", currentUTCFormate);
                int Id = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return Id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountSession>> GetAccountSession(int AccountId)
        {
            try
            {
                var QueryStatement = @"select 
                                         id
                                        ,session_id
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
                dynamic accountsessions = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<AccountSession> accountsessionsList = new List<AccountSession>();
                foreach (dynamic record in accountsessions)
                {
                    accountsessionsList.Add(Map(record));
                }
                return accountsessionsList.AsEnumerable();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> DeleteSessionByAccountId(int AccountId)
        {
            try
            {
                var QueryStatement = @"DELETE FROM
                                        master.accountsession 
                                        where account_id=@account_id
                                        RETURNING Id;";
                var parameter = new DynamicParameters();
                parameter.Add("@id", AccountId);
                int Id = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return Id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private AccountSession Map(dynamic record)
        {
            AccountSession entity = new AccountSession();
            entity.Id = record.id;
            entity.Session_Id = record.session_id;
            entity.IpAddress = record.ip_address;
            entity.LastSessionRefresh = record.last_session_refresh;
            entity.SessionStartedAt = record.session_started_at;
            entity.SessionExpiredAt = record.sessoin_expired_at;
            entity.AccountId = record.account_id;
            entity.CreatedAt = record.created_at;
            return entity;
        }
        public async Task<AccountSession> GetAccountSessionById(int SessionId)
        {
            try
            {
                var QueryStatement = @"select 
                                         id
                                        ,session_id
                                        ,ip_address
                                        ,last_session_refresh
                                        ,session_started_at
                                        ,sessoin_expired_at
                                        ,account_id
                                        ,created_at
                                        from master.accountsession 
                                        where id=@SessionId";
                var parameter = new DynamicParameters();

                parameter.Add("@SessionId", SessionId);
                dynamic accountsessions = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<AccountSession> accountsessionsList = new List<AccountSession>();
                foreach (dynamic record in accountsessions)
                {
                    accountsessionsList.Add(Map(record));
                }
                return accountsessionsList.AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
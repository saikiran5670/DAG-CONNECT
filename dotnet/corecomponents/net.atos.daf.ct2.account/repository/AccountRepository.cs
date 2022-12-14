using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.account.ENUM;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.identitysession.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataAccess _dataAccess;
        public AccountRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        #region Account
        public async Task<Account> Create(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@email", String.IsNullOrEmpty(account.EmailId) ? account.EmailId : account.EmailId.ToLower());
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                parameter.Add("@type", (char)account.AccountType);
                parameter.Add("@driver_id", account.DriverId);
                parameter.Add("@created_at", account.CreatedAt.Value);
                parameter.Add("@organization_Id", account.Organization_Id);
                parameter.Add("@owner_email", account.OwnerEmail?.ToLower());

                // For System account, organization preference will be the default preference.
                string query = account.AccountType == AccountType.PortalAccount
                    ? @"insert into master.account(email,salutation,first_name,last_name,type,driver_id,state,preference_id,blob_id,created_at) 
                      values(@email,@salutation,@first_name,@last_name,@type,@driver_id,'A',null,null,@created_at) RETURNING id"

                    : @"insert into master.account(email,salutation,first_name,last_name,type,driver_id,state,preference_id,blob_id,created_at,owner_email) 
                      values(@email,@salutation,@first_name,@last_name,@type,@driver_id,'A', 
                      (select preference_id from master.organization org where org.id=@organization_Id),null,@created_at,@owner_email) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
                    parameter.Add("@state", "A");
                    parameter.Add("@account_id", account.Id);
                    query = @"insert into master.accountorg(account_id,organization_id,start_date,end_date,state)  
                                   values(@account_id,@organization_Id,@start_date,@end_date,@state) RETURNING id";
                    await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return account;
        }

        public async Task<Account> Update(Account account)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", account.Id);
                // parameter.Add("@email", String.IsNullOrEmpty(account.EmailId) ? account.EmailId : account.EmailId.ToLower());
                parameter.Add("@salutation", account.Salutation);
                parameter.Add("@first_name", account.FirstName);
                parameter.Add("@last_name", account.LastName);
                parameter.Add("@type", (char)account.AccountType);
                parameter.Add("@driver_id", account.DriverId == "" ? null : account.DriverId);
                string query = @"update master.account set salutation = @salutation,
                                first_name = @first_name,last_name = @last_name ,driver_id=@driver_id, type = @type
                                where id = @id RETURNING id";
                account.Id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
            return account;
        }
        public async Task<bool> Delete(int accountid, int organization_id)
        {
            TransactionScope transactionScope = null;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", accountid);
                parameter.Add("@organization_id", organization_id);
                string query = string.Empty;
                int result = 0;

                using (transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // check in user need to delete 
                    // Delete Account Group Reference
                    query = @"delete from master.groupref gr
                         using master.group g,master.accountorg ao 
                         where gr.ref_id = @id and ao.organization_id = @organization_id 
                         and g.id=gr.group_id and ao.state='A'";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    // Delete account role
                    query = @"delete from master.accountrole where account_id = @id and organization_id = @organization_id;";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    // disable account with organization
                    query = @"update master.accountorg set state='D' where account_id = @id and organization_id = @organization_id";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    transactionScope.Complete();
                }
                // check if account associated with multiple organization
                query = @"select count(1) from master.accountorg where state='A' and account_id = @id";
                result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (result <= 0)
                {
                    // disable preference
                    query = @"update master.accountpreference set state='D' from master.account where master.accountpreference.id=master.account.preference_id and master.account.id=@id;";
                    //query += @"delete from master.accountblob ab using master.account a where a.id = @id and a.blob_id = ab.id and a.state = 'A';";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    // disable account 
                    query = @"update master.account set state='D' where id = @id;";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                return true;
            }

            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (transactionScope != null)
                    transactionScope.Dispose();
            }
        }

        public async Task<Account> Duplicate(AccountFilter filter)
        {
            var parameter = new DynamicParameters();
            Account account = null;
            try
            {
                string query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.driver_id,a.type as accounttype,ag.organization_id as 
                Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.state='A' 
                and ag.state='A' where 1=1 ";

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
                dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                foreach (dynamic record in result)
                {

                    account = Map(record);
                    account.IsDuplicateInOrg = true;
                }
                // account is part of other organization
                if (account == null)
                {
                    query = @"select a.id,a.email,a.salutation,a.first_name,a.last_name,a.driver_id,a.type as accounttype,ag.organization_id as 
                    Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.state='A' 
                    and ag.state='A' where 1=1 ";

                    // email id filter
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        parameter.Add("@email", filter.Email.ToLower());
                        query = query + " and LOWER(a.email) = @email ";
                    }
                    query = query + "limit 1";
                    result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                    foreach (dynamic record in result)
                    {

                        account = Map(record);
                        account.IsDuplicate = true;
                    }
                }
                return account;
            }
            catch (Exception)
            {
                throw;
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
                Organization_Id,a.preference_id,a.blob_id,a.created_at from master.account a join master.accountorg ag on a.id = ag.account_id and a.state='A' 
                and ag.state='A' where 1=1 ";

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
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);

                    foreach (dynamic record in result)
                    {
                        accounts.Add(Map(record));
                    }
                }
                return accounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Account> GetAccountByEmailId(string emailId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@email", emailId.ToLower());
                var query = @"select id, email,owner_email,type, salutation, first_name, last_name, driver_id from master.account where lower(email) = @email and state='A'";

                dynamic result = await _dataAccess.QueryFirstOrDefaultAsync<dynamic>(query, parameter);

                return MapAccount(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Account> GetAccountByAccountId(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@accountId", accountId);
                var query = @"select id, email,owner_email,type, salutation, first_name, last_name from master.account where id = @accountId and state='A'";

                dynamic result = await _dataAccess.QuerySingleAsync<dynamic>(query, parameter);

                return MapAccount(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetCount(int organization_id)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                int count = 0;
                query = @"select count(1) from master.account a join master.accountorg ag on a.id = ag.account_id and a.state='A' 
                and ag.state='A' where ag.organization_id=@organization_id";
                parameter.Add("@organization_id", organization_id);
                count = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Account> AddAccountToOrg(Account account)
        {
            try
            {
                if (!await CheckIfAccountExistsInOrg(account.Id, account.Organization_Id.Value))
                {
                    var parameter = new DynamicParameters();
                    string query = string.Empty;
                    parameter.Add("@account_id", account.Id);
                    parameter.Add("@organization_Id", account.Organization_Id.Value);
                    parameter.Add("@start_date", account.StartDate);
                    if (account.EndDate.HasValue)
                    {
                        parameter.Add("@end_date", account.EndDate);
                    }
                    else
                    {
                        parameter.Add("@end_date", null);
                    }
                    query = @"insert into master.accountorg(account_id,organization_id,start_date,end_date,state)  
                                   values(@account_id,@organization_Id,@start_date,@end_date,'A') RETURNING id";
                    account.Id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                return account;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckIfAccountExistsInOrg(int accountId, int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_Id", orgId);

                query = @"SELECT EXISTS 
                            ( SELECT 1 FROM master.accountorg WHERE account_id=@account_id AND organization_Id=@organization_Id AND state='A')";
                return await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check if System Account has organization already linked to disallow linkage with another organization
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfSystemAccAlreadyHasOrgLinked(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);

                string query = @"SELECT COUNT(1) 
                                FROM master.account acc
                                INNER JOIN master.accountOrg ao ON acc.id=ao.account_id 
                                WHERE acc.id=@account_id AND acc.type='S'";

                var linkedOrgCount = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                return linkedOrgCount >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpsertPasswordModifiedDate(int accountId, long modifiedAt)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", accountId);
                parameter.Add("@modified_at", modifiedAt);

                string query =
                    @"INSERT INTO master.passwordpolicy (account_id, modified_at)
                        VALUES(@account_id, @modified_at) 
                        ON CONFLICT (account_id) 
                        DO 
                        UPDATE SET modified_at = @modified_at,is_reminder_sent = false
                        RETURNING id";

                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateIsReminderSent(int accountId, bool isReminderSent = true)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", accountId);
                parameter.Add("@is_reminder_sent", isReminderSent);

                string query = "UPDATE master.passwordpolicy SET is_reminder_sent = @is_reminder_sent where account_id = @account_id RETURNING id";
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpsertPasswordPolicyAccount(PasswordPolicyAccount passwordPolicyForAccount)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", passwordPolicyForAccount.AccountId);
                parameter.Add("@failed_login_attempts", passwordPolicyForAccount.FailedLoginAttempts);
                parameter.Add("@locked_until", passwordPolicyForAccount.LockedUntil);
                parameter.Add("@account_lock_attempts", passwordPolicyForAccount.AccountLockAttempts);
                parameter.Add("@is_blocked", passwordPolicyForAccount.IsBlocked);
                parameter.Add("@last_login", passwordPolicyForAccount.LastLogin);

                string query =
                    @"INSERT INTO master.passwordpolicy (account_id, failed_login_attempts,locked_until,account_lock_attempts,is_blocked,last_login)
                        VALUES(@account_id, @failed_login_attempts,@locked_until,@account_lock_attempts,@is_blocked,@last_login) 
                        ON CONFLICT (account_id) 
                        DO 
                        UPDATE SET failed_login_attempts = @failed_login_attempts,
                            locked_until = @locked_until, account_lock_attempts = @account_lock_attempts,
                            is_blocked = @is_blocked, last_login=@last_login
                        RETURNING id";

                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<long?> GetPasswordModifiedDate(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", accountId);

                string query =
                    @"SELECT modified_at from master.passwordpolicy where account_id = @account_id";

                var record = await _dataAccess.ExecuteScalarAsync<long?>(query, parameter);
                return record;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PasswordPolicyAccount> GetPasswordPolicyAccount(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);

                string query =
                    @"SELECT account_id as AccountId,failed_login_attempts as FailedLoginAttempts,locked_until as LockedUntil,account_lock_attempts as AccountLockAttempts,is_blocked as IsBlocked,last_login as LastLogin,is_reminder_sent as IsReminderSent from master.passwordpolicy where account_id = @account_id";

                return await _dataAccess.QueryFirstOrDefaultAsync<PasswordPolicyAccount>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@emailId", emailId.ToLower());

                string accountQuery =
                    @"SELECT preference_id from master.account where lower(email) = @emailId";

                var accountPreferenceId = await _dataAccess.QueryFirstAsync<int?>(accountQuery, parameter);

                if (!accountPreferenceId.HasValue)
                {
                    string orgQuery = string.Empty;
                    int? orgPreferenceId = null;
                    if (orgId.HasValue && orgId > 0)
                    {
                        var orgParameter = new DynamicParameters();
                        orgParameter.Add("@orgId", orgId);

                        orgQuery = @"SELECT preference_id from master.organization WHERE id=@orgId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, orgParameter);
                    }
                    else
                    {
                        orgQuery =
                            @"SELECT o.preference_id from master.account acc
                            INNER JOIN master.accountOrg ao ON acc.id=ao.account_id
                            INNER JOIN master.organization o ON ao.organization_id=o.id
                            where lower(acc.email) = @emailId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, parameter);
                    }

                    if (!orgPreferenceId.HasValue)
                        return "EN-GB";
                    else
                        return await GetCodeByPreferenceId(orgPreferenceId.Value);
                }
                return await GetCodeByPreferenceId(accountPreferenceId.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetCodeByPreferenceId(int preferenceId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@preferenceId", preferenceId);

                string query =
                    @"SELECT l.code from master.accountpreference ap
                    INNER JOIN translation.language l ON ap.id = @preferenceId AND ap.language_id=l.id";

                var languageCode = await _dataAccess.QueryFirstAsync<string>(query, parameter);

                return languageCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int?> GetAccountPreferenceId(string emailId, int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@emailId", emailId.ToLower());

                string accountQuery =
                    @"SELECT preference_id from master.account where lower(email) = @emailId";

                var accountPreferenceId = await _dataAccess.QueryFirstAsync<int?>(accountQuery, parameter);

                if (!accountPreferenceId.HasValue)
                {
                    string orgQuery = string.Empty;
                    int? orgPreferenceId = null;
                    if (orgId > 0)
                    {
                        var orgParameter = new DynamicParameters();
                        orgParameter.Add("@orgId", orgId);

                        orgQuery = @"SELECT preference_id from master.organization WHERE id=@orgId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, orgParameter);
                    }
                    else
                    {
                        orgQuery =
                            @"SELECT o.preference_id from master.account acc
                            INNER JOIN master.accountOrg ao ON acc.id=ao.account_id
                            INNER JOIN master.organization o ON ao.organization_id=o.id
                            where lower(acc.email) = @emailId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, parameter);
                    }

                    return orgPreferenceId;
                }
                return accountPreferenceId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<AccountPreferenceResponse> GetAccountPreferencesById(int preferenceId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@preferenceId", preferenceId);

                string query =
                    @"SELECT 
                        l.code as Language,
	                    df.name as DateFormat,
	                    tz.name as TimeZone,
	                    tf.code as TimeFormat,
	                    u.code as UnitDisplay,
	                    vd.code as VehicleDisplay
                    FROM master.accountpreference ap
                    INNER JOIN translation.language l ON ap.id = @preferenceId AND ap.language_id=l.id
                    INNER JOIN master.dateformat df ON ap.id = @preferenceId AND ap.date_format_id=df.id
                    INNER JOIN master.timezone tz ON ap.id = @preferenceId AND ap.timezone_id=tz.id
                    INNER JOIN master.timeformat tf ON ap.id = @preferenceId AND ap.time_format_id=tf.id
                    INNER JOIN master.unit u ON ap.id = @preferenceId AND ap.unit_id=u.id
                    INNER JOIN master.vehicledisplay vd ON ap.id = @preferenceId AND ap.vehicle_display_id=vd.id";

                return await _dataAccess.QueryFirstOrDefaultAsync<AccountPreferenceResponse>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAccountOfPasswordExpiry(int noOfDays)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@noOfDays", noOfDays);

                var query = @"Select acc.id as Id, acc.email as email, acc.owner_email as owner_email,acc.type, acc.salutation as salutation, acc.first_name as first_name, last_name as LastName from master.account acc inner join master.passwordpolicy pp on acc.id = pp.account_id where pp.is_blocked = false and pp.is_reminder_sent = false and acc.State= 'A' and EXTRACT(day FROM(now() - TO_TIMESTAMP(modified_at / 1000))) >= @noOfDays";

                var result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                List<Account> accounts = new List<Account>();
                foreach (var item in result)
                {
                    accounts.Add(MapAccount(item));
                }
                return accounts;
            }
            catch (Exception)
            {
                throw;
            }
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
                    var blobId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                else // update profile picture
                {
                    query = @"insert into master.accountblob(image_type,image) values(@image_type,@image) RETURNING id";
                    var blobId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    accountBlob.Id = blobId;
                    if (blobId > 0)
                    {
                        parameter.Add("@blob_id", blobId);
                        query = "update master.account set blob_id=@blob_id where id=@account_id";
                        await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
                dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
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
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Account Access Relationship

        public async Task<AccessRelationship> CreateAccessRelationship(AccessRelationship entity)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                parameter.Add("@access_type", (char)entity.AccessRelationType);
                parameter.Add("@account_group_id", entity.AccountGroupId);
                parameter.Add("@vehicle_group_id", entity.VehicleGroupId);

                //TODO:check for duplicate access relationship, if not required and is not the case from UI will remove this check.
                query = @"select id from master.accessrelationship where access_type=@access_type and account_group_id=@account_group_id and vehicle_group_id=@vehicle_group_id";
                var accessId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (accessId > 0)
                {
                    entity.Exists = true;
                    entity.Id = accessId;
                    return entity;
                }

                query = @"insert into master.accessrelationship(access_type,account_group_id,vehicle_group_id) " +
                              "values(@access_type,@account_group_id,@vehicle_group_id) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                entity.Id = id;
            }
            catch (Exception)
            {
                throw;
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
                    id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
            catch (Exception)
            {
                throw;
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
                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
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
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                    //Account account;
                    foreach (dynamic record in result)
                    {
                        entity.Add(MapAccessRelationship(record));
                    }
                }
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Account/Vehicle Access Relationship

        public async Task<List<AccountVehicleAccessRelationship>> GetAccountVehicleAccessRelationship(AccountVehicleAccessRelationshipFilter filter, bool is_vehicleGroup)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<AccountAccessRelationshipEntity> entity = new List<AccountAccessRelationshipEntity>();
                string query = string.Empty;
                List<AccountVehicleAccessRelationship> response = new List<AccountVehicleAccessRelationship>();
                if (filter != null)
                {
                    // org filter
                    if (filter.OrganizationId > 0 && is_vehicleGroup)
                    {
                        // vehicles and vehicle groups
                        query = @"SELECT id,COALESCE(name,'') as name,access_type,count,true as is_group,group_id,COALESCE(group_name,'') as group_name,is_ag_vg_group 
                                FROM (
                                    SELECT vg.id,vg.name,ar.access_type,
                                     CASE WHEN (vg.group_type ='D' and vg.function_enum='A') 
						                  THEN (
                                                -- Fetch Owned+Visible vehicles from vehicle group type 'D'
                                                -- Fetch Owned vehicles from vehicle group type 'D'
                                                SELECT COUNT(id)
                                                FROM
                                                (
                                                    SELECT veh.id
									                FROM master.vehicle veh 
									                INNER JOIN master.orgrelationshipmapping org on veh.id=org.vehicle_id 
									                INNER JOIN master.orgrelationship ors on ors.id=org.relationship_id
									                            AND (org.owner_org_id=@organization_id AND lower(ors.code)='owner')
									                            AND ors.state='A'
									                            AND CASE WHEN COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date ELSE COALESCE(end_date,0)=0 END
                                                    UNION
                                                    SELECT v.id
                                                        FROM master.vehicle v
                                                        INNER JOIN master.groupref gref ON v.id=gref.ref_id
                                                        INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V'
                                                        INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.target_org_id=@organization_id
                                                        INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                        WHERE 
	                                                        case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                        else COALESCE(end_date,0) = 0 end

                                                        UNION
                                                        -- Visible vehicles of type D, method O
                                                        SELECT v.id
                                                        FROM master.group grp
                                                        INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.owner_org_id=grp.organization_id and orm.target_org_id=@organization_id and grp.group_type='D' AND grp.object_type='V'
                                                        INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                        INNER JOIN master.vehicle v on v.organization_id = grp.organization_id
                                                        WHERE 
	                                                        case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                        else COALESCE(end_date,0) = 0 end
                                                 ) owned_visible_vehicles
                                                )
				                          WHEN (vg.group_type ='D' and vg.function_enum='V')
                                          THEN (
                                                -- Fetch Visible vehicles from vehicle group type 'D'
						                        -- Visible vehicles of type G
                                                SELECT COUNT(id)
                                                FROM
                                                (
                                                    SELECT v.id
                                                    FROM master.vehicle v
                                                    INNER JOIN master.groupref gref ON v.id=gref.ref_id
                                                    INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V'
                                                    INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.target_org_id=@organization_id
                                                    INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                    WHERE 
	                                                    case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                    else COALESCE(end_date,0) = 0 end

                                                    UNION
                                                    -- Visible vehicles of type D, method O
                                                    SELECT v.id
                                                    FROM master.group grp
                                                    INNER JOIN master.orgrelationshipmapping as orm on grp.id = orm.vehicle_group_id and orm.owner_org_id=grp.organization_id and orm.target_org_id=@organization_id and grp.group_type='D' AND grp.object_type='V'
                                                    INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                    INNER JOIN master.vehicle v on v.organization_id = grp.organization_id
                                                    WHERE 
	                                                    case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                    else COALESCE(end_date,0) = 0 end
                                                ) visible_vehicles
                                               )
				                          WHEN (vg.group_type ='D' and vg.function_enum='O') 
						                  THEN (
                                                -- Fetch Owned vehicles from vehicle group type 'D'
                                                SELECT count(distinct veh.id) 
									            FROM master.vehicle veh 
									            INNER JOIN master.orgrelationshipmapping org on veh.id=org.vehicle_id 
									            INNER JOIN master.orgrelationship ors on ors.id=org.relationship_id
									                        AND ((org.owner_org_id=@organization_id AND lower(ors.code)='owner') or veh.organization_id=@organization_id)
									                        AND ors.state='A'
									                        AND CASE WHEN COALESCE(end_date,0) !=0 THEN to_timestamp(COALESCE(end_date)/1000)::date>now()::date ELSE COALESCE(end_date,0)=0 END
                                               )
							              ELSE (
                                                -- Fetch Owned vehicles from vehicle group type 'G'
                                                SELECT count(gr.group_id) 
                                                FROM master.groupref gr 
                                                WHERE gr.group_id=vg.id --or gr.group_id=om.vehicle_group_id
                                                      --and om.owner_org_id=@organization_id and lower(os.code)='owner'
                                               )
                                          END as count,
                                        CASE WHEN (a.id is NULL) THEN ag.id ELSE a.id END as group_id,
                                        CASE WHEN (a.id is NULL) THEN ag.name ELSE a.salutation || ' ' || a.first_name || ' ' || a.last_name  end as group_name,
                                        CASE WHEN (a.id is NULL) THEN true ELSE false END as is_ag_vg_group
                                    FROM master.group vg 
                                    INNER JOIN master.accessrelationship ar on ar.vehicle_group_id=vg.id and vg.object_type='V' and vg.group_type in('G','D')
                                    INNER JOIN master.group ag on ag.id = ar.account_group_id and ag.organization_id=@organization_id and ag.object_type='A'                             
                                    LEFT OUTER JOIN master.account a on a.id = ag.ref_id 
							        --LEFT JOIN master.orgrelationshipmapping as om on vg.id = om.vehicle_group_id
							        --LEFT JOIN master.orgrelationship as os on om.relationship_id=os.id and os.state='A'
                                    WHERE vg.organization_id=@organization_id 
                                          --and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                      --else COALESCE(end_date,0) = 0 end
                                    ORDER BY vg.id desc 
                                ) vehiclegroup

                            UNION ALL

                            SELECT id,COALESCE(name,'') as name,access_type,count,false as is_group,group_id,COALESCE(group_name,'') as group_name,is_ag_vg_group 
                            FROM (
                                    SELECT v.id,v.name,ar.access_type,0 as count,
                                        CASE WHEN (a.id is NULL) then ag.id else a.id end as group_id,
                                        CASE WHEN (a.id is NULL) then ag.name else a.salutation || ' ' || a.first_name || ' ' || a.last_name  end as group_name,
                                        CASE WHEN (a.id is NULL) then true else false end as is_ag_vg_group
                                    FROM master.group vg 
                                    INNER JOIN master.vehicle v on v.id=vg.ref_id and vg.organization_id=@organization_id and vg.group_type='S' and vg.object_type='V'
                                    INNER JOIN master.accessrelationship ar on ar.vehicle_group_id=vg.id 
                                    INNER JOIN master.group ag on ag.id = ar.account_group_id and ag.organization_id=@organization_id and ag.object_type='A'
                                    LEFT OUTER JOIN master.account a on a.id = ag.ref_id where vg.ref_id > 0
                                    ORDER BY v.id desc
                                 ) vehicles";
                    }
                    else
                    {
                        // account and account groups
                        query = @"
                            SELECT id,COALESCE(name,'') as name,access_type,count,true as is_group,group_id,COALESCE(group_name,'') as group_name,is_ag_vg_group
                            FROM (
                                 SELECT ag.id,ag.name,ar.access_type, 
                                     case when (ag.group_type ='D') then 
                                     (SELECT count(acc.id) 
                                        FROM master.account acc 
                                        INNER JOIN master.accountorg ao on acc.id=ao.account_id and ao.organization_id=@organization_id
                                        WHERE acc.state='A' and ao.state='A')
                                     else (select count(gr.group_id) from master.groupref gr where gr.group_id=ag.id ) end as count,
                                     case when (v.id is NULL) then vg.id else v.id end as group_id,
                                     case when (v.id is NULL) then vg.name else v.name end as group_name,
                                     case when (v.id is NULL) then true else false end as is_ag_vg_group                         
                                 FROM master.group ag 
                                 INNER JOIN master.accessrelationship ar on ar.account_group_id=ag.id and ag.organization_id=@organization_id and ag.object_type='A' and ag.group_type in('G','D')
                                 INNER JOIN master.group vg on vg.id = ar.vehicle_group_id and vg.organization_id=@organization_id and vg.object_type='V' 
                                 LEFT OUTER JOIN master.vehicle v on v.id = vg.ref_id 
                                 WHERE vg.organization_id=@organization_id 
                                 ORDER BY ag.id desc
                                ) accountgroup

                         -- accounts
                         UNION ALL

                         SELECT id,COALESCE(name,'') as name,access_type,count,false as is_group,group_id,COALESCE(group_name,'') as group_name,is_ag_vg_group
                         FROM (
                                 SELECT a.id,a.salutation || ' ' || a.first_name || ' ' || a.last_name as name, ar.access_type,0 as count,
                                     case when (v.id is NULL) then vg.id else v.id end as group_id,
                                     case when (v.id is NULL) then vg.name else v.name end as group_name,
                                     case when (v.id is NULL) then true else false end as is_ag_vg_group
                                 FROM master.group ag 
                                 INNER JOIN master.account a on a.id=ag.ref_id and ag.organization_id=@organization_id and ag.object_type='A'
                                 INNER JOIN master.accessrelationship ar on ar.account_group_id=ag.id 
                                 INNER JOIN master.group vg on vg.id = ar.vehicle_group_id and vg.organization_id=@organization_id 
                                 LEFT OUTER JOIN MASTER.vehicle v on v.id = vg.ref_id where ag.ref_id > 0
                                 ORDER BY a.id desc
                              ) accounts";
                    }
                    parameter.Add("@organization_id", filter.OrganizationId);
                    IEnumerable<AccountAccessRelationshipEntity> accessRelationship = await _dataAccess.QueryAsync<AccountAccessRelationshipEntity>(query, parameter);
                    var groups = from stu in accessRelationship group stu by stu.Id into egroup orderby egroup.Key descending select egroup;
                    //Account account;
                    int firstRecord = 0;
                    foreach (var group in groups)
                    {
                        firstRecord = 0;
                        AccountVehicleAccessRelationship relationship = null;
                        foreach (var account in group)
                        {
                            if (firstRecord == 0)
                            {
                                // map both
                                relationship = MapVehicleAccessRelationship(account);
                            }
                            else
                            {
                                // add child only 
                                relationship.RelationshipData.Add(MapAccessRelationshipData(account));
                            }
                            firstRecord++;
                        }
                        response.Add(relationship);
                    }
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<AccountVehicleEntity>> GetVehicle(AccountVehicleAccessRelationshipFilter filter, bool is_vehicle)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                List<AccountVehicleEntity> response = new List<AccountVehicleEntity>();
                if (filter != null)
                {
                    // org filter
                    if (filter.OrganizationId > 0 && is_vehicle)
                    {
                        query = @"select id,name,count,true as is_group from (
                                select vg.id,vg.name,
                                case when (vg.group_type ='D') then 
	                                (select count(v.id) 
                                        from master.vehicle v where v.organization_id=@organization_id)
	                                else (select count(gr.group_id) from master.groupref gr where gr.group_id=vg.id ) end as count
                                from master.group vg 
                                where length(vg.name) > 0 and vg.organization_id=@organization_id and vg.object_type='V' and vg.group_type in ('G','D')
                                ) vehicleGroup
                                union all
                                select id,name,count,false as is_group from (
                                select v.id,v.name, 0 as count
                                from master.vehicle v 
                                where v.organization_id=@organization_id and length(v.name) > 0
                                ) vehicles";
                    }
                    else
                    {
                        query = @"select id,name,0 as count,true as is_group from (
                                select vg.id,vg.name
                                from master.group vg 
                                where vg.organization_id=@organization_id and vg.object_type='V' and vg.group_type in ('G','D') 
                                and length(vg.name) > 0
                                ) vehicleGroup
                                union all
                                select id,name,count,false as is_group from (
                                select v.id,v.name, 0 as count
                                from master.vehicle v 
                                where v.organization_id=@organization_id and length(v.name) > 0
                                ) vehicles";
                    }
                    parameter.Add("@organization_id", filter.OrganizationId);
                    IEnumerable<AccountVehicleEntity> accessRelationship = await _dataAccess.QueryAsync<AccountVehicleEntity>(query, parameter);
                    response = accessRelationship.ToList();
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<AccountVehicleEntity>> GetAccount(AccountVehicleAccessRelationshipFilter filter, bool is_account)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                List<AccountVehicleEntity> response = new List<AccountVehicleEntity>();
                if (filter != null)
                {
                    // org filter
                    if (filter.OrganizationId > 0 && is_account)
                    {
                        query = @"-- account group
                                SELECT id,name,count,true as is_group 
                                FROM (
                                    SELECT ag.id,ag.name,
                                    CASE WHEN (ag.group_type ='D') 
                                         THEN (SELECT count(acc.id) 
                                                FROM master.account acc 
                                                INNER JOIN master.accountorg ao on acc.id=ao.account_id and ao.organization_id=@organization_id
                                                WHERE acc.state='A' and ao.state='A')
	                                     ELSE (SELECT count(gr.group_id) FROM master.groupref gr WHERE gr.group_id=ag.id ) END as count
                                    FROM master.group ag 
                                    WHERE ag.object_type='A' and ag.group_type in ('G','D') and ag.organization_id=@organization_id 
                                        and length(ag.name) > 0
                                ) accountGroup
                                UNION ALL
                                SELECT id,name,count,false as is_group 
                                FROM (
                                    SELECT a.id,a.salutation || ' ' || a.first_name || ' ' || a.last_name  as name,0 as count
                                    FROM master.account a INNER JOIN master.accountorg ar on ar.account_id=a.id 
                                    WHERE ar.organization_id=@organization_id and length(a.first_name) > 0 and a.state='A' and ar.state='A'
                                ) accounts";
                    }
                    else
                    {
                        query = @"-- account group
                                SELECT id,name,0 as count,true as is_group 
                                FROM (
                                    SELECT ag.id,ag.name
                                    FROM master.group ag 
                                    WHERE ag.object_type='A' and ag.group_type in ('G','D') and ag.organization_id=@organization_id 
                                        and length(ag.name) > 0
                                ) accountGroup
                                UNION ALL
                                SELECT id,name,count,false as is_group 
                                FROM (
                                    SELECT a.id,a.salutation || ' ' || a.first_name || ' ' || a.last_name  as name,0 as count
                                    FROM master.account a INNER JOIN master.accountorg ar on ar.account_id=a.id 
                                    WHERE ar.organization_id=@organization_id and length(a.first_name) > 0 and a.state='A' and ar.state='A'
                                ) accounts";
                    }
                    parameter.Add("@organization_id", filter.OrganizationId);
                    IEnumerable<AccountVehicleEntity> accessRelationship = await _dataAccess.QueryAsync<AccountVehicleEntity>(query, parameter);
                    response = accessRelationship.ToList();
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteVehicleAccessRelationship(int organizationId, int groupId, bool isVehicle)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;

                //TODO: duplicate access relationship check and should not insert
                if (organizationId > 0 && groupId > 0 && isVehicle)
                {
                    // delete vehicle group access relation                    
                    query = "delete from master.accessrelationship where vehicle_group_id=@id";
                }
                else
                {
                    // delete vehicle group relationship
                    query = "delete from master.accessrelationship where account_group_id=@id";
                }
                parameter.Add("@id", groupId);
                var result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                throw;
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
                            await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        }
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
                    await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    result = true;
                }
            }
            catch (Exception)
            {
                throw;
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
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
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
            catch (Exception)
            {
                throw;
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
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                    foreach (dynamic record in result)
                    {
                        Roles.Add(new KeyValue() { Id = record.id, Name = record.name });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Roles;
        }

        public async Task<bool> CheckForFeatureAccessByEmailId(string emailId, string featureName)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@email", emailId.ToLower());
                parameter.Add("@feature", featureName);

                var query =
                    @"SELECT EXISTS 
                (
                    SELECT 1 FROM master.account acc
                    INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND lower(acc.email) = @email AND acc.state = 'A'
                    INNER JOIN master.Role r ON r.id = ar.role_id AND r.state = 'A'
                    INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.name = @feature
                )";
                return await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
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
                    query = @"select o.id, coalesce(o.name, '(' || o.org_id || ')') as name from master.organization o inner join master.accountorg ao on o.id=ao.organization_id and ao.state='A' where ao.account_id=@account_id";
                    IEnumerable<KeyValue> result = await _dataAccess.QueryAsync<KeyValue>(query, parameter);
                    keyValueList = result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return keyValueList;
        }

        private async Task<IEnumerable<OrganizationKeyValue>> GetAccountOrgs(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                var query = @"select o.org_id as OrgCode, o.name as Name from master.organization o inner join master.accountorg ao on o.id=ao.organization_id and ao.state='A' where ao.account_id=@account_id";

                return await _dataAccess.QueryAsync<OrganizationKeyValue>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
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
                    query = @"select r.id,r.name,r.id,ac.organization_id as Organization_Id from master.role r inner join master.accountrole ac on r.id=ac.role_id and r.state='A' where ac.account_id=@account_id";
                    IEnumerable<AccountOrgRole> result = await _dataAccess.QueryAsync<AccountOrgRole>(query, parameter);
                    AccountOrgRoleList = result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return AccountOrgRoleList;
        }
        // End - Account Rendering
        #endregion

        #region ResetPasswordToken

        public async Task<ResetPasswordToken> Create(ResetPasswordToken resetPasswordToken)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", resetPasswordToken.AccountId);
                parameter.Add("@token_secret", resetPasswordToken.ProcessToken);
                parameter.Add("@status", resetPasswordToken.Status.ToString());
                parameter.Add("@expiry_at", resetPasswordToken.ExpiryAt.Value);
                parameter.Add("@created_at", resetPasswordToken.CreatedAt.Value);

                string query = @"insert into master.resetpasswordtoken(account_id,token_secret,status,expiry_at,created_at) " +
                              "values(@account_id,@token_secret,@status,@expiry_at,@created_at) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                resetPasswordToken.Id = id;
            }
            catch (Exception)
            {
                throw;
            }
            return resetPasswordToken;
        }

        public async Task<int> Update(int id, ResetTokenStatus status)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@id", id);
                parameter.Add("@status", status.ToString());

                string query = @"update master.resetpasswordtoken set status=@status
                                where id=@id RETURNING id";

                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResetPasswordToken> GetIssuedResetToken(Guid tokenSecret)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@token_secret", tokenSecret);

                string query = @"select * from master.resetpasswordtoken
                                where token_secret=@token_secret and status='Issued'";

                var record = await _dataAccess.QueryFirstOrDefaultAsync(query, parameter);
                return (record != null ? MapToken(record) : null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResetPasswordToken> GetIssuedResetTokenByAccountId(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", accountId);

                string query = @"select * from master.ResetPasswordToken 
                                where account_id=@account_id and status='Issued'";

                var record = await _dataAccess.QueryFirstOrDefaultAsync(query, parameter);
                return (record != null ? MapToken(record) : null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<MenuFeatureDto>> GetMenuFeaturesList(MenuFeatureRquest request, int[] vehicleids)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@account_id", request.AccountId);
                parameter.Add("@role_id", request.RoleId);
                parameter.Add("@organization_id", request.OrganizationId);
                parameter.Add("@code", request.LanguageCode);
                parameter.Add("@context_org_id", request.ContextOrgId);
                parameter.Add("@vehicleids", vehicleids);
                string query =
                    @"SELECT DISTINCT
                    f.id as FeatureId, f.name as FeatureName, f.type as FeatureType, f.key as FeatureKey, f.level as FeatureLevel, mn.id as MenuId, mn.sort_id as MenuSortId, mn.name as MenuName, tl.value as TranslatedValue, COALESCE(mn2.name, '') as ParentMenuName, mn.key as MenuKey, mn.url as MenuUrl, mn.seq_no as MenuSeqNo
                    FROM
                    (
	                    --Account Route
	                    SELECT f.id
	                    FROM master.Account acc
	                    INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND acc.id = @account_id AND ar.organization_id = @organization_id AND ar.role_id = @role_id AND acc.state = 'A'
	                    INNER JOIN master.Role r ON ar.role_id = r.id AND r.state = 'A'
	                    INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
 	                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND f.name not like 'api.%' and r.level <= f.level
	                    INTERSECT
	                    --Subscription Route
	                    SELECT f.id
	                    FROM
	                    (
		                    SELECT pkg.feature_set_id
		                    FROM master.Package pkg
		                    INNER JOIN master.Subscription s ON s.package_id = pkg.id AND s.organization_id = @context_org_id AND s.state = 'A' AND pkg.state = 'A'
		                    UNION
		                    SELECT pkg.feature_set_id FROM master.Package pkg WHERE pkg.type='P' AND pkg.state = 'A'    --Consider platform type packages
                            UNION 
                            select p.feature_set_id
                            from master.subscription s 
                            inner join master.package p on p.id=s.package_id and s.type='N'
                            inner join master.vehicle v on s.vehicle_id = v.id  and v.id = ANY(@vehicleids) 
	                    ) subs
                        INNER JOIN master.FeatureSet fset ON subs.feature_set_id = fset.id AND fset.state = 'A'
	                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND f.name not like 'api.%'
                    ) fsets
                    INNER JOIN master.Feature f ON f.id = fsets.id AND f.state = 'A' AND f.type <> 'D' AND f.name not like 'api.%'
                    LEFT JOIN master.Menu mn ON mn.feature_id = f.id AND mn.state = 'A' AND mn.id <> 0
                    LEFT JOIN master.Menu mn2 ON mn.parent_id = mn2.id AND mn2.state = 'A' AND mn2.id <> 0
                    LEFT JOIN translation.translation tl ON tl.name = mn.key AND tl.code = @code
                    ORDER BY MenuSortId, MenuSeqNo";

                var record = await _dataAccess.QueryAsync<MenuFeatureDto>(query, parameter);
                return record;
            }
            catch (Exception)
            {
                throw;
            }
        }
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

        private PasswordPolicyAccount TopasswordPolicyBlockAccount(dynamic item)
        {
            var passwordPolicyBlockAccount = new PasswordPolicyAccount();
            passwordPolicyBlockAccount.AccountId = item.account_id;
            passwordPolicyBlockAccount.FailedLoginAttempts = item.failed_login_attempts;
            passwordPolicyBlockAccount.LockedUntil = item.locked_until;
            passwordPolicyBlockAccount.AccountLockAttempts = item.account_lock_attempts;
            passwordPolicyBlockAccount.IsBlocked = item.is_blocked;
            passwordPolicyBlockAccount.LastLogin = item.last_login;
            return passwordPolicyBlockAccount;
        }

        private Account MapAccount(dynamic record)
        {
            if (record is null)
                return null;

            Account account = new Account();
            account.Id = record.id;
            account.EmailId = record.email;
            account.Salutation = record.salutation;
            account.FirstName = record.first_name;
            account.LastName = record.last_name;
            account.DriverId = record.driver_id;
            account.OwnerEmail = record.owner_email;
            account.AccountType = (AccountType)Convert.ToChar(record.type);
            return account;
        }
        private Account Map(dynamic record)
        {
            Account account = new Account();
            account.Id = record.id;
            if (record.email is object)
                account.EmailId = record.email;
            if (record.salutation is object)
                account.Salutation = record.salutation;
            if (record.first_name is object)
                account.FirstName = record.first_name;
            if (record.last_name is object)
                account.LastName = record.last_name;
            account.Organization_Id = record.organization_id;
            account.AccountType = (AccountType)Convert.ToChar(record.accounttype);
            if (record.preference_id is object)
                account.PreferenceId = (int)record.preference_id;
            if (record.blob_id is object) account.BlobId = (int)record.blob_id;
            if (record.driver_id is object) account.DriverId = record.driver_id;
            account.CreatedAt = null;
            if ((record.created_at as object) != null)
            {
                account.CreatedAt = record.created_at;
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
        private AccountVehicleAccessRelationship MapVehicleAccessRelationship(AccountAccessRelationshipEntity record)
        {
            AccountVehicleAccessRelationship entity = new AccountVehicleAccessRelationship();
            entity.Id = record.Id;
            entity.Name = record.Name ?? string.Empty;
            entity.AccessType = (AccessRelationType)Convert.ToChar(record.Access_type);
            entity.Count = record.Count;
            entity.IsGroup = record.Is_group;
            entity.RelationshipData = new List<RelationshipData>();
            entity.RelationshipData.Add(MapAccessRelationshipData(record));
            return entity;
        }
        //private RelationshipData MapAccountAccessRelationship(AccountAccessRelationshipEntity record)
        //{
        //    RelationshipData entity = new RelationshipData();
        //    entity.Id = record.id;
        //    entity.Name = record.name;
        //    entity.AccessType = (AccessRelationType)Convert.ToChar(record.access_type);
        //    entity.Count = record.count;
        //    entity.IsGroup = record.is_group;
        //    entity.RelationshipData = new List<RelationshipData>();
        //    entity.RelationshipData.Add(MapAccessRelationshipData(record));
        //    return entity;
        //}
        private RelationshipData MapAccessRelationshipData(AccountAccessRelationshipEntity record)
        {
            RelationshipData entity = new RelationshipData();
            entity.Id = record.Group_id;
            entity.Name = record.Group_name ?? string.Empty;
            entity.IsGroup = record.Is_ag_vg_group;
            return entity;
        }
        //private RelationshipData MapAccessRelationshipData(AccountAccessRelationshipEntity record)
        //{
        //    RelationshipData entity = new RelationshipData();
        //    entity.Id = record.vehicle_id;
        //    entity.Name = record.vehicle_name;
        //    entity.IsGroup = record.is_vehicle_group;
        //    return entity;
        //}

        private ResetPasswordToken MapToken(dynamic record)
        {
            ResetPasswordToken objToken = new ResetPasswordToken();
            objToken.Id = record.id;
            objToken.AccountId = record.account_id;
            objToken.ProcessToken = record.token_secret;
            objToken.Status = Enum.Parse<ResetTokenStatus>(record.status, false);
            objToken.ExpiryAt = record.expiry_at;
            objToken.CreatedAt = record.created_at;
            return objToken;
        }
        #endregion

        #region AccountSSO
        public async Task<List<SSOTokenResponse>> GetAccountSSODetails(AccountToken account)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                List<SSOTokenResponse> response = new List<SSOTokenResponse>();
                if (account.AccountId > 0)
                {
                    query = @"WITH cte_account
                                AS (
	                                SELECT DISTINCT act.preference_id AS preferenceid
		                                ,act.email AS accountid
		                                ,CONCAT (act.first_name,' ',act.last_name) AS accountName
                                        ,r.code AS roleid
                                    FROM master.account act
                                    INNER JOIN master.accountrole actrole ON act.id = actrole.account_id
                                    INNER JOIN master.role r ON r.id = actrole.role_id
                                    WHERE act.STATE = 'A' AND act.id=@accountID and actrole.role_id=@roleID)
                            ,cte_actpreference
                                AS (
	                                SELECT _timezone.name AS timezonename
		                                ,_dateformat.name AS dateformat
		                                ,_unit.code AS UnitDisplay
                                        ,_vehicledisplay.code as VehicleDisplay
                                        ,_timeformat.code as TimeFormat
                                        ,_language.code as Language
		                                ,actp.id AS accountpreferenceid
		                                ,actp.vehicle_display_id AS VehicleDisplayId
		                                ,actp.timezone_id AS TimeZone
		                                ,actp.date_format_id
		                                ,actp.unit_id
	                                FROM master.accountpreference actp
	                                INNER JOIN master.timezone _timezone ON _timezone.id = actp.timezone_id
	                                INNER JOIN master.unit _unit ON _unit.id = actp.unit_id
									INNER JOIN master.DATEFORMAT _dateformat ON _dateformat.id = actp.date_format_id
									INNER JOIN master.vehicledisplay _vehicledisplay ON _vehicledisplay.id = actp.vehicle_display_id
                                    INNER JOIN master.timeformat _timeformat ON _timeformat.id = actp.time_format_id
                                    INNER JOIN translation.language _language ON _language.id = actp.language_id
                            )
                            SELECT cte_act.accountid
                            ,cte_act.accountname
                            ,cte_act.roleid
                            ,(select org_id from master.organization where id=@OrganizationID) as organizationid
                            ,(select name as organizationname from master.organization where id=@OrganizationID) as organizationname
                            ,cte_actp.timezonename as timezone
                            ,cte_actp.DATEFORMAT
                            ,cte_actp.unitdisplay
                            ,cte_actp.vehicledisplay
                            ,cte_actp.TimeFormat
                            ,cte_actp.Language
                            FROM cte_actpreference cte_actp
                            RIGHT JOIN cte_account cte_act ON cte_act.preferenceid = cte_actp.accountpreferenceid";
                    //}
                    parameter.Add("@accountID", account.AccountId);
                    parameter.Add("@roleID", account.RoleId);
                    parameter.Add("@organizationID", account.OrganizationId);
                    IEnumerable<SSOTokenResponse> accountDetails = await _dataAccess.QueryAsync<SSOTokenResponse>(query, parameter);
                    response = accountDetails.ToList();
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public async Task<AccountPreferenceResponse> GetAccountPreferences(string accountEmail, int organisationId)
        {
            try
            {
                var prefId = await GetAccountPreferenceId(accountEmail, organisationId);

                return await GetAccountPreferencesById(prefId ?? 0) ?? new AccountPreferenceResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAccountPreferences(UpdatePreferencesDataServiceRequest request)
        {
            _dataAccess.Connection.Open();
            var transaction = _dataAccess.Connection.BeginTransaction();
            try
            {
                int result = 0;
                var parameter = new DynamicParameters();
                parameter.Add("@AccountEmail", request.AccountEmail.ToLower());

                var query = @"SELECT preference_id FROM master.account WHERE lower(email) = @AccountEmail";
                var accountPreferenceId = await _dataAccess.QueryFirstAsync<int?>(query, parameter);

                parameter = new DynamicParameters();
                parameter.Add("@Language", request.Language.ToLower().Trim());
                parameter.Add("@TimeZone", request.TimeZone.ToLower().Trim());
                parameter.Add("@TimeFormat", request.TimeFormat.ToLower().Trim());
                parameter.Add("@UnitDisplay", request.UnitDisplay.ToLower().Trim());
                parameter.Add("@VehicleDisplay", request.VehicleDisplay.ToLower().Trim());
                parameter.Add("@DateFormat", request.DateFormat.ToLower().Trim());

                if (accountPreferenceId.HasValue)
                {
                    parameter.Add("@PreferenceId", accountPreferenceId.Value);
                    query = @"UPDATE master.accountpreference 
                              SET
                                language_id = (SELECT id FROM translation.language WHERE lower(code) = @Language),
                                date_format_id = (SELECT id FROM master.dateformat WHERE lower(name) = @DateFormat),
                                timezone_id = (SELECT id FROM master.timezone WHERE lower(name) = @TimeZone),
                                time_format_id = (SELECT id FROM master.timeformat WHERE lower(code) = @TimeFormat),
                                unit_id = (SELECT id FROM master.unit WHERE lower(code) = @UnitDisplay),
                                vehicle_display_id = (SELECT id FROM master.vehicledisplay WHERE lower(code) = @VehicleDisplay)
                              WHERE id = @PreferenceId RETURNING id";
                    result = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                else
                {
                    query = @"INSERT INTO master.accountpreference
                                (type, state, icon_id, page_refresh_time, currency_id, landing_page_display_id, language_id, timezone_id,
                                 unit_id, vehicle_display_id, date_format_id, time_format_id) 
                              VALUES ('A', 'A', NULL, 2, 
                                       (SELECT id FROM master.currency WHERE lower(name) like 'euro%'),
                                       (SELECT id FROM master.landingpagedisplay WHERE lower(name) = 'dashboard'), 
                                       (SELECT id FROM translation.language WHERE lower(code) = @Language),
                                       (SELECT id FROM master.timezone WHERE lower(name) = @TimeZone),
                                       (SELECT id FROM master.unit WHERE lower(code) = @UnitDisplay),
                                       (SELECT id FROM master.vehicledisplay WHERE lower(code) = @VehicleDisplay),
                                       (SELECT id FROM master.dateformat WHERE lower(name) = @DateFormat),                                       
                                       (SELECT id FROM master.timeformat WHERE lower(code) = @TimeFormat)) RETURNING id";
                    var preferenceId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                    parameter = new DynamicParameters();
                    parameter.Add("@PreferenceId", preferenceId);
                    parameter.Add("@AccountEmail", request.AccountEmail.ToLower());

                    result = await _dataAccess.ExecuteScalarAsync<int>("UPDATE master.account SET preference_id=@PreferenceId WHERE lower(email) = @AccountEmail RETURNING id", parameter);
                }

                transaction.Commit();
                return result > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
        }

        public async Task<ValidateDriverResponse> ValidateDriver(string accountEmail, int organisationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@OrganisationId", organisationId);
                parameter.Add("@AccountEmail", accountEmail);

                var account = await GetAccountByEmailId(accountEmail);
                var accountOrgs = await GetAccountOrgs(account.Id);

                var preferenceId = await GetAccountPreferenceId(accountEmail, organisationId);

                var response = await GetAccountPreferencesById(preferenceId ?? 0);

                var finalResponse = new ValidateDriverResponse
                {
                    AccountID = accountEmail,
                    AccountName = $"{ account.FirstName } { account.LastName }",
                    DateFormat = response?.DateFormat,
                    TimeFormat = response?.TimeFormat,
                    TimeZone = response?.TimeZone,
                    UnitDisplay = response?.UnitDisplay,
                    VehicleDisplay = response?.VehicleDisplay,
                    Language = response?.Language,
                    Organisations = accountOrgs.Select(x => new ValidateDriverOrganisation { Id = x.OrgCode, Name = x.Name }).ToList()
                };
                return finalResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetDriverRoleId()
        {
            try
            {
                var query = @"SELECT id FROM master.role WHERE organization_id IS NULL AND code='DRIVER'";

                return await _dataAccess.ExecuteScalarAsync<int>(query, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CountryDetails>> GetCountryDetails(CountryFilter countryFilter)
        {
            try
            {

                var parameter = new DynamicParameters();
                var queryStatement = @"SELECT id,name,
                                        region_type as RegionType, code, 
                                       dial_code as DialCode
	                                   FROM master.country where 1=1";

                if (!string.IsNullOrEmpty(countryFilter.Code))
                {
                    parameter.Add("@code", countryFilter.Code);
                    queryStatement = queryStatement + " and code = @code";
                }

                if (!string.IsNullOrEmpty(countryFilter.RegionType))
                {
                    parameter.Add("@region_type", countryFilter.RegionType);
                    queryStatement = queryStatement + " and region_type = @region_type";
                }

                if (!string.IsNullOrEmpty(countryFilter.DialCode))
                {
                    parameter.Add("@dialCode", countryFilter.DialCode);
                    queryStatement = queryStatement + " and dial_code = @dialCode";

                }
                return await _dataAccess.QueryAsync<CountryDetails>(queryStatement, parameter);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AccountMigration>> GetPendingAccountsForCreation()
        {
            try
            {
                var query = @"SELECT account_id as AccountId, first_name as FirstName, last_name as LastName, email 
                              FROM master.accountmigration am
                              INNER JOIN master.account a ON a.id = am.account_id
                              WHERE am.state='P'";

                return await _dataAccess.QueryAsync<AccountMigration>(query, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAccountMigrationState(int accountId, AccountMigrationState state)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@account_id", accountId);
                parameters.Add("@state", (char)state);
                var query = @"UPDATE master.accountmigration SET state=@state WHERE account_id = @account_id RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                return id > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

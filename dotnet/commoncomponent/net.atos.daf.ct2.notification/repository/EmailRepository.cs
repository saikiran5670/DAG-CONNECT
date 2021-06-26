using System;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notification.entity;

namespace net.atos.daf.ct2.notification.repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDataAccess _dataAccess;
        public EmailRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
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
        public async Task<NotificationAccount> GetAccountByEmailId(string emailId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@email", emailId.ToLower());
                var query = @"select id, email, salutation, first_name, last_name,organization_id from master.account where lower(email) = @email and state='A'";

                dynamic result = await _dataAccess.QuerySingleAsync<dynamic>(query, parameter);

                return MapAccount(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private NotificationAccount MapAccount(dynamic record)
        {
            NotificationAccount account = new NotificationAccount();
            account.Id = record.id;
            account.EmailId = record.email;
            account.Salutation = record.salutation;
            account.FirstName = record.first_name;
            account.LastName = record.last_name;
            account.Organization_Id = record.organization_id;
            return account;
        }


    }
}

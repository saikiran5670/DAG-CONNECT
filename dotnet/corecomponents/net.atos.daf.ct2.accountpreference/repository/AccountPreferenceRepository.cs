using System;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using System.Transactions;
using System.Text;

namespace net.atos.daf.ct2.accountpreference
{
    public class AccountPreferenceRepository : IAccountPreferenceRepository
    {
        private readonly IDataAccess dataAccess;
        public AccountPreferenceRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<AccountPreference> Create(AccountPreference preference)
        {
            try
            {
                var parameter = new DynamicParameters();
                int PreferenceId=0;
                int Id=0;
                string queryCheck = string.Empty;
                parameter.Add("@ref_id", preference.RefId);
                parameter.Add("@type", (char)preference.PreferenceType);
                parameter.Add("@language_id", preference.LanguageId);
                parameter.Add("@timezone_id", preference.TimezoneId);
                parameter.Add("@currency_id", preference.CurrencyId);
                parameter.Add("@unit_id", preference.UnitId);
                parameter.Add("@vehicle_display_id", preference.VehicleDisplayId);
                parameter.Add("@date_format_id", preference.DateFormatTypeId);
                parameter.Add("@time_format_id", preference.TimeFormatId);
                parameter.Add("@landing_page_display_id", preference.LandingPageDisplayId);
                //parameter.Add("@driver_id", preference.DriverId);

                // check the ref_id must be account id or organization id                
                if (preference.PreferenceType == PreferenceType.Account)
                {
                    // check if preference does not exists 
                    queryCheck = "select preference_id from master.account where is_active=true and id=@ref_id";
                    PreferenceId = await dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (PreferenceId > 0)
                    {
                        preference.Exists = true;
                        return preference;
                    }
                    // in valid account preference
                    queryCheck = "select a.id from master.account a join master.accountorg ag on a.id = ag.account_id and ag.is_active=true where a.id=@ref_id";
                    Id = await dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (Id <= 0)
                    {
                        preference.RefIdNotValid = true;                        
                        return preference;
                    }
                }
                // check the ref_id must be account id or organization id                
                if (preference.PreferenceType == PreferenceType.Organization)
                {
                    // check if preference does not exists 
                    queryCheck = "select preference_id from master.organization where is_active=true and id=@ref_id";
                    PreferenceId = await dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (PreferenceId > 0)
                    {
                        preference.Exists = true;
                        return preference;
                    }
                    // invalid organization
                    queryCheck = "select id from master.organization where is_active=true and id=@ref_id";
                    Id = await dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (Id <= 0)
                    {
                        preference.RefIdNotValid = true;                        
                        return preference;
                    }
                }

                string query = @"insert into master.accountpreference
                                (type,language_id,timezone_id,
                                currency_id,unit_id,vehicle_display_id,date_format_id,is_active,time_format_id,landing_page_display_id) 
                                values (@type,@language_id,@timezone_id,
                                @currency_id,@unit_id,@vehicle_display_id,@date_format_id,true,@time_format_id,@landing_page_display_id) RETURNING id";

                var preferenceId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                // Update preference id for account or organization
                if (preference.PreferenceType == PreferenceType.Account)
                {
                    queryCheck = "update master.account set preference_id=@preference_id where id=@ref_id";                    
                    
                }
                else if (preference.PreferenceType == PreferenceType.Organization)
                {
                    queryCheck = "update master.organization set preference_id=@preference_id where id=@ref_id";
                }                
                parameter.Add("@preference_id", preferenceId);
                await dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                preference.Id = preferenceId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return preference;
        }
        public async Task<AccountPreference> Update(AccountPreference preference)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@ref_id", preference.RefId);
                parameter.Add("@id", preference.Id);
                parameter.Add("@type", (char)preference.PreferenceType);
                parameter.Add("@language_id", preference.LanguageId);
                parameter.Add("@timezone_id", preference.TimezoneId);
                parameter.Add("@currency_id", preference.CurrencyId);
                parameter.Add("@unit_id", preference.UnitId);
                parameter.Add("@vehicle_display_id", preference.VehicleDisplayId);
                parameter.Add("@date_format_id", preference.DateFormatTypeId);
                parameter.Add("@time_format_id", preference.TimeFormatId);
                parameter.Add("@landing_page_display_id", preference.LandingPageDisplayId);
                //parameter.Add("@driver_id", preference.DriverId);

                var query = @"update master.accountpreference set language_id=@language_id,
                            timezone_id=@timezone_id,currency_id=@currency_id,unit_id=@unit_id,
                            vehicle_display_id=@vehicle_display_id,
                            date_format_id=@date_format_id,is_active=true,time_format_id=@time_format_id,landing_page_display_id=@landing_page_display_id
	                        WHERE id=@id RETURNING id;";
                var Id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return preference;

        }
        public async Task<bool> Delete(int preferenceID,PreferenceType preferenceType)
        {
            try
            {
                var parameter = new DynamicParameters();
                StringBuilder query = new StringBuilder();
                string checkPreferenceQuery = string.Empty;
                int id = 0;
                parameter.Add("@id", preferenceID);
                checkPreferenceQuery = @"select id from master.accountpreference where id=@id and is_active=true";
                id = await dataAccess.ExecuteScalarAsync<int>(checkPreferenceQuery, parameter);
                if (id == 0) return false;                
                //using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                //{
                    query.Append("update master.accountpreference set is_active=false where id=@id");
                    //result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);                    
                    // Update preference id for account or organization
                    if (preferenceType == PreferenceType.Account)
                    {
                        query.Append (" ; " + "update master.account set preference_id=null where preference_id=@id;");
                    }
                    else if (preferenceType == PreferenceType.Organization)
                    {
                        query.Append(" ; " + "update master.organization set preference_id=null where preference_id=@id;");
                    }
                    await dataAccess.ExecuteScalarAsync<int>(query.ToString(), parameter);
                    //transactionScope.Complete();
                //}
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<AccountPreference> entity = new List<AccountPreference>();
                parameter.Add("@id", filter.Id);
                var query = @"select id,type,language_id,timezone_id,currency_id,unit_id,vehicle_display_id,date_format_id,time_format_id,vehicle_display_id,is_active,landing_page_display_id from master.accountpreference where is_active=true and id= @id order by 1 desc limit 1";
                // if (filter != null)
                // {
                //     // id filter
                //     if (filter.Id > 0)
                //     {
                //         parameter.Add("@id", filter.Id);
                //         query = query + " and id= @id";
                //     }
                    // // account or organization id filter
                    // if (filter.Ref_Id > 0)
                    // {
                    //     parameter.Add("@Ref_id", filter.Ref_Id);
                    //     query = query + " and Ref_Id= @Ref_Id";
                    // }
                    // type filter                    
                    // if (((char)filter.PreferenceType) != ((char)PreferenceType.None))
                    // {
                    //     parameter.Add("@type", (char)filter.PreferenceType);
                    //     query = query + " and type= @type";
                    // }
                //     query = query + @" order by 1 desc limit 1";
                // }
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                //Account account;
                foreach (dynamic record in result)
                {
                    entity.Add(Map(record));
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AccountPreference Map(dynamic record)
        {
            AccountPreference entity = new AccountPreference();
            entity.Id = record.id;
            //entity.RefId = record.ref_id;
            entity.PreferenceType = (PreferenceType)Convert.ToChar(record.type);
            entity.LanguageId = record.language_id;
            entity.TimezoneId = record.timezone_id;
            entity.CurrencyId = record.currency_id;
            entity.UnitId = record.unit_id;
            entity.VehicleDisplayId = record.vehicle_display_id;
            entity.DateFormatTypeId = record.date_format_id;
            //if (Convert.ToString(record.driver_id) != null) entity.DriverId = record.driver_id;
            entity.TimeFormatId = record.time_format_id;
            entity.LandingPageDisplayId = record.landing_page_display_id;
            record.isActive = record.is_active;
            return entity;
        }

    }
}

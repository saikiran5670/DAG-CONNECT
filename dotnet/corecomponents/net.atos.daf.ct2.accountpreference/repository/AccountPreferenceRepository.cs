using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.accountpreference
{
    public class AccountPreferenceRepository : IAccountPreferenceRepository
    {
        private readonly IDataAccess _dataAccess;
        readonly string _iconName = "Preference_Icon";
        public AccountPreferenceRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }


        public async Task<AccountPreference> Create(AccountPreference preference)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                var parameter = new DynamicParameters();
                int iconId = 0;
                if (!string.IsNullOrEmpty(preference.IconByte))
                {
                    parameter.Add("@icon", Convert.FromBase64String(preference.IconByte));
                    parameter.Add("@type", (char)preference.PreferenceType);//For account preference
                    parameter.Add("@name", _iconName);//need to add name of the icon
                    parameter.Add("@state", 'A');
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@created_by", preference.CreatedBy);
                    parameter.Add("@modified_at", 0);
                    parameter.Add("@modified_by", 0);
                    string queryicon = @"insert into master.icon
                                (icon,type,name,state,created_at,created_by,modified_at,modified_by) 
                                values (@icon,@type,@name,@state,@created_at,@created_by,@modified_at,@modified_by)RETURNING id";

                    iconId = await _dataAccess.ExecuteScalarAsync<int>(queryicon, parameter);
                }
                int preferenceId = 0;
                int id = 0;
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
                parameter.Add("@Icon_id", iconId);
                parameter.Add("@page_refresh_time", preference.PageRefreshTime);
                //parameter.Add("@driver_id", preference.DriverId);

                // check the ref_id must be account id or organization id
                //comment code open
                if (preference.PreferenceType == PreferenceType.Account)
                {
                    // check if preference does not exists 
                    queryCheck = "select preference_id from master.account where state='A' and id=@ref_id";
                    preferenceId = await _dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (preferenceId > 0)
                    {
                        preference.Exists = true;
                        return preference;
                    }
                    // in valid account preference
                    queryCheck = "select a.id from master.account a join master.accountorg ag on a.id = ag.account_id and ag.state='A' where a.id=@ref_id";
                    id = await _dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (id <= 0)
                    {
                        preference.RefIdNotValid = true;
                        return preference;
                    }
                }
                // check the ref_id must be account id or organization id                
                if (preference.PreferenceType == PreferenceType.Organization)
                {
                    // check if preference does not exists 
                    queryCheck = "select preference_id from master.organization where state='A' and id=@ref_id";
                    preferenceId = await _dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (preferenceId > 0)
                    {
                        preference.Exists = true;
                        return preference;
                    }
                    // invalid organization
                    queryCheck = "select id from master.organization where state='A' and id=@ref_id";
                    id = await _dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                    if (id <= 0)
                    {
                        preference.RefIdNotValid = true;
                        return preference;
                    }
                }
                //comment code end

                string query = @"insert into master.accountpreference
                                (type,language_id,timezone_id,
                                currency_id,unit_id,vehicle_display_id,date_format_id,state,time_format_id,landing_page_display_id,icon_id,page_refresh_time) 
                                values (@type,@language_id,@timezone_id,
                                @currency_id,@unit_id,@vehicle_display_id,@date_format_id,'A',@time_format_id,@landing_page_display_id,@Icon_id,@page_refresh_time) RETURNING id";

                var createdPreferenceId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                // Update preference id for account or organization
                //comment code open
                if (preference.PreferenceType == PreferenceType.Account)
                {
                    queryCheck = "update master.account set preference_id=@preference_id where id=@ref_id";

                }
                else if (preference.PreferenceType == PreferenceType.Organization)
                {
                    queryCheck = "update master.organization set preference_id=@preference_id where id=@ref_id";
                }
                parameter.Add("@preference_id", createdPreferenceId);
                await _dataAccess.ExecuteScalarAsync<int>(queryCheck, parameter);
                //comment code end
                preference.Id = createdPreferenceId;
                preference.IconId = iconId;

                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }

            finally
            {
                _dataAccess.Connection.Close();
            }
            return preference;
        }
        public async Task<AccountPreference> Update(AccountPreference preference)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();

            try
            {
                int createdIconId = 0;
                var parameter = new DynamicParameters();
                if (!string.IsNullOrEmpty(preference.IconByte))
                {
                    string queryicon;
                    int iconIdExists = await _dataAccess.QuerySingleAsync<int>("select coalesce((SELECT Icon_id FROM master.accountpreference where id=@id), 0)", new { id = preference.Id });
                    parameter.Add("@icon", Convert.FromBase64String(preference.IconByte));
                    parameter.Add("@type", (char)preference.PreferenceType);
                    parameter.Add("@name", _iconName);
                    parameter.Add("@state", 'A');
                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@modified_by", preference.CreatedBy);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@created_by", preference.CreatedBy);
                    if (iconIdExists > 0)
                    {
                        queryicon = @"update  master.icon set 
                                icon=@icon,type=@type,name=@name,state=@state,modified_at=@modified_at,modified_by=@modified_by where id=" + preference.IconId + " RETURNING id;";
                    }
                    else
                    {
                        queryicon = @"insert into master.icon
                                (icon,type,name,state,created_at,created_by) 
                                values (@icon,@type,@name,@state,@created_at,@created_by)RETURNING id";

                    }
                    createdIconId = await _dataAccess.ExecuteScalarAsync<int>(queryicon, parameter);
                }
                parameter.Add("@id", preference.Id);
                parameter.Add("@language_id", preference.LanguageId);
                parameter.Add("@timezone_id", preference.TimezoneId);
                parameter.Add("@currency_id", preference.CurrencyId);
                parameter.Add("@unit_id", preference.UnitId);
                parameter.Add("@vehicle_display_id", preference.VehicleDisplayId);
                parameter.Add("@date_format_id", preference.DateFormatTypeId);
                parameter.Add("@time_format_id", preference.TimeFormatId);
                parameter.Add("@landing_page_display_id", preference.LandingPageDisplayId);
                parameter.Add("@page_refresh_time", preference.PageRefreshTime);

                if (preference.IconId > 0)
                {
                    parameter.Add("@Icon_id", preference.IconId);
                }
                else
                {
                    parameter.Add("@Icon_id", createdIconId);
                }

                var query = @"update master.accountpreference set language_id=@language_id,
                            timezone_id=@timezone_id,currency_id=@currency_id,unit_id=@unit_id,
                            vehicle_display_id=@vehicle_display_id,
                            date_format_id=@date_format_id,state='A',time_format_id=@time_format_id,landing_page_display_id=@landing_page_display_id,icon_id=@icon_id,page_refresh_time=@page_refresh_time
	                        WHERE id=@id RETURNING id;";
                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                preference.IconId = createdIconId;
                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }

            finally
            {
                _dataAccess.Connection.Close();
            }
            return preference;

        }
        public async Task<bool> Delete(int preferenceID, PreferenceType preferenceType)
        {
            try
            {
                var parameter = new DynamicParameters();
                StringBuilder query = new StringBuilder();
                string checkPreferenceQuery = string.Empty;
                int id = 0;
                parameter.Add("@id", preferenceID);
                checkPreferenceQuery = @"select id from master.accountpreference where id=@id and state='A'";
                id = await _dataAccess.ExecuteScalarAsync<int>(checkPreferenceQuery, parameter);
                if (id == 0) return false;
                //using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                //{
                query.Append("update master.accountpreference set state='D' where id=@id");
                int icon_id = 0;
                checkPreferenceQuery = @"select icon_id from master.accountpreference where id=@id and state='A'";
                icon_id = await _dataAccess.ExecuteScalarAsync<int>(checkPreferenceQuery, parameter);
                query.Append(" ; " + "update master.icon set state='D' , modified_at= " + UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()) + " where id=" + icon_id + " ");
                //result = await dataAccess.ExecuteScalarAsync<int>(query, parameter);                    
                // Update preference id for account or organization
                if (preferenceType == PreferenceType.Account)
                {
                    query.Append(" ; " + "update master.account set preference_id=null where preference_id=@id;");
                }
                else if (preferenceType == PreferenceType.Organization)
                {
                    query.Append(" ; " + "update master.organization set preference_id=null where preference_id=@id;");
                }
                await _dataAccess.ExecuteScalarAsync<int>(query.ToString(), parameter);
                //transactionScope.Complete();
                //}
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AccountPreference>> Get(AccountPreferenceFilter filter)
        {
            try
            {


                var parameter = new DynamicParameters();
                List<AccountPreference> entity = new List<AccountPreference>();
                parameter.Add("@id", filter.Id);
                var query = @"SELECT ac.id,ac.type,ac.language_id,ac.timezone_id,ac.currency_id,ac.unit_id,ac.vehicle_display_id,
                            ac.date_format_id,ac.time_format_id,ac.state,ac.landing_page_display_id,
                            COALESCE(i.id,0) as iconId,i.icon,ac.page_refresh_time
                            FROM   master.accountpreference ac
                            LEFT OUTER JOIN master.icon i
                            ON i.id = ac.icon_id
                            WHERE ac.state = 'A'
                            AND ac.id = @id
                            ORDER BY 1 DESC
                            LIMIT  1 ";
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
                dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                //Account account;
                foreach (dynamic record in result)
                {
                    entity.Add(Map(record));
                }
                return entity;
            }
            catch (Exception)
            {
                throw;
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
            entity.IconId = record.iconid;
            if (record.icon != null && record.icon.Length > 0)
            {
                //string base64String = Convert.ToBase64String(record.icon, 0, record.icon.Length);
                entity.IconByte = Convert.ToBase64String(record.icon, 0, record.icon.Length);
            }
            //record.isActive = record.state;
            entity.PageRefreshTime = record.PageRefreshTime;
            return entity;
        }

    }
}

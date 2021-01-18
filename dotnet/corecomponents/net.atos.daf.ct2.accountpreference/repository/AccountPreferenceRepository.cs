using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;

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
                parameter.Add("@ref_id", preference.Ref_Id);
                parameter.Add("@type", (char)preference.PreferenceType);
                parameter.Add("@language_id", preference.Language_Id);
                parameter.Add("@timezone_id", preference.Timezone_Id);
                parameter.Add("@currency_type", (char)preference.Currency_Type);
                parameter.Add("@unit_type", (char)preference.Unit_Type);
                parameter.Add("@vehicle_display_type", (char)preference.VehicleDisplay_Type);
                parameter.Add("@date_format_type", (char)preference.DateFormat_Type);
                parameter.Add("@driver_id", preference.DriverId);
                parameter.Add("@is_active", preference.Is_Active);

                string query = @"insert into master.accountpreference
                                (ref_id,type,language_id,timezone_id,
                                currency_type,unit_type,vehicle_display_type,date_format_type,driver_id,is_active
                                ) values (
                                @ref_id,@type,@language_id,@timezone_id,
                                @currency_type,@unit_type,@vehicle_display_type,@date_format_type,@driver_id,true
                                ) RETURNING id";

                var preferenceId = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

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
                parameter.Add("@id", preference.Id);
                parameter.Add("@ref_id", preference.Ref_Id);
                parameter.Add("@type", (char)preference.PreferenceType);
                parameter.Add("@language_id", preference.Language_Id);
                parameter.Add("@timezone_id", preference.Timezone_Id);
                parameter.Add("@currency_type", (char)preference.Currency_Type);
                parameter.Add("@unit_type", (char)preference.Unit_Type);
                parameter.Add("@vehicle_display_type", (char)preference.VehicleDisplay_Type);
                parameter.Add("@date_format_type", (char)preference.DateFormat_Type);
                //parameter.Add("@isActive", preference.Is_Active);

                var query = @"update master.accountpreference set ref_id=@ref_id,language_id=@language_id,
                            timezone_id=@timezone_id, currency_type=@currency_type,unit_type=@unit_type,
                            vehicle_display_type=@vehicle_display_type,
                            date_format_type=@date_format_type,is_active=true                                     
	                                WHERE id = @id and type=@type
                                    RETURNING id;";
                var Id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return preference;

        }
        public async Task<bool> Delete(int refId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", refId);
                var query = @"update master.accountpreference set is_active=false where ref_id = @id";
                await dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
                var query = @"select id,ref_id,type,language_id,timezone_id,currency_type,unit_type,vehicle_display_type,date_format_type,is_active from master.accountpreference where is_active=true";

                if (filter != null)
                {
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and id= @id";
                    }
                    // account or organization id filter
                    if (filter.Ref_Id > 0)
                    {
                        parameter.Add("@Ref_id", filter.Ref_Id);
                        query = query + " and Ref_Id= @Ref_Id";
                    }
                    // type filter                    
                    if (((char)filter.PreferenceType) != ((char)PreferenceType.None))
                    {
                        parameter.Add("@type", (char)filter.PreferenceType);
                        query = query + " and type= @type";
                    }
                }
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
                entity.Ref_Id  = record.ref_id;
                entity.PreferenceType = (PreferenceType)Convert.ToChar(record.type);
                entity.Language_Id = record.language_id;
                entity.Timezone_Id = record.timezone_id;
                entity.Currency_Type = (CurrencyType)Convert.ToChar(record.currency_type);
                entity.Unit_Type = (UnitType)Convert.ToChar(record.unit_type);
                entity.VehicleDisplay_Type= (VehicleDisplayType)Convert.ToChar(record.vehicle_display_type);
                entity.DateFormat_Type = (DateFormatDisplayType)Convert.ToChar(record.date_format_type);
                if(Convert.ToString(record.driver_id)!=null) entity.DriverId = record.driver_id;
                record.isActive  = record.is_active;
                
                return entity;
            }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.utilities;
using static net.atos.daf.ct2.utilities.CommonEnums;

namespace net.atos.daf.ct2.termsandconditions.repository
{
    public class TermsAndConditionsRepository : ITermsAndConditionsRepository
    {
        private readonly IDataAccess _dataAccess;

        public TermsAndConditionsRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
        public async Task<AccountTermsCondition> AddUserAcceptedTermCondition(AccountTermsCondition accountTermsCondition)
        {
            try
            {
                var QueryStatement = @"INSERT INTO master.accounttermsacondition
                                      (organization_id
	                                    ,account_id
	                                    ,terms_and_condition_id
	                                    ,accepted_date)
	                             VALUES (@organization_id
	                                    ,@account_id
	                                    ,@terms_and_condition_id
	                                    ,@accepted_date) RETURNING id";


                var parameter = new DynamicParameters();

                parameter.Add("@organization_id", accountTermsCondition.Organization_Id);
                parameter.Add("@account_id", accountTermsCondition.Account_Id);
                parameter.Add("@terms_and_condition_id", accountTermsCondition.Terms_And_Condition_Id);
                parameter.Add("@accepted_date", UTCHandling.GetUTCFromDateTime(accountTermsCondition.Accepted_Date.ToString()));
                parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                int accountTermsConditionID = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                accountTermsCondition.Id = accountTermsConditionID;

                if (accountTermsConditionID > 0)
                {
                    var UpdateVerNoStatement = @"UPDATE master.account
	                                             SET latest_accepted_tac_ver_no=@latest_accepted_tac_ver_no
	                                             WHERE id=@id
                                                 RETURNING latest_accepted_tac_ver_no";
                    parameter.Add("@latest_accepted_tac_ver_no", accountTermsCondition.Version_no);
                    parameter.Add("@id", accountTermsCondition.Account_Id);
                    await _dataAccess.ExecuteScalarAsync<string>(UpdateVerNoStatement, parameter);
                }

                return accountTermsCondition;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId, int OrganizationId)
        {
            try
            {
                List<TermsAndConditions> Objtermcondn = new List<TermsAndConditions>();
                var QueryStatement = string.Empty;
                if (AccountId > 0 && OrganizationId > 0)
                {

                    QueryStatement = @"SELECT terms.id
		                                ,terms.version_no
		                                ,terms.code
		                                ,terms.description
		                                ,terms.state
		                                ,terms.created_at
                                        ,terms.start_date
										,accterm.accepted_date
										,acc.first_name  
										,acc.last_name 
	                                FROM master.termsandcondition terms 
	                                Inner Join master.accounttermsacondition accterm
	                                on terms.id=accterm.terms_and_condition_id
									Inner Join master.account acc
									on accterm.account_id=acc.id
                                    where 1=1";
                }
                else
                {
                    QueryStatement = @"SELECT terms.id
		                                ,terms.version_no
		                                ,terms.code		                               
		                                ,terms.state
		                                ,terms.start_date
                                        ,terms.created_at
										,accterm.accepted_date
										,acc.first_name  
										,acc.last_name 
	                                FROM master.termsandcondition terms 
	                                Inner Join master.accounttermsacondition accterm
	                                on terms.id=accterm.terms_and_condition_id
									Inner Join master.account acc
									on accterm.account_id=acc.id
                                    where 1=1";
                }
                var parameter = new DynamicParameters();

                // Account Filter
                if (AccountId > 0)
                {
                    parameter.Add("@account_id", AccountId);
                    QueryStatement = QueryStatement + " and accterm.account_id=@account_id";
                }

                // OrganizationId Filter
                if (OrganizationId > 0)
                {
                    parameter.Add("@organization_id", OrganizationId);
                    QueryStatement = QueryStatement + " and accterm.organization_id=@organization_id";
                }

                dynamic result = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                TermsAndConditions termsAndConditions = new TermsAndConditions();
                foreach (dynamic record in result)
                {
                    Objtermcondn.Add(Map(record));
                }
                return Objtermcondn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TermsAndConditions> GetLatestTermCondition(int AccountId, int OrganizationId)
        {
            var QueryStatement = @"select id
                                    , version_no
                                    , code
                                    , description
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition 
                                    where id not in(select terms_and_condition_id
		                                       from master.accounttermsacondition
		                                       where account_id=@account_id 
		                                       and organization_id=@organization_id)                                   
                                    and lower(code) = (select SUBSTRING (lower(lang.code), 1,2)
					                                      from master.account acc 
					                                      inner join  master.accountpreference accpref
					                                      on acc.preference_id=accpref.id
					                                      inner join translation.language lang
					                                      on accpref.language_id=lang.id
					                                      and acc.id=@account_id)
                                    and state='A'";
            var parameter = new DynamicParameters();
            parameter.Add("@account_id", AccountId);
            parameter.Add("@organization_id", OrganizationId);
            dynamic result = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            if (((System.Collections.Generic.List<object>)result).Count == 0)
            {
                var DefaultQueryStatement = @"select id
                                    , version_no
                                    , code
                                    , description
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition 
                                    where state='A'                                  
                                    and lower(code)='en'";
                result = await _dataAccess.QueryAsync<dynamic>(DefaultQueryStatement, null);
            }

            TermsAndConditions termsAndConditions = new TermsAndConditions();
            foreach (dynamic record in result)
            {
                termsAndConditions = Map(record);
            }
            return termsAndConditions;
        }

        async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@id", roleId);
            parameter.Add("@organization_id", orgId);
            var data = await _dataAccess.ExecuteScalarAsync
                             (@"select level from master.Role where id=@id and organization_id=@organization_id",
                            parameter);
            int level = data != null ? Convert.ToInt32(data) : 0;
            return level;
        }

        public async Task<List<string>> GetAllVersionNo(VersionByID objVersionByID)
        {
            var parameter = new DynamicParameters();
            string QueryStatement = string.Empty;

            switch (objVersionByID.LevelCode)
            {
                case 10:
                case 20:
                    QueryStatement = @"select id,version_no,code from  master.termsandcondition";
                    break;
                case 30:
                case 40:
                    parameter.Add("@organization_id", objVersionByID.OrgId);
                    parameter.Add("@account_id", objVersionByID.AccountId);
                    QueryStatement = @"select tc.id,tc.version_no,tc.code from  master.termsandcondition tc
                           inner join master.accounttermsacondition atc on atc.terms_and_condition_id = tc.id
                           where atc.organization_id = @organization_id and atc.account_id = @account_id";
                    break;
            }

            dynamic result = await _dataAccess.QueryAsync<TermsAndConditions>(QueryStatement, parameter);
            List<string> ObjVersionList = new List<string>();
            foreach (dynamic record in result)
            {
                ObjVersionList.Add(record.Version_no);
            }

            return ObjVersionList;
        }

        public async Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode)
        {
            List<TermsAndConditions> Objtermcondn = new List<TermsAndConditions>();
            string QueryStatement;
            if ((!string.IsNullOrEmpty(VersionNo)) && (!string.IsNullOrEmpty(LanguageCode)))
            {
                QueryStatement = @"select id
                                    , version_no
                                    , code
                                    , description
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition 
                                    where lower(version_no)=lower(trim(@version_no))";
            }
            else
            {
                QueryStatement = @"select id
                                    , version_no
                                    , code
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition 
                                    where lower(version_no)=lower(trim(@version_no))";
            }

            var parameter = new DynamicParameters();
            parameter.Add("@version_no", VersionNo);

            // Language Code Filter
            if (LanguageCode != null && Convert.ToInt32(LanguageCode.Length) > 0)
            {
                // parameter.Add("@code", "%" + LanguageCode + "%");
                // QueryStatement = QueryStatement + " and code LIKE SUBSTRING(lower(@code), 1,2)";
                parameter.Add("@code", LanguageCode);
                QueryStatement = QueryStatement + " and lower(code) = SUBSTRING (lower(@code), 1,2)";
            }
            dynamic result = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

            foreach (dynamic record in result)
            {
                Objtermcondn.Add(Map(record));
            }
            return Objtermcondn;
        }


        public async Task<TermsAndConditionResponseList> UploadTermsAndCondition(TermsandConFileDataList objTermsandConFileDataList)
        {
            try
            {
                TermsAndConditionResponseList objTermsAndConditionResponseList = new TermsAndConditionResponseList();
                if (objTermsandConFileDataList != null && objTermsandConFileDataList.Data.Count > 0)
                {
                    objTermsAndConditionResponseList.TermsAndConditionDetails = new List<TermsAndConditionResponse>();
                    foreach (var item in objTermsandConFileDataList.Data)
                    {
                        TermsAndConditionResponse objTermsAndConditionResponse = new TermsAndConditionResponse();
                        var descriptionExists = await TermsAndConditionExits(item.Version_no, item.Code);
                        //To insert non existing record
                        if (descriptionExists.Code == null && descriptionExists.Version_no == null)
                        {
                            var QueryStatement = @"INSERT INTO master.termsandcondition 
            (version_no,code,description,state,start_date,end_date,created_at,created_by)
VALUES (@version_no,@code,@description,@state,@start_date,@end_date,@created_at,@created_by) RETURNING id";

                            var parameter = new DynamicParameters();

                            parameter.Add("@version_no", item.Version_no);
                            parameter.Add("@code", item.Code);
                            parameter.Add("@description", item.Description);
                            parameter.Add("@state", "A");
                            parameter.Add("@start_date", objTermsandConFileDataList.Start_date == 0 ? UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()) : objTermsandConFileDataList.Start_date);
                            parameter.Add("@end_date", objTermsandConFileDataList.End_date);
                            parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                            parameter.Add("@created_by", objTermsandConFileDataList.Created_by);

                            int id = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                            if (id > 0)
                            {
                                objTermsAndConditionResponse.Id = id;
                                objTermsAndConditionResponse.FileName = item.FileName;
                                objTermsAndConditionResponse.Action = "Inserted Sucessfully";
                                objTermsAndConditionResponseList.TermsAndConditionDetails.Add(objTermsAndConditionResponse);
                            }
                        }
                        else if (descriptionExists.Code != null && descriptionExists.Version_no != null)
                        {   //To update description when same version and code are passed
                            double dbVersion, feVersion;
                            dbVersion = Convert.ToDouble(descriptionExists.Version_no.Substring(1, descriptionExists.Version_no.Length - 1));
                            feVersion = Convert.ToDouble(item.Version_no.Substring(1, item.Version_no.Length - 1));
                            if (feVersion < dbVersion && descriptionExists.Code == item.Code)
                            {

                                //var QueryStatement = @"UPDATE master.termsandcondition 
                                //       SET description=@description,modified_at=@modified_at
                                //       ,modified_by=@modified_by 
                                //      WHERE version_no=@version_no and code=@code";

                                //var parameter = new DynamicParameters();
                                //parameter.Add("@description", item.description);
                                //parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                                //parameter.Add("@modified_by", objTermsandConFileDataList.accountId);
                                //parameter.Add("@version_no", item.version_no);
                                //parameter.Add("@code", item.code);


                                //int id = await dataAccess.ExecuteAsync(QueryStatement, parameter);
                                //if (id > 0)
                                //{
                                objTermsAndConditionResponse.Id = descriptionExists.Id;
                                objTermsAndConditionResponse.FileName = item.FileName;
                                objTermsAndConditionResponse.Action = $"No action, greater Version already exists for : {descriptionExists.Version_no}_{descriptionExists.Code}";
                                objTermsAndConditionResponseList.TermsAndConditionDetails.Add(objTermsAndConditionResponse);

                                //}
                            }
                            else if (feVersion == dbVersion && descriptionExists.Code == item.Code)
                            {
                                string QueryStatement = string.Empty;
                                var parameter = new DynamicParameters();
                                if (objTermsandConFileDataList.Start_date == 0)
                                {
                                    QueryStatement = @"UPDATE master.termsandcondition 
                                       SET description=@description,modified_at=@modified_at
                                       ,modified_by=@modified_by 
                                      WHERE version_no=@version_no and code=@code";

                                    parameter.Add("@description", item.Description);
                                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                                    parameter.Add("@modified_by", objTermsandConFileDataList.Created_by);
                                    parameter.Add("@version_no", item.Version_no);
                                    parameter.Add("@code", item.Code);
                                }
                                else
                                {
                                    QueryStatement = @"UPDATE master.termsandcondition 
                                       SET description=@description,modified_at=@modified_at
                                       ,modified_by=@modified_by,start_date=@start_date,end_date=@end_date
                                      WHERE version_no=@version_no and code=@code";

                                    parameter.Add("@description", item.Description);
                                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                                    parameter.Add("@modified_by", objTermsandConFileDataList.Created_by);
                                    parameter.Add("@version_no", item.Version_no);
                                    parameter.Add("@code", item.Code);
                                    parameter.Add("@start_date", objTermsandConFileDataList.Start_date);
                                    parameter.Add("@end_date", objTermsandConFileDataList.End_date);
                                }

                                int id = await _dataAccess.ExecuteAsync(QueryStatement, parameter);
                                if (id > 0)
                                {
                                    objTermsAndConditionResponse.Id = descriptionExists.Id;
                                    objTermsAndConditionResponse.FileName = item.FileName;
                                    objTermsAndConditionResponse.Action = "Updated existing Record.";
                                    objTermsAndConditionResponseList.TermsAndConditionDetails.Add(objTermsAndConditionResponse);

                                }
                            }
                            //To update outdate version to state D and Insert new record
                            else if (descriptionExists.Version_no != item.Version_no && descriptionExists.Code == item.Code)
                            {
                                var QueryStatement = @"UPDATE master.termsandcondition 
                             SET state=@state,end_date=@end_date,modified_by=@modified_by 
                             WHERE code=@code";
                                var parameter = new DynamicParameters();
                                parameter.Add("@state", "I");
                                parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                                parameter.Add("@modified_by", objTermsandConFileDataList.Created_by);
                                parameter.Add("@code", item.Code);

                                int rowEffected = await _dataAccess.ExecuteAsync(QueryStatement, parameter);
                                if (rowEffected > 0)
                                {
                                    var Query = @"INSERT INTO master.termsandcondition 
                                   (version_no,code,description,state,start_date,created_at,created_by)
                            VALUES (@version_no,@code,@description,@state,@start_date,@created_at,@created_by) RETURNING id";

                                    var param = new DynamicParameters();
                                    param.Add("@version_no", item.Version_no);
                                    param.Add("@code", item.Code);
                                    param.Add("@description", item.Description);
                                    param.Add("@state", "A");
                                    param.Add("@start_date", objTermsandConFileDataList.Start_date == 0 ? UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()) : objTermsandConFileDataList.Start_date);
                                    param.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                                    param.Add("@created_by", objTermsandConFileDataList.Created_by);

                                    int id = await _dataAccess.ExecuteScalarAsync<int>(Query, param);
                                    if (id > 0)
                                    {
                                        objTermsAndConditionResponse.Id = id;
                                        objTermsAndConditionResponse.FileName = item.FileName;
                                        objTermsAndConditionResponse.Action = "Inserted Sucessfully and disabled previous version";
                                        objTermsAndConditionResponseList.TermsAndConditionDetails.Add(objTermsAndConditionResponse);
                                    }
                                }
                            }
                        }
                    }
                }
                return objTermsAndConditionResponseList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<VersionAndCodeExits> TermsAndConditionExits(string version, string code)
        {
            var QueryStatement = @"select id,version_no,code from master.termsandcondition where code=@code and state=@state";
            var parameter = new DynamicParameters();
            parameter.Add("@version_no", version);
            parameter.Add("@code", code);
            parameter.Add("@state", "A");
            var data = await _dataAccess.QueryFirstOrDefaultAsync<VersionAndCodeExits>(QueryStatement, parameter);
            if (data == null)
            {
                VersionAndCodeExits objVersionAndCodeExits = new VersionAndCodeExits();
                return objVersionAndCodeExits;
            }
            return data;
        }
        public async Task<InactivateTandCStatusResponceList> InactivateTermsandCondition(InactivateTandCRequestList objInactivateTandCRequestList)
        {
            var QueryStatement = @"update  master.termsandcondition set state=I where id=@id";
            InactivateTandCStatusResponceList objInactivateTandCStatusResponceList = new InactivateTandCStatusResponceList();
            foreach (var item in objInactivateTandCRequestList.Ids)
            {
                var parameter = new DynamicParameters();

                parameter.Add("@id", item.Id);
                int rowsEffected = await _dataAccess.ExecuteAsync(QueryStatement, parameter);
                InactivateTandCStatusResponce objInactivateTandCStatusResponce = new InactivateTandCStatusResponce();
                if (rowsEffected == 0)
                {
                    objInactivateTandCStatusResponce.Id = item.Id;
                    objInactivateTandCStatusResponce.Action = "Inactivation Unsucessfull";
                }
                else
                {
                    objInactivateTandCStatusResponce.Id = item.Id;
                    objInactivateTandCStatusResponce.Action = "Inactivation Sucessfull";
                }
                objInactivateTandCStatusResponceList.Ids.Add(objInactivateTandCStatusResponce);
            }
            return objInactivateTandCStatusResponceList;
        }

        async Task<int> OrganizationExists(int orgId)
        {

            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@id", orgId);
            parameterToGetPackageId.Add("@state", "A");
            string query = @"select id from master.organization where id=@id and state = @state";
            var data = await _dataAccess.ExecuteScalarAsync<int>
                             (query, parameterToGetPackageId);
            return data;
        }
        async Task<int> UserAccountExists(int accountId)
        {

            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@id", accountId);
            parameterToGetPackageId.Add("@state", "A");
            string query = @"select id from master.account where id=@id and state = @state";
            var data = await _dataAccess.ExecuteScalarAsync<int>
                             (query, parameterToGetPackageId);
            return data;
        }

        public async Task<bool> CheckUserAcceptedTermCondition(int AccountId, int OrganizationId)
        {
            try
            {
                var QueryStatement = @"select coalesce((select distinct termc.id
                                            from master.termsandcondition termc
                                            inner join master.accounttermsacondition acctermc
                                            on termc.id=acctermc.terms_and_condition_id
                                            where acctermc.account_id=@account_id
                                            and acctermc.organization_id=@organization_id
                                            and termc.state=@state), 0)";

                var parameter = new DynamicParameters();
                parameter.Add("@account_id", AccountId);
                parameter.Add("@organization_id", OrganizationId);
                parameter.Add("@state", Convert.ToChar(State.Active));
                int result = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Private methods
        private TermsAndConditions Map(dynamic record)
        {
            TermsAndConditions termsAndConditions = new TermsAndConditions();
            termsAndConditions.Id = record.id;
            termsAndConditions.Code = record.code;
            termsAndConditions.Version_no = record.version_no;
            if (record.description != null)
            {
                termsAndConditions.Description = record.description;
            }
            termsAndConditions.State = Convert.ToChar(record.state);
            termsAndConditions.StartDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.start_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            if (record.end_date != null)
            {
                termsAndConditions.EndDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.end_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            }
            if (record.created_at != null)
            {
                termsAndConditions.Created_At = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.created_at, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            }
            if (record.accepted_date != null)
            {
                termsAndConditions.Accepted_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.accepted_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            }
            termsAndConditions.FirstName = record.first_name;
            termsAndConditions.Lastname = record.last_name;
            return termsAndConditions;
        }

        #endregion

    }
}

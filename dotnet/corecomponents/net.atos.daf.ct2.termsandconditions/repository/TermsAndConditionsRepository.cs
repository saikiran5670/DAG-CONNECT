using net.atos.daf.ct2.termsandconditions.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using Dapper;
using System.Data;

namespace net.atos.daf.ct2.termsandconditions.repository
{
    public class TermsAndConditionsRepository : ITermsAndConditionsRepository
    {
        private readonly IDataAccess dataAccess;

        public TermsAndConditionsRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
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

                int accountTermsConditionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                accountTermsCondition.Id = accountTermsConditionID;

                if (accountTermsConditionID > 0)
                {
                    var UpdateVerNoStatement = @"UPDATE master.account
	                                             SET latest_accepted_tac_ver_no=@latest_accepted_tac_ver_no
	                                             WHERE id=@id
                                                 RETURNING latest_accepted_tac_ver_no";
                    parameter.Add("@latest_accepted_tac_ver_no", accountTermsCondition.version_no);
                    parameter.Add("@id", accountTermsCondition.Account_Id);
                    await dataAccess.ExecuteScalarAsync<string>(UpdateVerNoStatement, parameter);
                }

                return accountTermsCondition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TermsAndConditions>> GetAcceptedTermConditionByUser(int AccountId,int OrganizationId)
        {
            try
            {
                List<TermsAndConditions> Objtermcondn = new List<TermsAndConditions>();
                   var QueryStatement = @"SELECT terms.id
		                                ,terms.version_no
		                                ,terms.code
		                                ,terms.description
		                                ,terms.state
		                                ,terms.start_date
	                                FROM master.termsandcondition terms 
	                                Inner Join master.accounttermsacondition accterm
	                                on terms.id=accterm.terms_and_condition_id
	                                where 1=1";
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

                dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                TermsAndConditions termsAndConditions = new TermsAndConditions();
                foreach (dynamic record in result)
                {
                    Objtermcondn.Add(Map(record));
                }
                return Objtermcondn;
            }
            catch (Exception ex)
            {
                throw ex;
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
		                                       where account_id=account_id 
		                                       and organization_id=organization_id)                                   
                                    and lower(code) = (select SUBSTRING (lower(lang.code), 1,2)
					                                      from master.account acc 
					                                      inner join  master.accountpreference accpref
					                                      on acc.preference_id=accpref.id
					                                      inner join translation.language lang
					                                      on accpref.language_id=lang.id
					                                      and acc.id=account_id)
                                    and state='A'";
            var parameter = new DynamicParameters();
            parameter.Add("@account_id", AccountId);
            parameter.Add("@organization_id", OrganizationId);
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            TermsAndConditions termsAndConditions = new TermsAndConditions();
            foreach (dynamic record in result)
            {
                termsAndConditions = Map(record);
            }
            return termsAndConditions;
        }

        public async Task<List<string>> GetAllVersionNo()
        {

            var QueryStatement = @"select  id
                                    , version_no
                                    , code
                                    , description
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition";

            dynamic result = await dataAccess.QueryAsync<TermsAndConditions>(QueryStatement,null);
            List<string> ObjVersionList = new List<string>();
            foreach (dynamic record in result)
            {
                ObjVersionList.Add(record.version_no);
            }

            return ObjVersionList;
        }

        public async Task<List<TermsAndConditions>> GetTermConditionForVersionNo(string VersionNo, string LanguageCode)
        {
            List<TermsAndConditions> Objtermcondn = new List<TermsAndConditions>();
            var QueryStatement = @"select id
                                    , version_no
                                    , code
                                    , description
                                    , state
                                    , start_date
                                    , end_date
                                    FROM master.termsandcondition 
                                    where lower(version_no)=lower(trim(@version_no))";

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
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

            foreach (dynamic record in result)
            {
                Objtermcondn.Add(Map(record));
            }
            return Objtermcondn;
        }


        #region Private methods
        private TermsAndConditions Map(dynamic record)
        {
            TermsAndConditions termsAndConditions = new TermsAndConditions();
            termsAndConditions.Id = record.id;
            termsAndConditions.Code = record.code;
            termsAndConditions.version_no = record.version_no;
            termsAndConditions.Description = record.description;
            termsAndConditions.State = Convert.ToChar(record.state);
            termsAndConditions.StartDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.start_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            return termsAndConditions;
        }

        #endregion

    }
}

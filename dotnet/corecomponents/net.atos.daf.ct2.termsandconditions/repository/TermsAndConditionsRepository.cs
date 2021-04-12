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
                parameter.Add("@accepted_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                int accountTermsConditionID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                accountTermsCondition.Id = accountTermsConditionID;

                if (accountTermsConditionID > 0)
                {
                    var UpdateVerNoStatement = @"UPDATE master.account
	                                             SET latest_accepted_tac_ver_no=@latest_accepted_tac_ver_no
	                                             WHERE id=@account_id
                                                 RETURNING account_id";
                    parameter.Add("@latest_accepted_tac_ver_no", accountTermsCondition.version_no);
                    await dataAccess.ExecuteScalarAsync<int>(UpdateVerNoStatement, parameter);
                }

                return accountTermsCondition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TermsAndConditions> GetAcceptedTermConditionByUser(int AccountId)
        {
            try
            {
                
                var QueryStatement = @"SELECT terms.id
		                                ,terms.version_no
		                                ,terms.code
		                                ,terms.description
		                                ,terms.state
		                                ,terms.start_date
	                                FROM master.termsandcondition terms 
	                                Inner Join accounttermsacondition accterm
	                                on terms.id=accterm.terms_and_condition_id
	                                where accterm.account_id=@account_id";
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", AccountId);
                dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                TermsAndConditions termsAndConditions = new TermsAndConditions();
                foreach (dynamic record in result)
                {
                    termsAndConditions = Map(record);
                }
                return termsAndConditions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private TermsAndConditions Map(dynamic record)
        {
            TermsAndConditions termsAndConditions = new TermsAndConditions();
            termsAndConditions.Id = record.id;
            termsAndConditions.Code = record.code;
            termsAndConditions.version_no = record.version_no;
            termsAndConditions.Description = record.description;
            termsAndConditions.State = record.state;
            return termsAndConditions;
        }
    }
}

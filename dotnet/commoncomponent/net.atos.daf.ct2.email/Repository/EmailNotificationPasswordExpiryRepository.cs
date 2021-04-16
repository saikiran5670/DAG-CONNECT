using Dapper;
using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.email.Repository
{
    public class EmailNotificationPasswordExpiryRepository : IEmailNotificationPasswordExpiryRepository
    {
        private readonly IDataAccess dataAccess;
        public EmailNotificationPasswordExpiryRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        public async Task<IEnumerable<string>> GetEmailOfPasswordExpiry(int noOfDays)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@noOfDays", noOfDays);

                var query = @"Select acc.email from master.account acc inner join master.passwordpolicy pp on acc.id = pp.account_id where State= 'A' and EXTRACT(day FROM(now() - TO_TIMESTAMP(modified_at / 1000))) > @noOfDays";
                return await dataAccess.QueryAsync<string>(query, parameter);                
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}

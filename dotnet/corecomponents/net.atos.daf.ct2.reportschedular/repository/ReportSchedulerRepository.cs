using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public class ReportSchedulerRepository : IReportSchedulerRepository
    {
        private readonly IDataAccess _dataAccess;       

        public ReportSchedulerRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        #region Activate Report Scheduler

        #endregion

        #region Parameter Report Schedular 
        public async Task<IEnumerable<ReportType>> GetReportType(int accountid, int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct r.id as Id,r.name as ReportName
					                      FROM master.report r						                     
						                     INNER JOIN master.Feature f ON f.id = r.feature_id AND f.state = 'A' 
						                     INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_id = f.id
						                     INNER JOIN master.FeatureSet fset ON fsf.feature_set_id = fset.id AND fset.state = 'A'
						                     INNER JOIN master.Role ro ON ro.feature_set_id = fset.id AND ro.state = 'A'
						                     INNER JOIN master.AccountRole ar ON ro.id = ar.role_id and ar.organization_id = @organization_id
						                     INNER JOIN master.account acc ON acc.id = ar.account_id AND acc.state = 'A' AND acc.id = @account_id
											 INNER JOIN master.package pack ON pack.feature_set_id = fset.id AND fset.state = 'A' AND pack.state='A'
											 INNER JOIN master.subscription sub ON sub.package_id = pack.id AND sub.state = 'A' AND pack.state='A'
	 			                          WHERE acc.id = @account_id AND ar.Organization_id = @organization_id ; ";

                parameterType.Add("@organization_id", organizationid);
                parameterType.Add("@account_id", accountid);

                IEnumerable<ReportType> reporttype = await _dataAccess.QueryAsync<ReportType>(queryStatement, parameterType);
                return reporttype;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<ReceiptEmails>> GetRecipientsEmails(int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct 
                                        acc.email as Email from master.account acc
                                        Where organization_id= @organization_id
                                        AND state='A' AND email is not null;";
                parameterType.Add("@organization_id", organizationid);
                IEnumerable<ReceiptEmails> reporttype = await _dataAccess.QueryAsync<ReceiptEmails>(queryStatement, null);
                return reporttype;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DriverDetail>> GetDriverDetails(int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT
                                       id as Id
                                      ,dr.first_name || ' ' || dr.last_name AS DriverName                                         		  
                                      ,dr.driver_id as DriverId                                        		                                 		                                    		  
                                       FROM 
                                       master.driver dr
                                       Where dr.organization_id= @organization_id;";
                parameterType.Add("@organization_id", organizationid);
                IEnumerable<DriverDetail> driverdetails = await _dataAccess.QueryAsync<DriverDetail>(queryStatement, null);
                return driverdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}

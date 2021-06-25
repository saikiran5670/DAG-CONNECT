using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.email;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {
        public async Task<bool> SendReportEmail()
        {

            //var emailScheduleDetails = _reportSchedulerRepository.GetReportEmailDetails();
            //var emailid = "";
            //foreach (var item in emailScheduleDetails)
            //{
            //    var account = _accountManager.GetAccountByEmailId(emailid).Result;
            //    var isSuccess = await _accountManager.TriggerSendEmailRequest(account, EmailEventType.SendReport, null, EmailContentType.Html);
            //    return isSuccess;
            //}

            return false;

        }


        //private async Task<bool> TriggerSendEmailRequest(RecipientDetail recipientDetail, EmailEventType eventType, Guid? tokenSecret = null, EmailContentType contentType = EmailContentType.Html)
        //{
        //    var emailScheduleDetails = _reportSchedulerRepository.GetReportEmailDetails();
        //    var messageRequest = new MessageRequest();
        //    if (eventType == EmailEventType.PasswordExpiryNotification)
        //    {
        //        messageRequest.RemainingDaysToExpire = Convert.ToInt32(_configuration["RemainingDaysToExpire"]);
        //    }
        //    //messageRequest.AccountInfo = new AccountInfo
        //    //{
        //    //    FullName = account.FullName,
        //    //    OrganizationName = account.OrgName
        //    //};
        //    messageRequest.ToAddressList = new Dictionary<string, string>()
        //    {
        //        { recipientDetail.EmailId, null }
        //    };
        //    messageRequest.Configuration = _emailConfiguration;
        //    messageRequest.TokenSecret = tokenSecret;
        //    try
        //    {
        //        var languageCode = await GetLanguageCodePreference(recipientDetail.EmailId, recipientDetail.Organization_Id);
        //        var emailTemplate = await _translationManager.GetEmailTemplateTranslations(eventType, contentType, languageCode);

        //        return await EmailHelper.SendEmail(messageRequest, emailTemplate);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Reprt Component", "Report Manager", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "Trigger Email: " + ex.Message, 1, 2, account.EmailId);
        //        return false;
        //    }
        //}
        //public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        //{
        //    return await _reportSchedulerRepository.GetLanguageCodePreference(emailId.ToLower(), orgId);
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.entity;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace net.atos.daf.ct2.email
{
    public class EmailHelper
    {
        private static async Task<bool> SendEmail(MessageRequest messageRequest)
        {
            var apiKey = messageRequest.Configuration.ApiKey;
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();

            BuildMessageParameters(msg, messageRequest);

            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }

        private static void BuildMessageParameters(SendGridMessage msg, MessageRequest messageRequest)
        {
            msg.SetSubject(messageRequest.Subject);
            msg.SetFrom(new EmailAddress(messageRequest.Configuration.FromAddress, messageRequest.Configuration.FromName));

            if (messageRequest.Configuration.IsReplyAllowed)
                msg.SetReplyTo(new EmailAddress(messageRequest.Configuration.ReplyToAddress, messageRequest.Configuration.ReplyToAddress));

            SetSendToList(msg, messageRequest.ToAddressList, messageRequest.IsBcc);
            SetContent(msg, messageRequest.Content, messageRequest.ContentMimeType);
        }

        private static void SetSendToList(SendGridMessage msg, Dictionary<string, string> toAddressList, bool isBcc)
        {
            var recipients = new List<EmailAddress>();
            foreach (var keyValuePair in toAddressList)
            {
                recipients.Add(new EmailAddress(keyValuePair.Key, keyValuePair.Value));
            }
            if (isBcc)
            {
                msg.AddBccs(recipients);
            }
            else
            {
                msg.AddTos(recipients);
            }
        }

        private static void SetContent(SendGridMessage msg, string content, string mimeType)
        {
            msg.AddContent(mimeType, content);
        }

        public static async Task<bool> SendEmail(MessageRequest messageRequest, EmailTemplate emailTemplate)
        {
            var emailContent = string.Empty;
            try
            {
                Uri baseUrl = new Uri(messageRequest.Configuration.PortalUIBaseUrl);
                Uri logoUrl = new Uri(baseUrl, "assets/logo.png");

                var emailTemplateContent = GetEmailContent(emailTemplate);

                if (string.IsNullOrEmpty(emailTemplateContent))
                    return false;

                switch (emailTemplate.EventType)
                {
                    case EmailEventType.CreateAccount:
                        Uri setUrl = new Uri(baseUrl, $"#/auth/createpassword/{ messageRequest.TokenSecret }");

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.AccountInfo.FullName, messageRequest.AccountInfo.OrganizationName, setUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ResetPassword:
                        Uri resetUrl = new Uri(baseUrl, $"#/auth/resetpassword/{ messageRequest.TokenSecret }");
                        Uri resetInvalidateUrl = new Uri(baseUrl, $"#/auth/resetpasswordinvalidate/{ messageRequest.TokenSecret }");

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.AccountInfo.FullName, resetUrl.AbsoluteUri, resetInvalidateUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ChangeResetPasswordSuccess:
                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.AccountInfo.FullName, baseUrl.AbsoluteUri);
                        break;
                    case EmailEventType.PasswordExpiryNotification:
                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.AccountInfo.FullName, baseUrl.AbsoluteUri, messageRequest.ToAddressList.First().Key, DateTime.Now.AddDays(messageRequest.RemainingDaysToExpire).ToString("dd-MMM-yyyy"));
                        break;
                    case EmailEventType.ScheduledReportEmail:
                        emailContent = GetReportEmailContent(emailTemplateContent, baseUrl, messageRequest);
                        emailTemplate.Description = emailContent;
                        emailContent = GetEmailContent(emailTemplate);
                        break;
                }

                if (emailTemplate.TemplateLabels.Count() > 0)
                {
                    var translationLabel = emailTemplate.TemplateLabels.Where(x => x.LabelKey.EndsWith("_Subject")).FirstOrDefault();
                    messageRequest.Subject = translationLabel == null ? " " : translationLabel.TranslatedValue;
                }

                messageRequest.Content = emailContent;
                messageRequest.ContentMimeType = emailTemplate.ContentType == (char)EmailContentType.Html ? MimeType.Html : MimeType.Text;
                return await SendEmail(messageRequest); //wrap this function usder while loop with retry condition
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GetReportEmailContent(string emailTemplate, Uri baseUrl, MessageRequest messageRequest)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            var downloadUrl = "reportscheduler/download?Token={0}";
            string btnLabel = "\"button\"";
            string lblBlank = "\"_blank\"";
            string lblPlaceholder = "\"{0}\"";
            var urldown = @"<a class=" + btnLabel + " href=" + lblPlaceholder + " target=" + lblBlank + ">[lblDownloadReportButton]</a>";

            Uri downloadReportUrl = null;
            foreach (var token in messageRequest.ReportTokens)
            {
                downloadReportUrl = new Uri(baseUrl, string.Format(downloadUrl, token));
                urldown += "<br/>";
            }

            string urlplace = string.Format(urldown, downloadReportUrl);
            builder.AppendFormat(emailTemplate, messageRequest.AccountInfo.FullName, urlplace);
            return builder.ToString();
        }

        public static string GetEmailContent(EmailTemplate emailTemplate)
        {
            var replacedContent = emailTemplate.Description;
            Regex regex = new Regex(@"\[(.*?)\]");

            var dictLabels = emailTemplate.TemplateLabels.ToDictionary(x => x.LabelKey, x => x.TranslatedValue);

            foreach (Match match in regex.Matches(emailTemplate.Description))
            {
                if (dictLabels.ContainsKey(match.Groups[1].Value))
                    replacedContent = replacedContent.Replace(match.Value, dictLabels[match.Groups[1].Value]);
            }
            return replacedContent;
        }
    }
}

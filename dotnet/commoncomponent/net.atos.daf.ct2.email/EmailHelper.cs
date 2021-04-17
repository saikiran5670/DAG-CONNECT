﻿using System;
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

            if(messageRequest.Configuration.IsReplyAllowed)
                msg.SetReplyTo(new EmailAddress(messageRequest.Configuration.ReplyToAddress, messageRequest.Configuration.ReplyToAddress));

            SetSendToList(msg, messageRequest.ToAddressList);
            SetContent(msg, messageRequest.Content, messageRequest.ContentMimeType);
        }

        private static void SetSendToList(SendGridMessage msg, Dictionary<string, string> toAddressList)
        {
            var recipients = new List<EmailAddress>();
            foreach (var keyValuePair in toAddressList)
            {
                recipients.Add(new EmailAddress(keyValuePair.Key, keyValuePair.Value));
            }
            msg.AddTos(recipients);
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

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.accountInfo.FullName, messageRequest.accountInfo.OrganizationName, setUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ResetPassword:
                        Uri resetUrl = new Uri(baseUrl, $"#/auth/resetpassword/{ messageRequest.TokenSecret }");
                        Uri resetInvalidateUrl = new Uri(baseUrl, $"#/auth/resetpasswordinvalidate/{ messageRequest.TokenSecret }");

                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.accountInfo.FullName, resetUrl.AbsoluteUri, resetInvalidateUrl.AbsoluteUri);
                        break;
                    case EmailEventType.ChangeResetPasswordSuccess:
                        emailContent = string.Format(emailTemplateContent, logoUrl.AbsoluteUri, messageRequest.accountInfo.FullName, baseUrl.AbsoluteUri);
                        break;
                    default:
                        messageRequest.Subject = string.Empty;
                        break;
                }

                messageRequest.Subject = emailTemplate.TemplateLabels.Where(x => x.LabelKey.EndsWith("_Subject")).First().TranslatedValue;
                messageRequest.Content = emailContent;
                messageRequest.ContentMimeType = emailTemplate.ContentType == (char)EmailContentType.Html ? MimeType.Html : MimeType.Text;
                
                return await SendEmail(messageRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetEmailContent(EmailTemplate emailTemplate)
        {
            var replacedContent = emailTemplate.Description;
            Regex regex = new Regex(@"\[(.*?)\]");

            var dictLabels = emailTemplate.TemplateLabels.ToDictionary(x => x.LabelKey, x => x.TranslatedValue);

            foreach (Match match in regex.Matches(emailTemplate.Description))
            {
                if(dictLabels.ContainsKey(match.Groups[1].Value))
                    replacedContent = replacedContent.Replace(match.Value, dictLabels[match.Groups[1].Value]);
            }
            return replacedContent;
        }
    }
}

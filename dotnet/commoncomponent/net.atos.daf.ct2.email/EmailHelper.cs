using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
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
        public static async Task<bool> SendEmail(MessageRequest messageRequest)
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

        public static string GetEmailContent(EmailTemplate emailTemplate)
        {
            //ResourceManager rs = new ResourceManager("net.atos.daf.ct2.email.Templates", Assembly.GetExecutingAssembly());
            //return rs.GetString(eventType.ToString(), CultureInfo.CurrentCulture);
            var replacedContent = emailTemplate.Description;
            Regex regex = new Regex(@"\[(.*?)\]");

            foreach (Match match in regex.Matches(emailTemplate.Description))
            {
                replacedContent = replacedContent.Replace(match.Value, 
                    emailTemplate.TemplateLabels
                    .Where(x => x.LabelKey.Equals(match.Groups[1].Value)).FirstOrDefault()?.TranslatedValue);
            }
            return replacedContent;
        }
    }
}

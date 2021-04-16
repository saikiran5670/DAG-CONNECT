using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
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
            if (messageRequest.ToAddressList.ContainsKey("him.waghulde@atos.net"))
                throw new Exception("test error");
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

        public static string GetTemplateHtmlString(EmailTemplateType templateType)
        {
            ResourceManager rs = new ResourceManager("net.atos.daf.ct2.email.Templates", Assembly.GetExecutingAssembly());
            return rs.GetString(templateType.ToString(), CultureInfo.CurrentCulture);
        }
    }
}

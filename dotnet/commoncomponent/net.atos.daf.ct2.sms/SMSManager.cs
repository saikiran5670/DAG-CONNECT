using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.sms.entity;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace net.atos.daf.ct2.sms
{
    public class SMSManager : ISMSManager
    {
        private readonly IConfiguration _configuration;
        private readonly SMSConfiguration _smsConfiguration;
        public SMSManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _smsConfiguration = new SMSConfiguration();
            _configuration.GetSection("SMSConfiguration").Bind(_smsConfiguration);
        }
        public async Task<string> SendSMS(SMS smsdetail)
        {
            try
            {
                var accountSid = _smsConfiguration.AccountSid; //"AC652c7ba994c2b9fa3fb9667d1516257e";
                var authToken = _smsConfiguration.AuthToken;//"9e5f38507b21bcc9cf8bc856488a5e13";
                TwilioClient.Init(accountSid, authToken);
                var message = MessageResource.Create(
                    body: smsdetail.Body,
                    from: new Twilio.Types.PhoneNumber(_smsConfiguration.FromPhoneNumber),//Twilio.Types.PhoneNumber("+16308844316"),
                    to: new Twilio.Types.PhoneNumber(smsdetail.ToPhoneNumber)
                //messagingServiceSid: "MG5ad5618d31b2a9100761761fe1ed1755" Message sender id code
                );
                return message.Status.ToString();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<string> GetSMSBySid(string sid)
        {
            var accountSid = _smsConfiguration.AccountSid; //"AC652c7ba994c2b9fa3fb9667d1516257e";
            var authToken = _smsConfiguration.AuthToken;//"9e5f38507b21bcc9cf8bc856488a5e13";
            TwilioClient.Init(accountSid, authToken);
            var message = MessageResource.Fetch(pathSid: sid);
            return message.Status.ToString();
        }
    }
}

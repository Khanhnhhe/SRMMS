using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SRMMS.SMS
{
    public class TwilioService : ITwilioService
    {
        private readonly string _twilioPhoneNumber;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;

        public TwilioService(IConfiguration configuration)
        {
            _twilioAccountSid = configuration["Twilio:AccountSid"];
            _twilioAuthToken = configuration["Twilio:AuthToken"];
            _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];

            if (string.IsNullOrEmpty(_twilioAccountSid) || string.IsNullOrEmpty(_twilioAuthToken) || string.IsNullOrEmpty(_twilioPhoneNumber))
            {
                throw new ArgumentException("Twilio configuration is missing or incomplete.");
            }
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.");
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.");

            // Format phone number if necessary
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = $"+84{phoneNumber.Substring(1)}";
            }

            try
            {
                TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                if (messageResource.ErrorCode != null)
                {
                    throw new Exception($"Twilio Error: {messageResource.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                throw;
            }
        }
    }
}


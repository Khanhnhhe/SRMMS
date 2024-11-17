using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using SRMMS.SMS;

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
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

        var messageResource = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(_twilioPhoneNumber),
            to: new PhoneNumber(phoneNumber)
        );
    }
}

namespace SRMMS.SMS
{
    public interface ITwilioService
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }
}

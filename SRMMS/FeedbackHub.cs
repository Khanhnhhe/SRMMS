using Microsoft.AspNetCore.SignalR;

public class FeedbackHub : Hub
{
    public async Task NotifyFeedbackUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveFeedbackUpdate", message);
    }
}

using Microsoft.AspNetCore.SignalR;
using SRMMS.Models;

namespace SRMMS.Hubs
{
    public class FeedbackHub : Hub
    {
        public async Task SendBookingUpdate(List<Feedback> feedbacks)
        {

            await Clients.All.SendAsync("ReceiveFeedbackUpdate", feedbacks);
        }
    }
}

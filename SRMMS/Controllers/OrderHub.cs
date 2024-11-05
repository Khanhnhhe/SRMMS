using System;
using Microsoft.AspNetCore.SignalR;

namespace SRMMS.Controllers
{
	public class OrderHub : Hub
	{
        public async Task SendOrderUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", message);
        }
    }
}


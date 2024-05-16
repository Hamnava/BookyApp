using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class AvailabilityHub : Hub
    {
        public async Task CheckAvailability(bool message)
        {
            await Clients.All.SendAsync("ReceiveAvailabilityUpdate", message);
        }
    }
}

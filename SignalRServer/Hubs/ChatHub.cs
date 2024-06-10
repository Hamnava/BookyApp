using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, ClientInfo> ClientsInfo = new();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            if (ClientsInfo.TryGetValue(connectionId, out var clientInfo))
            {
                clientInfo.IsOnline = false;
                await Clients.All.SendAsync("UpdateClientList", ClientsInfo.Values);
                await Clients.All.SendAsync("ClientStatusChanged", clientInfo.UniqueClientId, "offline");

            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterClient(string uniqueClientId, string deviceId, string clientType)
        {
            var connectionId = Context.ConnectionId;

            // Add new client
            var clientInfo = new ClientInfo
            {
                ConnectionId = connectionId,
                UniqueClientId = uniqueClientId,
                DeviceId = deviceId,
                ClientType = clientType,
                IsOnline = true
            };

            ClientsInfo[connectionId] = clientInfo;

            await Clients.All.SendAsync("UpdateClientList", ClientsInfo.Values);
            await Clients.All.SendAsync("ClientStatusChanged", clientInfo.ConnectionId, "online");
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToClient(string connectionId, string user, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
        }
    }

}



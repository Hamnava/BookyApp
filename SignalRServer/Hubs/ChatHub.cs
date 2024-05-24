using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, ClientInfo> ClientsInfo = new();

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            ClientsInfo[connectionId] = new ClientInfo
            {
                ConnectionId = connectionId,
                UniqueId = "Unknown",
                DeviceId = "Unknown",
                ClientType = "Unknown"
            };

            await Clients.All.SendAsync("UpdateClientList", ClientsInfo.Values);
            await Clients.All.SendAsync("ClientStatusChanged", connectionId, "online");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //ClientsInfo.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UpdateClientList", ClientsInfo.Values);
            await Clients.All.SendAsync("ClientStatusChanged", Context.ConnectionId, "offline");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterClient(string uniqueId, string deviceId, string clientType)
        {
            var connectionId = Context.ConnectionId;
            if (ClientsInfo.ContainsKey(connectionId))
            {
                ClientsInfo[connectionId].UniqueId = uniqueId;
                ClientsInfo[connectionId].DeviceId = deviceId;
                ClientsInfo[connectionId].ClientType = clientType;

                await Clients.All.SendAsync("UpdateClientList", ClientsInfo.Values);
            }
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




    public class ClientInfo
    {
        public string ConnectionId { get; set; }
        public string UniqueId { get; set; }
        public string DeviceId { get; set; }
        public string ClientType { get; set; } // e.g., WPF or Worker Service
    }

}

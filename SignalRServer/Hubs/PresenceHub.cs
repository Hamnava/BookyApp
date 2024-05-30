using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using System.Collections.Concurrent;

namespace SignalRServer.Hubs
{
    public class PresenceHub : Hub
    {
        private static readonly ConcurrentDictionary<string, PresenseInfo> ClientsInfo = new();
        private static readonly ConcurrentDictionary<string, string> ClientIdToConnectionId = new(); // Maps UniqueClientId to ConnectionId

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task RegisterClient(string uniqueClientId, string deviceId, bool isWorkerService)
        {
            var connectionId = Context.ConnectionId;

            // Check if the client already exists
            if (ClientIdToConnectionId.TryGetValue(uniqueClientId, out var oldConnectionId))
            {
                // Update the existing client's information
                if (ClientsInfo.TryGetValue(oldConnectionId, out var clientInfo))
                {
                    clientInfo.ConnectionId = connectionId;
                    clientInfo.DeviceId = deviceId;
                    clientInfo.IsWorkerService = isWorkerService;
                    clientInfo.IsOnline = true;

                    // Remove the old mapping
                    ClientsInfo.TryRemove(oldConnectionId, out _);
                    ClientIdToConnectionId.TryRemove(uniqueClientId, out _);

                    // Add the new mapping
                    ClientsInfo[connectionId] = clientInfo;
                    ClientIdToConnectionId[uniqueClientId] = connectionId;
                }
            }
            else
            {
                // Add new client
                var clientInfo = new PresenseInfo
                {
                    ConnectionId = connectionId,
                    UniqueClientId = uniqueClientId,
                    DeviceId = deviceId,
                    IsWorkerService = isWorkerService,
                    IsOnline = true
                };

                ClientsInfo[connectionId] = clientInfo;
                ClientIdToConnectionId[uniqueClientId] = connectionId;
            }

            // Check if this connected client is a worker service
            if (isWorkerService)
            {
                // Find any WPF client with the same DeviceId
                foreach (var kvp in ClientsInfo)
                {
                    var client = kvp.Value;
                    if (client.DeviceId == deviceId && !client.IsWorkerService)
                    {
                        await Clients.Client(client.ConnectionId).SendAsync("WorkerServiceStatus", true);
                    }
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            if (ClientsInfo.TryRemove(connectionId, out var clientInfo))
            {
                ClientIdToConnectionId.TryRemove(clientInfo.UniqueClientId, out _);

                if (clientInfo.IsWorkerService)
                {
                    // Find any WPF client with the same DeviceId
                    foreach (var kvp in ClientsInfo)
                    {
                        var client = kvp.Value;
                        if (client.DeviceId == clientInfo.DeviceId && !client.IsWorkerService)
                        {
                            await Clients.Client(client.ConnectionId).SendAsync("WorkerServiceStatus", false);
                        }
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetWorkerServiceStatus(string deviceId)
        {
            var isWorkerServiceOnline = ClientsInfo.Values.Any(client => client.DeviceId == deviceId && client.IsWorkerService && client.IsOnline);
            await Clients.Caller.SendAsync("WorkerServiceStatus", isWorkerServiceOnline);
        }

        public async Task SendHelloMessage(string message)
        {
            Console.WriteLine($"Message from {Context.ConnectionId}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }
    }


}





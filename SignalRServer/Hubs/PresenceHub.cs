using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class PresenceHub : Hub
    {
        public static HashSet<string> WorkerServiceConnections = new HashSet<string>();
        public static bool IsWorkerServiceConnected => WorkerServiceConnections.Count > 0;

        public override Task OnConnectedAsync()
        {
            var isWorkerService = Context.GetHttpContext().Request.Query["isWorkerService"];
            if (isWorkerService == "true")
            {
                Console.WriteLine($"Worker service connected with Id: {Context.ConnectionId}");
                WorkerServiceConnections.Add(Context.ConnectionId);
                Clients.All.SendAsync("WorkerServiceStatus", true);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (WorkerServiceConnections.Contains(Context.ConnectionId))
            {
                Console.WriteLine($"Worker service disconnected with Id: {Context.ConnectionId}");

                WorkerServiceConnections.Remove(Context.ConnectionId);
                Clients.All.SendAsync("WorkerServiceStatus", false);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task GetWorkerServiceStatus()
        {
            return Clients.Caller.SendAsync("WorkerServiceStatus", IsWorkerServiceConnected);
        }

        public Task SendMessage(string message)
        {
            Console.WriteLine($"Message from {Context.ConnectionId}: {message}");
            // You can customize this to send the message to specific clients or groups
            return Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }
    }


}

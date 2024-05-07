
using Microsoft.AspNetCore.SignalR.Client;

namespace WorkerService
{
    public class SignalRWorker : BackgroundService
    {
        private readonly HubConnection _connection;

        public SignalRWorker()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/chatHub")
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_connection.State == HubConnectionState.Disconnected)
                    {
                        await _connection.StartAsync();
                    }

                    await Task.Delay(10000, stoppingToken); // Delay to simulate work
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }
    }
}

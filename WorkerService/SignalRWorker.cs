
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Pipes;

namespace WorkerService
{
    public class SignalRWorker : BackgroundService
    {
        private readonly HubConnection _connection;
        private readonly ILogger<SignalRWorker> _logger;
        public SignalRWorker(ILogger<SignalRWorker> logger)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/chatHub")
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });

            _logger = logger;
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

                using (var server = new NamedPipeServerStream("SendMessageToSignalR", PipeDirection.In))
                {
                    Console.WriteLine("Waiting for connection...");
                    await server.WaitForConnectionAsync(stoppingToken);

                    using (var reader = new StreamReader(server))
                    {
                        string message = await reader.ReadToEndAsync();

                        await _connection.InvokeAsync("SendMessage", "Worker Service User", message);

                        _logger.LogInformation(message);
                    }
                }
            }
        }
    }
}

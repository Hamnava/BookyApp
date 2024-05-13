
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Pipes;

namespace WorkerService
{
    public class SignalRWorker : BackgroundService
    {
        private readonly HubConnection _connection;
        private readonly ILogger<SignalRWorker> _logger;
        private readonly NamedPipeService _pipeService;


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
            _pipeService = new NamedPipeService("SendMessageToSignalR", PipeDirection.In, logger);
        }


        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }


            await _pipeService.RunPipeServer(stoppingToken, _connection);
           
        }
    }
}

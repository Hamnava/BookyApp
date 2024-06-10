
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.IO.Pipes;
using WorkerService.Helpers;

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
            _pipeService = new NamedPipeService(PipeNames.SendMessageToSignalR, PipeDirection.In, logger);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync();

                    // Get the unique device ID
                    string deviceId = DeviceInfoHelper.GetDeviceId();
                    string uniqueClientId = Guid.NewGuid().ToString();
                    string clientType = "Worker Service";

                    // Send registration information
                    await _connection.InvokeAsync("RegisterClient", uniqueClientId, deviceId, clientType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            // Subscribe to the MessageReceived event
            _pipeService.MessageReceived += async (message) =>
            {
                try
                {
                    // Send the received message to SignalR
                    await _connection.InvokeAsync("SendMessage", "Worker Service", JsonConvert.SerializeObject(message));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception when sending message to SignalR: {ex.Message}");
                }
            };

            await _pipeService.RunPipeServer(stoppingToken);
        }
    }

}



using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO.Pipes;
using WorkerService.Helpers;
using WorkerService.Models;
using WorkerService.Services;

namespace WorkerService.Workers
{
    public class SignalRWorker : BackgroundService
    {
        private readonly SignalRService _signalRService;
        private readonly ILogger<SignalRWorker> _logger;
        private readonly NamedPipeService _pipeService;
        private readonly SignalRConfiguration _signalRConfiguration;

        public SignalRWorker(ILogger<SignalRWorker> logger, IOptions<SignalRConfiguration> signalRConfigurationOptions)
        {
            _signalRConfiguration = signalRConfigurationOptions.Value;

            _logger = logger;

            var settings = _signalRConfiguration.SignalR;
            _signalRService = new SignalRService(settings);

            _pipeService = new NamedPipeService(PipeNames.SendMessageToSignalR, PipeDirection.In, logger);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string deviceId = DeviceInfoHelper.GetDeviceId();
                string uniqueClientId = Guid.NewGuid().ToString();
                string clientType = "Worker Service";

                await _signalRService.InitializeChatConnection(uniqueClientId, deviceId, clientType);
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
                    await _signalRService.SendMessage("Worker Service", JsonConvert.SerializeObject(message));
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



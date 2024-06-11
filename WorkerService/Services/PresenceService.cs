using Microsoft.Extensions.Options;
using WorkerService.Models;

namespace WorkerService.Services
{
    public class PresenceService : BackgroundService
    {
        private readonly SignalRService _signalRService;
        private readonly SignalRConfiguration _signalRConfiguration;
        public PresenceService(IOptions<SignalRConfiguration> signalRConfigurationOptions)
        {
            _signalRConfiguration = signalRConfigurationOptions.Value;

            var settings = _signalRConfiguration.SignalR;
            _signalRService = new SignalRService(settings);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string deviceId = DeviceInfoHelper.GetDeviceId();
                string uniqueClientId = Guid.NewGuid().ToString();
                string clientType = "Worker Service";

                await _signalRService.InitializePresenceConnection(uniqueClientId, deviceId);

                // after one minute crush the project
                await Task.Delay(60000).ContinueWith(_ => SimulateCrash());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private void SimulateCrash()
        {
            Console.WriteLine("Simulating crash...");
            // stop the application
            Environment.Exit(1);
        }
    }
}

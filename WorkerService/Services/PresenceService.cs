using Microsoft.AspNetCore.SignalR.Client;
using WorkerService.Helper;

namespace WorkerService.Services
{
    public class PresenceService : BackgroundService
    {
        private HubConnection _connection;

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/presenceHub")
                .Build();

                _connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                };

                await _connection.StartAsync();
                await _connection.SendAsync("SendHelloMessage", "Worker service is running");

                // Get the unique device ID
                string deviceId = DeviceInfoHelper.GetDeviceId();
                string uniqueClientId = ClientIdHelper.GetOrCreateClientId();

                // Send registration information
                await _connection.InvokeAsync("RegisterClient", uniqueClientId, deviceId, true);

                //await Task.Delay(30000).ContinueWith(_ => SimulateCrash());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }


        }

        private void SimulateCrash()
        {
            Console.WriteLine("Simulating crash...");
            // Stop the timer to simulate the service crashing

            Environment.Exit(1);
        }
    }
}

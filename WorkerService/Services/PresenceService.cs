using Microsoft.AspNetCore.SignalR.Client;

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
                .WithUrl("https://localhost:7031/presenceHub?isWorkerService=true")
                .Build();

                _connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                };

                await _connection.StartAsync();
                await _connection.SendAsync("SendHelloMessage", "Worker service is running");

                await Task.Delay(30000).ContinueWith(_ => SimulateCrash());
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

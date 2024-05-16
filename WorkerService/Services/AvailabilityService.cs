using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Services
{
    public class AvailabilityService : BackgroundService
    {
        private readonly HubConnection _connection;
        public AvailabilityService(ILogger<SignalRWorker> logger)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/availabilityHub")
                .Build();

            _connection.On<bool>("ReceiveAvailabilityUpdate", (message) =>
            {
                Console.WriteLine($"Available:  {message}");
            });

           
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



        }
    }
}

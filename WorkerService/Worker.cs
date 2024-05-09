using System.IO.Pipes;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var server = new NamedPipeServerStream("PipesOfPiece", PipeDirection.In))
                {
                    Console.WriteLine("Waiting for connection...");
                    await server.WaitForConnectionAsync(stoppingToken);

                    using (var reader = new StreamReader(server))
                    {
                        string message = await reader.ReadToEndAsync();
                        Console.WriteLine($"Received: {message}");

                        _logger.LogInformation(message);
                    }
                }
            }
        }
    }
}

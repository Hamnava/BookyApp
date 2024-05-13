using Newtonsoft.Json;
using System.IO.Pipes;
using WorkerService.Models;

namespace WorkerService
{
    public class SecondWorker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly NamedPipeService _pipeService;

        public SecondWorker(ILogger<Worker> logger)
        {
            _logger = logger;
            _pipeService = new NamedPipeService("SendReceive", PipeDirection.InOut, logger);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _pipeService.RunPipeServer(stoppingToken);
        }
    }

   
}

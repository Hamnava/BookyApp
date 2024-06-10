using System.IO.Pipes;
using WorkerService.Helpers;

namespace WorkerService
{
    public class OnlyRecieveWorker : BackgroundService
    {
        private readonly ILogger<OnlyRecieveWorker> _logger;
        private readonly NamedPipeService _pipeService;

        public OnlyRecieveWorker(ILogger<OnlyRecieveWorker> logger)
        {
            _logger = logger;
            _pipeService = new NamedPipeService(PipeNames.OnlySend, PipeDirection.In, logger);
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _pipeService.RunPipeServer(stoppingToken);
        }
    }
}

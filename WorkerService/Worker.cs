using System.IO.Pipes;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly NamedPipeService _pipeService;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _pipeService = new NamedPipeService("PipesOfPiece", PipeDirection.In, logger);
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _pipeService.RunPipeServer(stoppingToken);
        }
    }
}

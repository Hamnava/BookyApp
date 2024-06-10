using System.IO.Pipes;
using WorkerService.Helpers;

namespace WorkerService
{
    public class SendReceiveWorker : BackgroundService
    {

        private readonly ILogger<OnlyRecieveWorker> _logger;
        private readonly NamedPipeService _pipeService;

        public SendReceiveWorker(ILogger<OnlyRecieveWorker> logger)
        {
            _logger = logger;
            _pipeService = new NamedPipeService(PipeNames.SendReceive, PipeDirection.InOut, logger);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _pipeService.RunPipeServer(stoppingToken);
        }
    }


}

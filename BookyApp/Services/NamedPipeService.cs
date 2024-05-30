using System.IO.Pipes;

namespace BookyApp.Services
{
    public class NamedPipeService
    {
        private readonly NamedPipeClientService _pipeClientService;

        public NamedPipeService()
        {
            _pipeClientService = new NamedPipeClientService();
        }

        public async Task SendMessageAsync(string pipeName, PipeDirection direction, object message)
        {
            await _pipeClientService.SendMessageAsync(pipeName, direction, message);
        }
    }

}

using Newtonsoft.Json;
using System.IO.Pipes;
using WorkerService.Models;

public class NamedPipeService
{
    private readonly string _pipeName;
    private readonly PipeDirection _pipeDirection;
    private readonly ILogger _logger;

    public NamedPipeService(string pipeName, PipeDirection pipeDirection, ILogger logger)
    {
        _pipeName = pipeName;
        _pipeDirection = pipeDirection;
        _logger = logger;
    }

    public async Task RunPipeServer(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var server = new NamedPipeServerStream(_pipeName, _pipeDirection, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                _logger.LogInformation("Waiting for connection...");
                await server.WaitForConnectionAsync(stoppingToken);

                if (_pipeDirection == PipeDirection.In || _pipeDirection == PipeDirection.InOut)
                {
                    await HandleIncomingMessages(server);
                }
            }
        }
    }

    private async Task HandleIncomingMessages(NamedPipeServerStream server)
    {
        using (var reader = new StreamReader(server))
        {
            string message = await reader.ReadLineAsync();
            _logger.LogInformation($"Received: {message}");
            Console.WriteLine($"Received: {message}");

            if (_pipeDirection == PipeDirection.InOut)
            {
                using (var writer = new StreamWriter(server))
                {
                    var data = JsonConvert.DeserializeObject<HelloWithObject>(message);
                    string response = $"Received: Name={data.Name}, Family={data.Family}, Age={data.Age}";

                    Console.WriteLine(response);

                    await writer.WriteLineAsync(response);
                    await writer.FlushAsync();

                    _logger.LogInformation(response);
                }
            }
        }
    }
}

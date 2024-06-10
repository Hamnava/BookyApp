using Newtonsoft.Json;
using System.IO.Pipes;

public class NamedPipeService
{
    private readonly string _pipeName;
    private readonly PipeDirection _pipeDirection;
    private readonly ILogger _logger;

    public event Func<object, Task> MessageReceived;

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
                    var receivedObject = await ReceiveMessageAsync(server);
                    _logger.LogInformation($"Received: {JsonConvert.SerializeObject(receivedObject)}");

                    // Optional: Handle the received object as needed
                    // e.g., Change some data or update DB records;

                    // Notify the worker service
                    if (MessageReceived != null)
                    {
                        await MessageReceived.Invoke(receivedObject);
                    }

                    if (_pipeDirection == PipeDirection.InOut)
                    {
                        // Example of sending a response back
                        var responseObject = new { Status = "Received", Data = receivedObject };
                        await SendMessageAsync(server, responseObject);
                    }
                }
            }
        }
    }

    private async Task<object> ReceiveMessageAsync(NamedPipeServerStream server)
    {
        using (var reader = new StreamReader(server, leaveOpen: true))
        {
            string message = await reader.ReadLineAsync();
            var receivedObject = JsonConvert.DeserializeObject<object>(message);
            return receivedObject;
        }
    }

    public async Task SendMessageAsync(NamedPipeServerStream server, object message)
    {
        if (server.IsConnected)
        {
            using (var writer = new StreamWriter(server) { AutoFlush = true })
            {
                string jsonMessage = JsonConvert.SerializeObject(message);
                await writer.WriteLineAsync(jsonMessage);
                await writer.FlushAsync();
            }
        }
    }
}




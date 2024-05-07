using Newtonsoft.Json;
using System.IO.Pipes;

namespace WorkerService
{
    public class SecondWorker : BackgroundService
    {


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var server = new NamedPipeServerStream("SendReceive", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                {
                    Console.WriteLine("Waiting for connection...");
                    await server.WaitForConnectionAsync(stoppingToken);

                    using (var reader = new StreamReader(server))
                    using (var writer = new StreamWriter(server))
                    {
                        string receivedJson = await reader.ReadLineAsync();

                        // Deserialize the JSON string back into an object
                        var data = JsonConvert.DeserializeObject<HelloWithObject>(receivedJson);
                        Console.WriteLine($"Received: Name={data.Name}, Family={data.Family}, Age={data.Age}");


                        // Send a response back to the client
                        await writer.WriteLineAsync($"Received: Name={data.Name}, Family={data.Family}, Age={data.Age}");
                        await writer.FlushAsync();
                    }
                }
            }


        }
    }

    public class HelloWithObject
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public int Age { get; set; }
    }
}

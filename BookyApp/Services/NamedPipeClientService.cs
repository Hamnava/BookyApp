using System.IO.Pipes;
using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;
using BookyApp.Helpers;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

public class NamedPipeClientService
{
 


   
    public async Task SendMessageAsync(string pipeName, PipeDirection direction, object dataToSend)
    {
        try
        {
            using (var client = new NamedPipeClientStream(".", pipeName, direction, PipeOptions.Asynchronous))
            {
                await client.ConnectAsync(5000);
              

                if (direction == PipeDirection.Out || direction == PipeDirection.InOut)
                {
                    using (var writer = new StreamWriter(client, leaveOpen: direction == PipeDirection.InOut))
                    {
                        string jsonData = JsonConvert.SerializeObject(dataToSend);
                        await writer.WriteLineAsync(jsonData);
                        await writer.FlushAsync();
                    }
                }

                if (direction == PipeDirection.In || direction == PipeDirection.InOut)
                {
                    using (var reader = new StreamReader(client))
                    {
                        string response = await reader.ReadLineAsync();
                        MessageBox.Show($"Response received: {response}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
           
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}

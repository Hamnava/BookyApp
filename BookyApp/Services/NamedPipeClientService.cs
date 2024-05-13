using System.IO.Pipes;
using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;
using BookyApp.Helpers;

public class NamedPipeClientService
{
    private readonly ConnectionStatusManager _statusManager;

    public NamedPipeClientService(ConnectionStatusManager statusManager)
    {
        _statusManager = statusManager;
    }

    public async Task SendMessageAsync(string pipeName, PipeDirection direction, object dataToSend, string? statusKey = null)
    {
        try
        {
            using (var client = new NamedPipeClientStream(".", pipeName, direction, PipeOptions.Asynchronous))
            {
                await client.ConnectAsync(5000);
                if(statusKey != null) 
                _statusManager.SetStatus(statusKey, Brushes.Green);

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
            if (statusKey != null)
                _statusManager.SetStatus(statusKey, Brushes.Red);
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}

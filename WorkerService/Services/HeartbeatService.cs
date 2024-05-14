using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WorkerService.Models;

public class HeartbeatSender : IHostedService
{
    private Timer _timer;
    private readonly int _port = 12345;
    private bool _simulateCrash = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SendHeartbeat, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        // Schedule a "crash" in 30 seconds
        if (_simulateCrash)
        {
            Task.Delay(30000).ContinueWith(_ => SimulateCrash());
        }
        return Task.CompletedTask;
    }

    private void SimulateCrash()
    {
        Console.WriteLine("Simulating crash...");
        // Stop the timer to simulate the service crashing

        var hello = new HelloWithObject();
        hello.Name.ToString();
    }

    private void SendHeartbeat(object state)
    {
        try
        {
            using (var client = new TcpClient("127.0.0.1", _port))
            {
                var message = Encoding.ASCII.GetBytes("Heartbeat");
                client.GetStream().Write(message, 0, message.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending heartbeat: " + ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

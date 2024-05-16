
using BookyApp.Helpers;
using BookyApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int _port = 12345;
        private readonly TcpListener _listener;
        private DateTime _lastHeartbeatReceived = DateTime.UtcNow;
        private readonly TimeSpan _heartbeatTimeout = TimeSpan.FromSeconds(10);

        private HubConnection _connection;
        private ConnectionStatusManager _statusManager = new ConnectionStatusManager();
        private NamedPipeClientService _pipeClientService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
            DataContext = _statusManager;
            _pipeClientService = new NamedPipeClientService();

            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            ListenForHeartbeats();

            CheckForHeartbeatTimeout();
        }

        private async void ListenForHeartbeats()
        {
            while (true)
            {
                try
                {
                    // Asynchronously accept a client connection
                    var client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected...");
                    HandleClient(client);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    Console.WriteLine("Error accepting client: " + ex.Message);
                }
            }
        }

        private async void HandleClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int length = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, length);
                if (message.Contains("Heartbeat"))
                {
                    _lastHeartbeatReceived = DateTime.UtcNow; 
                    UpdateUI(true); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading from client: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }


        private void CheckForHeartbeatTimeout()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if ((DateTime.UtcNow - _lastHeartbeatReceived) > _heartbeatTimeout)
                    {
                        UpdateUI(false);
                    }
                    await Task.Delay(1000); // Check every second
                }
            });
        }



        private void UpdateUI(bool isAlive)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _statusManager.ConnectionStatus = isAlive ? Brushes.Green : Brushes.Red;
            });   
        }

       

        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            await _pipeClientService.SendMessageAsync("PipesOfPiece", PipeDirection.Out, "Hello from WPF!");
        }

     

        private async void InitializeSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/chatHub")
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    MessageBox.Show(newMessage);
                });
            });

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SignalRMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await _connection.InvokeAsync("SendMessage", "WPF User", "Hello from WPF");
        }



        private async void SendAndRecieveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new HelloWithObject { Name = "Nematulah", Family = "Hussaini", Age = 24 };
            await _pipeClientService.SendMessageAsync("SendReceive", PipeDirection.InOut, data);
        }

        private async void SendMessageToSignalR_Click(object sender, RoutedEventArgs e)
        {
            var data = "This message received from WPF to worker service via NamedPipeClientStream and then from worker service sent to chatHub via SignalR!";

            await _pipeClientService.SendMessageAsync("SendMessageToSignalR", PipeDirection.Out, data);

        }
    }


  
}
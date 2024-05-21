
using BookyApp.Helpers;
using BookyApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;

namespace BookyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private HubConnection _connection;
        private ConnectionStatusManager _statusManager = new ConnectionStatusManager();
        private NamedPipeClientService _pipeClientService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
            PresenceSignalR();
            DataContext = _statusManager;
            _pipeClientService = new NamedPipeClientService();
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

        private async void PresenceSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7031/presenceHub")
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            _connection.On<bool>("WorkerServiceStatus", (isAlive) =>
            {
                UpdateUI(isAlive);
            });

            await _connection.StartAsync();
            await _connection.SendAsync("SendMessage", "WPF service is running");

            // Request the current status of the worker service immediately after connecting
            await _connection.InvokeAsync("GetWorkerServiceStatus");
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